namespace AutoCAD_NET_4_8_Framework
{
    partial class UCGioiHanKhoi
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblInfoText = new System.Windows.Forms.Label();
            this.btnDeleteKhoi = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold);
            this.lblHeader.Location = new System.Drawing.Point(22, 16);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(103, 25);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Tên vùng";
            // 
            // lblInfoText
            // 
            this.lblInfoText.AutoSize = true;
            this.lblInfoText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.lblInfoText.Location = new System.Drawing.Point(24, 46);
            this.lblInfoText.Name = "lblInfoText";
            this.lblInfoText.Size = new System.Drawing.Size(76, 15);
            this.lblInfoText.TabIndex = 1;
            this.lblInfoText.Text = "Số vách / trụ:";
            // 
            // btnDeleteKhoi
            // 
            this.btnDeleteKhoi.Location = new System.Drawing.Point(27, 80);
            this.btnDeleteKhoi.Name = "btnDeleteKhoi";
            this.btnDeleteKhoi.Size = new System.Drawing.Size(140, 36);
            this.btnDeleteKhoi.TabIndex = 2;
            this.btnDeleteKhoi.Text = "Xóa vùng";
            this.btnDeleteKhoi.Click += new System.EventHandler(this.BtnDeleteKhoi_Click);
            // 
            // UCGioiHanKhoi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.lblInfoText);
            this.Controls.Add(this.btnDeleteKhoi);
            this.Name = "UCGioiHanKhoi";
            this.Size = new System.Drawing.Size(340, 325);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label  lblHeader;
        private System.Windows.Forms.Label  lblInfoText;
        private System.Windows.Forms.Button btnDeleteKhoi;
    }
}
