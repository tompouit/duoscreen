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

        protected override void OnShown(EventArgs e)
        {
            border.Show();
        }

        public void Update(DropBar.SelectionType selection)
        {
            Screen topScreen    = Screen.AllScreens[0];
            Screen bottomScreen = Screen.AllScreens[1];
            switch (selection)
            {
                case DropBar.SelectionType.ToBottom:
                case DropBar.SelectionType.ToBottomLeft:
                case DropBar.SelectionType.ToBottomRight:
                case DropBar.SelectionType.OnBottom:
                case DropBar.SelectionType.OnBottomLeft:
                case DropBar.SelectionType.OnBottomRight:
                    SetSizeTo1Screen(bottomScreen, selection);
                    break;
                case DropBar.SelectionType.ToTop:
                case DropBar.SelectionType.ToTopLeft:
                case DropBar.SelectionType.ToTopRight:
                case DropBar.SelectionType.OnTop:
                case DropBar.SelectionType.OnTopLeft:
                case DropBar.SelectionType.OnTopRight:
                    SetSizeTo1Screen(topScreen, selection);
                    break;
                case DropBar.SelectionType.Expand:
                    Position(topScreen.WorkingArea.X, topScreen.WorkingArea.Y, topScreen.WorkingArea.Width, bottomScreen.WorkingArea.Bottom);
                    break;
            }
        }

        private void SetSizeTo1Screen(Screen screen, DropBar.SelectionType selection)
        {
            switch (selection)
            {
                case DropBar.SelectionType.ToBottom:
                case DropBar.SelectionType.ToTop:
                case DropBar.SelectionType.OnBottom:
                case DropBar.SelectionType.OnTop:
                    Position(screen.WorkingArea.X, screen.WorkingArea.Y, screen.WorkingArea.Width, screen.WorkingArea.Height);
                    break;
                case DropBar.SelectionType.ToBottomLeft:
                case DropBar.SelectionType.ToTopLeft:
                case DropBar.SelectionType.OnBottomLeft:
                case DropBar.SelectionType.OnTopLeft:
                    Position(screen.WorkingArea.X, screen.WorkingArea.Y, screen.WorkingArea.Width/2, screen.WorkingArea.Height);
                    break;
                case DropBar.SelectionType.ToBottomRight:
                case DropBar.SelectionType.ToTopRight:
                case DropBar.SelectionType.OnBottomRight:
                case DropBar.SelectionType.OnTopRight:
                    Position(screen.WorkingArea.X+screen.WorkingArea.Width/2, screen.WorkingArea.Y, screen.WorkingArea.Width/2, screen.WorkingArea.Height);
                    break;
            }
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
