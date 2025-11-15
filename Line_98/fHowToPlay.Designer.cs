namespace Line_98 {
    partial class fHowToPlay {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fHowToPlay));
            this.rtbHowToPlay = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtbHowToPlay
            // 
            this.rtbHowToPlay.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbHowToPlay.Font = new System.Drawing.Font("Times New Roman", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbHowToPlay.Location = new System.Drawing.Point(12, 12);
            this.rtbHowToPlay.Name = "rtbHowToPlay";
            this.rtbHowToPlay.ReadOnly = true;
            this.rtbHowToPlay.Size = new System.Drawing.Size(379, 426);
            this.rtbHowToPlay.TabIndex = 0;
            this.rtbHowToPlay.Text = resources.GetString("rtbHowToPlay.Text");
            // 
            // fHowToPlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 450);
            this.Controls.Add(this.rtbHowToPlay);
            this.Name = "fHowToPlay";
            this.Text = "fHowToPlay";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbHowToPlay;
    }
}