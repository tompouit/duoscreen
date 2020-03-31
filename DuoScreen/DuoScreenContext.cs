using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Topshelf.Runtime.Windows;

namespace DuoScreen
{

    /// <summary>
    /// The main application, managing hooks and notifyicon.
    /// It display a <see cref="DropBar"/> form when moving a window.
    /// </summary>
    public class DuoScreenContext : ApplicationContext
    {
        private static GCHandle GCSafetyHandle;
        private IContainer components;
        private NotifyIcon notifyIcon;
        private WinAPI.WinEventDelegate winEventDelegate;
        private IntPtr winEventHook;
        private DropBar dropBar;
        private MoveDetect moveDetect;

        /// <summary>
        /// Manager used to distinguish windows moving from windows resizing.<br></br>
        /// It call <see cref="beginMoving"/>, <see cref="endMoving"/>, and <see cref="continueMoving"/> callback, identifying moving events from EVENT_SYSTEM_MOVESIZESTART, EVENT_SYSTEM_MOVESIZEEND, EVENT_OBJECT_LOCATIONCHANGE messages.
        /// </summary>
        class MoveDetect
        {
            public delegate void Callback(IntPtr hWnd);
            private enum Phase { None, StartMoveOrSize, Moving, Sizing }

            private Phase phase;
            private Size startWndSize;
            private IntPtr movingWnd;
            private readonly Callback beginMoving;
            private readonly Callback endMoving;
            private readonly Callback continueMoving;

            public MoveDetect(Callback beginFunc, Callback endFunc, Callback continueFunc)
            {
                beginMoving += beginFunc;
                endMoving += endFunc;
                continueMoving += continueFunc;
            }

            public void StartMoveOrSize(IntPtr hWnd)
            {
                WinAPI.WINDOWPLACEMENT wndPlace = new WinAPI.WINDOWPLACEMENT();
                wndPlace.length = Marshal.SizeOf(wndPlace);
                WinAPI.GetWindowPlacement(hWnd, out wndPlace);
                if ((wndPlace.showCmd & WinAPI.SW_SHOWMAXIMIZED) == WinAPI.SW_SHOWMAXIMIZED)
                {
                    phase = Phase.Moving;
                    movingWnd = hWnd;
                    beginMoving(hWnd);
                }
                else
                {
                    WinAPI.GetWindowRect(hWnd, out WinAPI.RectInter r);
                    startWndSize = new Size(r.right - r.left, r.bottom - r.top);
                    phase = Phase.StartMoveOrSize;
                }
            }

            public void EndMoveOrSize()
            {
                if (phase == Phase.Moving)
                    endMoving(movingWnd);
                phase = Phase.None;
                movingWnd = IntPtr.Zero;
            }

            public void ChangeMoveOrSize(IntPtr hWnd)
            {
                if (phase == Phase.StartMoveOrSize)
                {
                    WinAPI.GetWindowRect(hWnd, out WinAPI.RectInter r);
                    Size wndSize = new Size(r.right - r.left, r.bottom - r.top);
                    if (wndSize == startWndSize)
                    {
                        phase = Phase.Moving;
                        movingWnd = hWnd;
                        beginMoving(hWnd);
                    }
                    //else phase = Phase.Sizing;   <-- Does not work well in some cases. Certainly some special windows can quickly change their size before restoring it and begining to move.
                }
                else if (phase == Phase.Moving)
                {
                    if (movingWnd == hWnd)  //Really usefull for some cases.
                        continueMoving(hWnd);
                }
            }
        }
        

        public DuoScreenContext()
        {
            InitializeContext();
        }

        private void InitializeContext()
        {
            //Create notify icon, with it's context menu:
            components = new Container();
            notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = Properties.Resources.DuoScreen,
                Text = Program.name,
                Visible = true
            };
            ToolStripMenuItem item = new ToolStripMenuItem("&Exit");
            item.Click += ExitItem_Click;
            notifyIcon.ContextMenuStrip.Items.Add(item);

