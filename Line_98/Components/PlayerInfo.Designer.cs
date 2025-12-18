namespace Line_98.Components
{
    partial class PlayerInfo
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelPlayerInfo = new System.Windows.Forms.Panel();
            this.labelTimePlayed = new System.Windows.Forms.Label();
            this.labelScore = new System.Windows.Forms.Label();
            this.labelPlayerName = new System.Windows.Forms.Label();
            this.panelPlayerInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelPlayerInfo
            // 
            this.panelPlayerInfo.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panelPlayerInfo.Controls.Add(this.labelTimePlayed);
            this.panelPlayerInfo.Controls.Add(this.labelScore);
            this.panelPlayerInfo.Controls.Add(this.labelPlayerName);
            this.panelPlayerInfo.Location = new System.Drawing.Point(3, 3);
            this.panelPlayerInfo.Name = "panelPlayerInfo";
            this.panelPlayerInfo.Size = new System.Drawing.Size(738, 66);
            this.panelPlayerInfo.TabIndex = 0;
            // 
            // labelTimePlayed
            // 
            this.labelTimePlayed.AutoSize = true;
            this.labelTimePlayed.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTimePlayed.Location = new System.Drawing.Point(319, 19);
            this.labelTimePlayed.Name = "labelTimePlayed";
            this.labelTimePlayed.Size = new System.Drawing.Size(125, 25);
            this.labelTimePlayed.TabIndex = 3;
            this.labelTimePlayed.Text = "TimePlayed";
            this.labelTimePlayed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelScore
            // 
            this.labelScore.AutoSize = true;
            this.labelScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelScore.Location = new System.Drawing.Point(623, 19);
            this.labelScore.Name = "labelScore";
            this.labelScore.Size = new System.Drawing.Size(68, 25);
            this.labelScore.TabIndex = 2;
            this.labelScore.Text = "Score";
            this.labelScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelPlayerName
            // 
            this.labelPlayerName.AutoSize = true;
            this.labelPlayerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPlayerName.Location = new System.Drawing.Point(39, 19);
            this.labelPlayerName.Name = "labelPlayerName";
            this.labelPlayerName.Size = new System.Drawing.Size(129, 25);
            this.labelPlayerName.TabIndex = 1;
            this.labelPlayerName.Text = "PlayerName";
            this.labelPlayerName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PlayerInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelPlayerInfo);
            this.Name = "PlayerInfo";
            this.Size = new System.Drawing.Size(744, 72);
            this.panelPlayerInfo.ResumeLayout(false);
            this.panelPlayerInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelPlayerInfo;
        private System.Windows.Forms.Label labelTimePlayed;
        private System.Windows.Forms.Label labelScore;
        private System.Windows.Forms.Label labelPlayerName;
    }
}
