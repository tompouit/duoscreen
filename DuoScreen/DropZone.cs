using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DuoScreen
{

    /// <summary>
    /// This form is displayed over a region to visualize where the selected window will be dropped.
    /// It is made with 2 forms: a semi-opaque rectangle, and a fully opaque border above it.
    /// </summary>
    public class DropZone : Form
    {
        private const int MarginSize = 10;
        private const double FillOpacity = 0.4;
        private const float BorderSize = 5;
        private static readonly Color FillColor = Color.LightCyan;
        private static readonly Color BorderColor = Color.DodgerBlue;

        private readonly OpaqueBorder border;

        public class OpaqueBorder : Form
        {
            public OpaqueBorder(Form owner)
            {
                Owner = owner;
                TransparencyKey = Color.Red;
                BackColor = Color.Red;
                ShowInTaskbar = false;
                FormBorderStyle = FormBorderStyle.None;
                ControlBox = false;
                StartPosition = FormStartPosition.Manual;
                Paint += new System.Windows.Forms.PaintEventHandler(OpaqueBorder_Paint);
            }

            protected override bool ShowWithoutActivation
            {
                get { return true; }
            }

            private void OpaqueBorder_Paint(object sender, PaintEventArgs e)
            {
                Pen pen = new Pen(BorderColor, BorderSize);
                Rectangle border = ClientRectangle; border.Width--; border.Height--;
                e.Graphics.DrawRectangle(pen, border);
            }
        }

        public DropZone()
        {
            InitializeComponent();
            BackColor = FillColor;
            Opacity = FillOpacity;
            TopLevel = true;

            border = new OpaqueBorder(this);
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                //Adde shadow under the form:
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            border.Show();
        }

        public void Update(DropBar.SelectionType selection)
        {
            Screen topScreen    = Screen.AllScreens[0];
            Screen bottomScreen = Screen.AllScreens[1];
            Point bottomScreenDelta = GetBottomScreenDelta();
            switch (selection)
            {
                case DropBar.SelectionType.ToTop:
                case DropBar.SelectionType.OnTop:
                case DropBar.SelectionType.ToTopLeft:
                case DropBar.SelectionType.OnTopLeft:
                case DropBar.SelectionType.ToTopRight:
                case DropBar.SelectionType.OnTopRight:
                    SetSizeTo1Screen(topScreen, Point.Empty, selection);
                    break;
                case DropBar.SelectionType.ToBottom:
                case DropBar.SelectionType.OnBottom:
                case DropBar.SelectionType.ToBottomLeft:
                case DropBar.SelectionType.OnBottomLeft:
                case DropBar.SelectionType.ToBottomRight:
                case DropBar.SelectionType.OnBottomRight:
                    SetSizeTo1Screen(bottomScreen, new Point(bottomScreenDelta.X, bottomScreenDelta.Y+topScreen.WorkingArea.Height), selection);
                    break;
                case DropBar.SelectionType.Expand:
                    Position(Math.Max(topScreen.Bounds.Left, bottomScreen.WorkingArea.Left), 0, Math.Min(topScreen.WorkingArea.Width, bottomScreen.WorkingArea.Width), bottomScreenDelta.Y+topScreen.WorkingArea.Height+bottomScreen.WorkingArea.Height);
                    break;
            }
        }

        private void SetSizeTo1Screen(Screen screen, Point delta, DropBar.SelectionType selection)
        {
            switch (selection)
            {
                case DropBar.SelectionType.ToTop:
                case DropBar.SelectionType.OnTop:
                case DropBar.SelectionType.ToBottom:
                case DropBar.SelectionType.OnBottom:
                    Position(delta.X, delta.Y, screen.WorkingArea.Width, screen.WorkingArea.Height);
                    break;
                case DropBar.SelectionType.ToTopLeft:
                case DropBar.SelectionType.OnTopLeft:
                case DropBar.SelectionType.ToBottomLeft:
                case DropBar.SelectionType.OnBottomLeft:
                    Position(delta.X, delta.Y, screen.WorkingArea.Width/2, screen.WorkingArea.Height);
                    break;
                case DropBar.SelectionType.ToTopRight:
                case DropBar.SelectionType.OnTopRight:
                case DropBar.SelectionType.ToBottomRight:
                case DropBar.SelectionType.OnBottomRight:
                    Position(delta.X+screen.WorkingArea.Width/2, delta.Y, screen.WorkingArea.Width/2, screen.WorkingArea.Height);
                    break;
            }
        }

        private Point GetBottomScreenDelta()
        {
            Screen topScreen    = Screen.AllScreens[0];
            Screen bottomScreen = Screen.AllScreens[1];
            return new Point(bottomScreen.WorkingArea.Left - topScreen.WorkingArea.Left, (topScreen.Bounds.Bottom - topScreen.WorkingArea.Bottom) + (bottomScreen.WorkingArea.Top - bottomScreen.Bounds.Top));
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DropZone
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DropZone";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.ResumeLayout(false);

        }

        private void Position(int x, int y, int width, int height)
        {
            Rectangle bounds = new Rectangle(x+MarginSize, y+MarginSize, width-MarginSize*2, height-MarginSize*2);
            DesktopBounds = bounds;
            border.DesktopBounds = bounds;
            Invalidate(true);
            border.Invalidate(true);
        }
    }

}
