using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DuoScreen
{
    /// <summary>
    /// The form displaying button shortcuts.
    /// If the mouse stay into a button for some time, it "focus" it, specializing selection.
    /// </summary>
    public partial class DropBar : Form
    {
        private const int HoverMargin = 30;  //in pixels.
        private const float MagnifyFactor = 1.5f;
        private const int DisplayMargin = 50;  //in pixels.
        private const int FocusingDelay = 1500;  //in ms.
        private readonly Color SelectionColor = Color.FromArgb(75, 240, 248, 255);

        public enum SelectionType { None, ToTop, ToTopLeft, ToTopRight, ToBottom, ToBottomLeft, ToBottomRight, OnTop, OnTopLeft, OnTopRight, OnBottom, OnBottomLeft, OnBottomRight, Expand }
        public enum WindowPosType { OnTop, OnBottom }

        public SelectionType Selection { get; private set; }
        private bool magnified;
        private DropZone dropZone;
        private Control focusedCtrl;
        private Control focusingCtrl;
        private readonly WindowPosType windowPos;
        private readonly Brush selectionBrush;

        public DropBar(WindowPosType _windowPos)
        {
            InitializeComponent();
            focusingTimer.Interval = FocusingDelay;
            magnified       = true;
            Selection       = SelectionType.None;
            windowPos       = _windowPos;
            selectionBrush  = new SolidBrush(SelectionColor);
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (dropZone != null)
                dropZone.Close();
        }

        private void ToLocation_Paint(object sender, PaintEventArgs e)
        {
            switch (Selection)
            {
                case SelectionType.ToTop:
                case SelectionType.ToTopLeft:
                case SelectionType.ToTopRight:
                case SelectionType.ToBottom:
                case SelectionType.ToBottomLeft:
                case SelectionType.ToBottomRight:
                    e.Graphics.FillRectangle(selectionBrush, toLocation.ClientRectangle);
                    break;
            }
        }

        private void OnLocation_Paint(object sender, PaintEventArgs e)
        {
            switch (Selection)
            {
                case SelectionType.OnTop:
                case SelectionType.OnTopLeft:
                case SelectionType.OnTopRight:
                case SelectionType.OnBottom:
                case SelectionType.OnBottomLeft:
                case SelectionType.OnBottomRight:
                    e.Graphics.FillRectangle(selectionBrush, onLocation.ClientRectangle);
                    break;
            }
        }

        private void Expand_Paint(object sender, PaintEventArgs e)
        {
            switch (Selection)
            {
                case SelectionType.Expand:
                    e.Graphics.FillRectangle(selectionBrush, expand.ClientRectangle);
                    break;
            }
        }

        public void UpdateWholeDisplay()
        {
            WinAPI.GetCursorPos(out WinAPI.PointInter _pos); Point pos = PointToClient((Point)_pos);

            UpdateMagnify(pos);
            UpdateFocusing(pos);
            UpdateSelection(pos, false);
        }

        private void UpdateMagnify(Point pos)
        {
            Rectangle hoverRectangle = ClientRectangle; hoverRectangle.Inflate(HoverMargin, HoverMargin);
            SetMagnify(hoverRectangle.Contains(pos));
        }

        private void SetMagnify(bool magnify)
        {
            float scaling = 0;
            if (magnify && !this.magnified)
                scaling = MagnifyFactor;
            else if (!magnify && this.magnified)
                scaling = 1f/MagnifyFactor;
            if (scaling != 0)
            {
                Rectangle r0 = DesktopBounds;
                Scale(new SizeF(scaling, scaling));
                Rectangle r1 = DesktopBounds;
                SetDesktopLocation(r1.X - (r1.Width-r0.Width)/2, r1.Y - (r1.Height-r0.Height)/2);
                this.magnified = magnify;
            }
        }

        private void UpdateFocusing(Point pos)
        {
            Control newFocusingCtrl = GetChildAtPoint(pos);
            if (focusingCtrl != newFocusingCtrl)
                focusingTimer.Stop();
            focusingCtrl = newFocusingCtrl;
            if (focusingCtrl != null)
                focusingTimer.Start();
        }

        private void UpdateSelection(Point pos, bool startFocus)
        {
            SelectionType oldSelection = Selection;
            Control selectedCtrl = GetChildAtPoint(pos);

            //Update focus:
            if (selectedCtrl == null)
                focusedCtrl = null;         //Outside of the bar --> reset focus.
            else if ((focusedCtrl != null) || startFocus)
                focusedCtrl = selectedCtrl; //Inside the bar --> update focus if it is already set.

            //Update selection:
            if (selectedCtrl == toLocation)
            {
                if (focusedCtrl == toLocation)
                    if (pos.X < toLocation.Width/2)
                        Selection = (windowPos == WindowPosType.OnTop) ? SelectionType.ToBottomLeft : SelectionType.ToTopLeft;
                    else
                        Selection = (windowPos == WindowPosType.OnTop) ? SelectionType.ToBottomRight : SelectionType.ToTopRight;
                 else
                    Selection = (windowPos == WindowPosType.OnTop) ? SelectionType.ToBottom : SelectionType.ToTop;
                //Update focused button:
                toLocation.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject(Selection.ToString());
            }
            else if (selectedCtrl == onLocation)
            {
                if (focusedCtrl == onLocation)
                    if (pos.X < onLocation.Location.X+onLocation.Width/2)
                        Selection = (windowPos == WindowPosType.OnTop) ? SelectionType.OnTopLeft : SelectionType.OnBottomLeft;
                    else
                        Selection = (windowPos == WindowPosType.OnTop) ? SelectionType.OnTopRight : SelectionType.OnBottomRight;
                else
                    Selection = (windowPos == WindowPosType.OnTop) ? SelectionType.OnTop : SelectionType.OnBottom;
                //Update focused button:
                onLocation.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject(Selection.ToString());
            }
            else if (selectedCtrl == expand)
                Selection = SelectionType.Expand;
            else
                Selection = SelectionType.None;

            //Restore unfocused buttons:
            if (selectedCtrl != toLocation)
                toLocation.Image = (windowPos == WindowPosType.OnTop) ? Properties.Resources.ToBottomNone : Properties.Resources.ToTopNone;
            if (selectedCtrl != onLocation)
                onLocation.Image = (windowPos == WindowPosType.OnTop) ? Properties.Resources.OnTopNone : Properties.Resources.OnBottomNone;

            if (Selection != oldSelection)
            {
                UpdateDropZone();
                Invalidate(true);
            }
        }

        private void UpdateDropZone()
        {
            if (Selection == SelectionType.None)
            {
                if (dropZone != null)
                {
                    dropZone.Close();
                    dropZone = null;
                }
            }
            else
            {
                if (dropZone == null)
                {
                    dropZone = new DropZone { Owner = this };
                    dropZone.Show();
                }
                dropZone.Update(Selection);
            }
        }

        private void DropBar_Shown(object sender, EventArgs e)
        {
            SetMagnify(true);  //Reset magnification.
            //Set location near cursor position:
            WinAPI.GetCursorPos(out WinAPI.PointInter _pos);  Point pos = (Point)_pos;
            Screen screen = Screen.FromPoint(pos);
            Rectangle workingArea = screen.WorkingArea;
            Point newLocation = new Point(pos.X - Width/2, pos.Y + DisplayMargin);
            //Adjust location to stay into working area:
            Point delta = screen.Primary ? Point.Empty : GetBottomScreenDelta();
            if (pos.Y + DisplayMargin + Height > delta.Y + workingArea.Height)
                newLocation.Y = pos.Y - Screen.PrimaryScreen.WorkingArea.Top - DisplayMargin - Height;
            if (pos.X - Width/2 < delta.X)
                newLocation.X = delta.X;
            else if (pos.X + Width/2 > delta.X + workingArea.Width)
                newLocation.X = delta.X + workingArea.Width - Width;
            DesktopLocation = newLocation;
            //Set TopMost:
            WinAPI.SetWindowPos(Handle, (IntPtr)WinAPI.HWND_TOPMOST, 0, 0, 0, 0, WinAPI.SWP_NOMOVE | WinAPI.SWP_NOSIZE | WinAPI.SWP_NOACTIVATE);
            //Initialize button's bitmap:
            switch (windowPos)
            {
                case WindowPosType.OnBottom:
                    toLocation.Image = Properties.Resources.ToTopNone;
                    onLocation.Image = Properties.Resources.OnBottomNone;
                    break;
            }
            //Shrink the bar:
            UpdateMagnify(PointToClient(pos));
        }

        private Point GetBottomScreenDelta()
        {
            Screen topScreen    = Screen.AllScreens[0];
            Screen bottomScreen = Screen.AllScreens[1];
            return new Point(bottomScreen.WorkingArea.Left - topScreen.WorkingArea.Left, topScreen.WorkingArea.Height + (topScreen.Bounds.Bottom - topScreen.WorkingArea.Bottom) + (bottomScreen.WorkingArea.Top - bottomScreen.Bounds.Top));
        }

        private void FocusingTimer_Tick(object sender, EventArgs e)
        {
            focusingTimer.Stop();
            WinAPI.GetCursorPos(out WinAPI.PointInter _pos); Point pos = PointToClient((Point)_pos);
            UpdateSelection(pos, true);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }
    }
}