            //Create WindowsEvent hook:
            winEventDelegate = new WinAPI.WinEventDelegate(WinEventCallback);
            GCSafetyHandle = GCHandle.Alloc(winEventDelegate);
            winEventHook = WinAPI.SetWinEventHook(WinAPI.EVENT_SYSTEM_MOVESIZESTART, WinAPI.EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, winEventDelegate, 0, 0, WinAPI.WINEVENT_OUTOFCONTEXT | WinAPI.WINEVENT_SKIPOWNPROCESS);

            moveDetect = new MoveDetect(BeginMovingWindow, EndMovingWindow, ContinueMovingWindow);
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            ExitThread();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        protected override void ExitThreadCore()
        {
            if (dropBar != null)
                dropBar.Close();
            notifyIcon.Visible = false;
            GCSafetyHandle.Free();
            WinAPI.UnhookWinEvent(winEventHook);
            base.ExitThreadCore();
        }

        void WinEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, long idObject, long idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (idObject == WinAPI.OBJID_WINDOW)
            {
                if (eventType == WinAPI.EVENT_SYSTEM_MOVESIZESTART)
                    moveDetect.StartMoveOrSize(hWnd);
                else if (eventType == WinAPI.EVENT_SYSTEM_MOVESIZEEND)
                    moveDetect.EndMoveOrSize();
                else if (eventType == WinAPI.EVENT_OBJECT_LOCATIONCHANGE)
                    moveDetect.ChangeMoveOrSize(hWnd);
            }
        }

        private void BeginMovingWindow(IntPtr hWnd)
        {
            Debug.Assert(dropBar == null);

            DropBar.WindowPosType windowPos = (Screen.FromHandle(hWnd).Primary ? DropBar.WindowPosType.OnTop : DropBar.WindowPosType.OnBottom);
            dropBar = new DropBar(windowPos);
            dropBar.Show();
        }

        private void ContinueMovingWindow(IntPtr hWnd)
        {
            Debug.Assert(dropBar != null);

            dropBar.UpdateWholeDisplay();
        }

        private void EndMovingWindow(IntPtr hWnd)
        {
            Debug.Assert(dropBar != null);

            switch (dropBar.Selection)
            {
                case DropBar.SelectionType.ToBottom:
                case DropBar.SelectionType.OnBottom:
                    MaximizeWindowToScreen(hWnd, Screen.AllScreens[1]);
                    break;
                case DropBar.SelectionType.ToBottomLeft:
                case DropBar.SelectionType.OnBottomLeft:
                    ExpandWindowToHalfScreen(hWnd, Screen.AllScreens[1], true);
                    break;
                case DropBar.SelectionType.ToBottomRight:
                case DropBar.SelectionType.OnBottomRight:
                    ExpandWindowToHalfScreen(hWnd, Screen.AllScreens[1], false);
                    break;
                case DropBar.SelectionType.ToTop:
                case DropBar.SelectionType.OnTop:
                    MaximizeWindowToScreen(hWnd, Screen.AllScreens[0]);
                    break;
                case DropBar.SelectionType.ToTopLeft:
                case DropBar.SelectionType.OnTopLeft:
                    ExpandWindowToHalfScreen(hWnd, Screen.AllScreens[0], true);
                    break;
                case DropBar.SelectionType.ToTopRight:
                case DropBar.SelectionType.OnTopRight:
                    ExpandWindowToHalfScreen(hWnd, Screen.AllScreens[0], false);
                    break;
                case DropBar.SelectionType.Expand:
                    ExpandWindowToBothScreens(hWnd);
                    break;
            }
            dropBar.Close();
            dropBar = null;
        }

