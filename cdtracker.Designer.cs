namespace applbot_CDTracker
{
    partial class cdtracker
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
            this.btnResetPos = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnResetPos
            // 
            this.btnResetPos.Location = new System.Drawing.Point(16, 13);
            this.btnResetPos.Name = "btnResetPos";
            this.btnResetPos.Size = new System.Drawing.Size(110, 23);
            this.btnResetPos.TabIndex = 0;
            this.btnResetPos.Text = "Reset Position";
            this.btnResetPos.UseVisualStyleBackColor = true;
            this.btnResetPos.Click += new System.EventHandler(this.btnResetPos_Click);
            // 
            // cdtracker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnResetPos);
            this.Name = "cdtracker";
            this.Size = new System.Drawing.Size(289, 150);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnResetPos;
    }
}
