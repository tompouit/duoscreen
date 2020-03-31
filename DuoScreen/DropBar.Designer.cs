namespace DuoScreen
{
    partial class DropBar
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toLocation = new System.Windows.Forms.PictureBox();
            this.expand = new System.Windows.Forms.PictureBox();
            this.focusingTimer = new System.Windows.Forms.Timer(this.components);
            this.onLocation = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.toLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.expand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.onLocation)).BeginInit();
            this.SuspendLayout();
            // 
            // toLocation
            // 
            this.toLocation.Image = global::DuoScreen.Properties.Resources.ToBottomNone;
            this.toLocation.Location = new System.Drawing.Point(0, 0);
            this.toLocation.Margin = new System.Windows.Forms.Padding(0);
            this.toLocation.Name = "toLocation";
            this.toLocation.Size = new System.Drawing.Size(100, 81);
            this.toLocation.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.toLocation.TabIndex = 0;
            this.toLocation.TabStop = false;
            this.toLocation.Paint += new System.Windows.Forms.PaintEventHandler(this.ToLocation_Paint);
            // 
            // expand
            // 
            this.expand.Image = global::DuoScreen.Properties.Resources.Expand;
            this.expand.Location = new System.Drawing.Point(200, 0);
            this.expand.Margin = new System.Windows.Forms.Padding(0);
            this.expand.Name = "expand";
            this.expand.Size = new System.Drawing.Size(100, 81);
            this.expand.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.expand.TabIndex = 1;
            this.expand.TabStop = false;
            this.expand.Paint += new System.Windows.Forms.PaintEventHandler(this.Expand_Paint);
            // 
            // focusingTimer
            // 
            this.focusingTimer.Tick += new System.EventHandler(this.FocusingTimer_Tick);
            // 
            // onLocation
            // 
            this.onLocation.Image = global::DuoScreen.Properties.Resources.OnTopNone;
            this.onLocation.Location = new System.Drawing.Point(100, 0);
            this.onLocation.Margin = new System.Windows.Forms.Padding(0);
            this.onLocation.Name = "onLocation";
            this.onLocation.Size = new System.Drawing.Size(100, 81);
            this.onLocation.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.onLocation.TabIndex = 2;
            this.onLocation.TabStop = false;
            this.onLocation.Paint += new System.Windows.Forms.PaintEventHandler(this.OnLocation_Paint);
            // 
            // DropBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(351, 106);
            this.Controls.Add(this.onLocation);
            this.Controls.Add(this.expand);
            this.Controls.Add(this.toLocation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DropBar";
            this.ShowInTaskbar = false;
            this.Text = "DropBar";
            this.Shown += new System.EventHandler(this.DropBar_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.toLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.expand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.onLocation)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox toLocation;
        private System.Windows.Forms.PictureBox expand;
        private System.Windows.Forms.Timer focusingTimer;
        private System.Windows.Forms.PictureBox onLocation;
    }
}