        private void MaximizeWindowToScreen(IntPtr hWnd, Screen screen)
        {
            Rectangle screenArea = screen.WorkingArea;
            WinAPI.WINDOWPLACEMENT wndPlace = new WinAPI.WINDOWPLACEMENT();
            wndPlace.length = Marshal.SizeOf(wndPlace);
            WinAPI.GetWindowPlacement(hWnd, out wndPlace);

            WinAPI.WINDOWPLACEMENT newPlace = new WinAPI.WINDOWPLACEMENT();
            newPlace.showCmd = WinAPI.SW_MAXIMIZE;
            newPlace.ptMaxPosition = screenArea.Location;
            newPlace.ptMinPosition = screenArea.Location;
            newPlace.rcNormalPosition.left = screenArea.Left;
            newPlace.rcNormalPosition.top = screenArea.Top;
            newPlace.rcNormalPosition.right = newPlace.rcNormalPosition.left + wndPlace.rcNormalPosition.right - wndPlace.rcNormalPosition.left;
            newPlace.rcNormalPosition.bottom = newPlace.rcNormalPosition.top + wndPlace.rcNormalPosition.bottom - wndPlace.rcNormalPosition.top;
            newPlace.length = Marshal.SizeOf(newPlace);
            WinAPI.SetWindowPlacement(hWnd, ref newPlace);
        }

        private void ExpandWindowToHalfScreen(IntPtr hWnd, Screen screen, bool toLeft)
        {
            //Note: should use Aero Snap, but there is no API for it...
            Rectangle screenArea = screen.WorkingArea;
            WinAPI.RectInter newPos = new WinAPI.RectInter();
            if (toLeft)  { newPos.left = screenArea.Left; newPos.right = screenArea.Right / 2; }
            else  { newPos.left = screenArea.Left+screenArea.Right/2; newPos.right = screenArea.Right; }
            newPos.top = screenArea.Top; newPos.bottom = screenArea.Bottom;
            WinAPI.AdjustWindowRectEx(ref newPos, WinAPI.GetWindowLong(hWnd, WinAPI.GWL_STYLE), false, WinAPI.GetWindowLong(hWnd, WinAPI.GWL_EXSTYLE));
            newPos.bottom--;
            WinAPI.SetWindowPos(hWnd, IntPtr.Zero, newPos.left, screenArea.Top, newPos.right - newPos.left, newPos.bottom - screenArea.Top, WinAPI.SWP_ASYNCWINDOWPOS | WinAPI.SWP_NOZORDER);
        }

        private void ExpandWindowToBothScreens(IntPtr hWnd)
        {
            Rectangle topScreenArea     = Screen.AllScreens[0].WorkingArea;
            Rectangle bottomScreenArea  = Screen.AllScreens[1].WorkingArea;
            bool topScreenNeedsHideTaskBar       = (topScreenArea.Bottom != Screen.AllScreens[0].Bounds.Bottom);
            bool bottomScreenNeedsHideTaskBar    = (bottomScreenArea.Top != Screen.AllScreens[1].Bounds.Top);

            WinAPI.WINDOWPLACEMENT wndPlace = new WinAPI.WINDOWPLACEMENT();
            wndPlace.length = Marshal.SizeOf(wndPlace);
            WinAPI.GetWindowPlacement(hWnd, out wndPlace);

            WinAPI.RectInter newPos = new WinAPI.RectInter
            {
                left = topScreenArea.Left,
                top = topScreenArea.Top,
                right = topScreenArea.Right,
                bottom = bottomScreenArea.Bottom
            };
            WinAPI.AdjustWindowRectEx(ref newPos, WinAPI.GetWindowLong(hWnd, WinAPI.GWL_STYLE), false, WinAPI.GetWindowLong(hWnd, WinAPI.GWL_EXSTYLE));
            newPos.top = topScreenArea.Top;

            WinAPI.WINDOWPLACEMENT newPlace = new WinAPI.WINDOWPLACEMENT
            {
                showCmd = WinAPI.SW_NORMAL,
                ptMaxPosition = Point.Empty,
                ptMinPosition = Point.Empty,
                rcNormalPosition = newPos,
                length = Marshal.SizeOf(typeof(WinAPI.WINDOWPLACEMENT))
            };
            WinAPI.SetWindowPlacement(hWnd, ref newPlace);
        }
    }

}
