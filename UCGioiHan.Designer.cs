namespace AutoCAD_NET_4_8_Framework
{
    partial class UCGioiHan
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
            this.lblHeader       = new System.Windows.Forms.Label();
            this.lblInfo         = new System.Windows.Forms.Label();
            this.btnAddBlock     = new System.Windows.Forms.Button();
            this.btnDeleteGioiHan = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblHeader
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font     = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold);
            this.lblHeader.Location = new System.Drawing.Point(23, 31);
            this.lblHeader.Text     = "Tên giới hạn";

            // lblInfo
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font     = new System.Drawing.Font("Microsoft Sans Serif", 8.75F);
            this.lblInfo.Location = new System.Drawing.Point(25, 65);
            this.lblInfo.Text     = "Thông số: ";

            // btnAddBlock
            this.btnAddBlock.Location = new System.Drawing.Point(28, 120);
            this.btnAddBlock.Size     = new System.Drawing.Size(140, 36);
            this.btnAddBlock.Text     = "+ Thêm vùng";
            this.btnAddBlock.Click   += new System.EventHandler(this.BtnAddBlock_Click);

            // btnDeleteGioiHan
            this.btnDeleteGioiHan.Location = new System.Drawing.Point(28, 162);
            this.btnDeleteGioiHan.Size     = new System.Drawing.Size(140, 36);
            this.btnDeleteGioiHan.Text     = "Xóa giới hạn";
            this.btnDeleteGioiHan.Click   += new System.EventHandler(this.BtnDeleteGioiHan_Click);

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblHeader, this.lblInfo, this.btnAddBlock, this.btnDeleteGioiHan
            });
            this.Name = "UCGioiHan";
            this.Size = new System.Drawing.Size(340, 343);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label  lblHeader;
        private System.Windows.Forms.Label  lblInfo;
        private System.Windows.Forms.Button btnAddBlock;
        private System.Windows.Forms.Button btnDeleteGioiHan;
    }
}
