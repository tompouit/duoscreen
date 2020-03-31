using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DuoScreen
{
    static public class WinAPI
    {
        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, long idObject, long idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate pfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder pString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out PointInter pPoint);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int Y, int cx, int cy, uint wFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, out RectInter pRect);

        [DllImport("user32.dll")]
        public static extern IntPtr GetClientRect(IntPtr hWnd, out RectInter pRect);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT pWndpl);

        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT pWndpl);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
        public  static extern uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string pClassName, string pWindowName);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern bool AdjustWindowRectEx(ref RectInter pRect, uint dwStyle, bool bMenu, uint dwExStyle);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref PointInter pPoint);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [StructLayout(LayoutKind.Sequential)]
        public struct PointInter
        {
            public int X;
            public int Y;
            public PointInter(int x, int y) { X = x; Y = y; }
            public static explicit operator Point(PointInter point) => new Point(point.X, point.Y);
            public static implicit operator PointInter(Point point) => new PointInter(point.X, point.Y);
        }

        public struct RectInter
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public static explicit operator Rectangle(RectInter rect) => new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
        }

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public PointInter ptMinPosition;
            public PointInter ptMaxPosition;
            public RectInter rcNormalPosition;
        }

        public struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_NORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_MAXIMIZE = 3;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_SHOW = 5;
        public const int SW_MINIMIZE = 6;
        public const int SW_SHOWMINNOACTIVE = 7;
        public const int SW_SHOWNA = 8;
        public const int SW_RESTORE = 9;

        public const int HWND_BOTTOM = 1;
        public const int HWND_NOTOPMOST = -2;
        public const int HWND_TOP = 0;
        public const int HWND_TOPMOST = -1;

        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;

        public const int SWP_ASYNCWINDOWPOS = 0x4000;
        public const int SWP_DEFERERASE = 0x2000;
        public const int SWP_DRAWFRAME = 0x0020;
        public const int SWP_FRAMECHANGED = 0x0020;
        public const int SWP_HIDEWINDOW = 0x0080;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_NOCOPYBITS = 0x0100;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOOWNERZORDER = 0x0200;
        public const int SWP_NOREDRAW = 0x0008;
        public const int SWP_NOREPOSITION = 0x0200;
        public const int SWP_NOSENDCHANGING = 0x0400;
        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_SHOWWINDOW = 0x0040;

        public const uint EVENT_SYSTEM_FOREGROUND = 3;
        public const uint WINEVENT_OUTOFCONTEXT = 0x0000;
        public const uint WINEVENT_INCONTEXT = 0x0004;
        public const uint WINEVENT_SKIPOWNPROCESS = 0x0002;
        public const uint EVENT_SYSTEM_MOVESIZESTART = 0x000A;
        public const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;

        public const uint EVENT_OBJECT_DRAGSTART = 0x8021;
        public const uint EVENT_OBJECT_DRAGENTER = 0x8024;
        public const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;

        public const uint OBJID_WINDOW = 0x00000000;
        public const uint OBJID_TITLEBAR = 0xFFFFFFFE;
        public const uint CHILDID_SELF = 0;


        public enum ABMsg : int
        {
            ABM_NEW = 0,
            ABM_REMOVE,
            ABM_QUERYPOS,
            ABM_SETPOS,
            ABM_GETSTATE,
            ABM_GETTASKBARPOS,
            ABM_ACTIVATE,
            ABM_GETAUTOHIDEBAR,
            ABM_SETAUTOHIDEBAR,
            ABM_WINDOWPOSCHANGED,
            ABM_SETSTATE
        }
    }
}
