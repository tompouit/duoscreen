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

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags fuFlags, uint uTimeout, out UIntPtr lpdwResult);

        [DllImport("Shell32.dll")]
        public static extern int SHChangeNotify(HChangeNotifyEventID eventId, HChangeNotifyFlags flags, IntPtr item1, IntPtr item2);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowType uCmd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetTopWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(int uiAction, int uiParam, ref RectInter pvParam, int fWinIni);

        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("user32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder pClassName, int nMaxCount);


        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc pEnumFunc, IntPtr lParam);
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

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
            public RectInter(int left, int top, int right, int bottom) { this.left = left; this.top = top; this.right = right; this.bottom = bottom; }
            public static explicit operator Rectangle(RectInter rect) => new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
            public static explicit operator RectInter(Rectangle rect) => new RectInter(rect.Left, rect.Top, rect.Right, rect.Bottom);
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
            public RectInter rc;
            public IntPtr lParam;
        }

        public struct MONITORINFO
        {
            public int cbSize;
            public RectInter rcMonitor;
            public RectInter rcWorkArea;
            public uint dwFlags;
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

        public const int WM_SETTINGCHANGE= 0x001A;
        public const int WM_THEMECHANGED = 0x031A;

        public const int HWND_BROADCAST = 0xffff;

        public const int SPI_GETWORKAREA = 0x0030;

        public const int WPF_SETMINPOSITION = 0x0001;

        public const uint WS_MAXIMIZEBOX = 0x00010000;
        public const uint WS_SIZEBOX = 0x00040000;

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
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL             = 0x0,
            SMTO_BLOCK              = 0x1,
            SMTO_ABORTIFHUNG        = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }

        public enum GetWindowType : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        public enum HChangeNotifyEventID
        {
            SHCNE_ALLEVENTS = 0x7FFFFFFF,
            SHCNE_ASSOCCHANGED = 0x08000000,
            SHCNE_ATTRIBUTES = 0x00000800,
            SHCNE_CREATE = 0x00000002,
            SHCNE_DELETE = 0x00000004,
            SHCNE_DRIVEADD = 0x00000100,
            SHCNE_DRIVEADDGUI = 0x00010000,
            SHCNE_DRIVEREMOVED = 0x00000080,
            SHCNE_EXTENDED_EVENT = 0x04000000,
            SHCNE_FREESPACE = 0x00040000,
            SHCNE_MEDIAINSERTED = 0x00000020,
            SHCNE_MEDIAREMOVED = 0x00000040,
            SHCNE_MKDIR = 0x00000008,
            SHCNE_NETSHARE = 0x00000200,
            SHCNE_NETUNSHARE = 0x00000400,
            SHCNE_RENAMEFOLDER = 0x00020000,
            SHCNE_RENAMEITEM = 0x00000001,
            SHCNE_RMDIR = 0x00000010,
            SHCNE_SERVERDISCONNECT = 0x00004000,
            SHCNE_UPDATEDIR = 0x00001000,
            SHCNE_UPDATEIMAGE = 0x00008000,
        }

        public enum HChangeNotifyFlags
        {
            SHCNF_DWORD = 0x0003,
            SHCNF_IDLIST = 0x0000,
            SHCNF_PATHA = 0x0001,
            SHCNF_PATHW = 0x0005,
            SHCNF_PRINTERA = 0x0002,
            SHCNF_PRINTERW = 0x0006,
            SHCNF_FLUSH = 0x1000,
            SHCNF_FLUSHNOWAIT = 0x2000
        }
    }
}
