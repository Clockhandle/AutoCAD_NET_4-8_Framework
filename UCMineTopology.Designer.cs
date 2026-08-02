namespace AutoCAD_NET_4_8_Framework
{
    partial class UCMineTopology
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
            this.lblNen        = new System.Windows.Forms.Label();
            this.listNen       = new System.Windows.Forms.ListBox();
            this.btnSelectNen  = new System.Windows.Forms.Button();
            this.btnClearNen   = new System.Windows.Forms.Button();

            this.lblNoc        = new System.Windows.Forms.Label();
            this.listNoc       = new System.Windows.Forms.ListBox();
            this.btnSelectNoc  = new System.Windows.Forms.Button();
            this.btnClearNoc   = new System.Windows.Forms.Button();

            this.lblBien       = new System.Windows.Forms.Label();
            this.listBien      = new System.Windows.Forms.ListBox();
            this.btnSelectBien = new System.Windows.Forms.Button();
            this.btnClearBien  = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // ── NỀN ──────────────────────────────────────────────
            this.lblNen.AutoSize = true;
            this.lblNen.Font     = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblNen.Location = new System.Drawing.Point(12, 8);
            this.lblNen.Text     = "Nền: 0 đường";

            this.listNen.FormattingEnabled = true;
            this.listNen.Location          = new System.Drawing.Point(12, 28);
            this.listNen.Size              = new System.Drawing.Size(450, 82);

            this.btnSelectNen.Location = new System.Drawing.Point(12, 116);
            this.btnSelectNen.Size     = new System.Drawing.Size(130, 30);
            this.btnSelectNen.Text     = "Chọn đường Nền";
            this.btnSelectNen.Click   += new System.EventHandler(this.btnSelectNen_Click);

            this.btnClearNen.Location  = new System.Drawing.Point(148, 116);
            this.btnClearNen.Size      = new System.Drawing.Size(110, 30);
            this.btnClearNen.Text      = "Xóa Nền";
            this.btnClearNen.Click    += new System.EventHandler(this.btnClearNen_Click);

            // ── NÓC ──────────────────────────────────────────────
            this.lblNoc.AutoSize = true;
            this.lblNoc.Font     = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblNoc.Location = new System.Drawing.Point(12, 158);
            this.lblNoc.Text     = "Nóc: 0 đường";

            this.listNoc.FormattingEnabled = true;
            this.listNoc.Location          = new System.Drawing.Point(12, 178);
            this.listNoc.Size              = new System.Drawing.Size(450, 82);

            this.btnSelectNoc.Location = new System.Drawing.Point(12, 266);
            this.btnSelectNoc.Size     = new System.Drawing.Size(130, 30);
            this.btnSelectNoc.Text     = "Chọn đường Nóc";
            this.btnSelectNoc.Click   += new System.EventHandler(this.btnSelectNoc_Click);

            this.btnClearNoc.Location  = new System.Drawing.Point(148, 266);
            this.btnClearNoc.Size      = new System.Drawing.Size(110, 30);
            this.btnClearNoc.Text      = "Xóa Nóc";
            this.btnClearNoc.Click    += new System.EventHandler(this.btnClearNoc_Click);

            // ── BIÊN ─────────────────────────────────────────────
            this.lblBien.AutoSize = true;
            this.lblBien.Font     = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblBien.Location = new System.Drawing.Point(12, 308);
            this.lblBien.Text     = "Biên: 0 đường";

            this.listBien.FormattingEnabled = true;
            this.listBien.Location          = new System.Drawing.Point(12, 328);
            this.listBien.Size              = new System.Drawing.Size(450, 82);

            this.btnSelectBien.Location = new System.Drawing.Point(12, 416);
            this.btnSelectBien.Size     = new System.Drawing.Size(130, 30);
            this.btnSelectBien.Text     = "Chọn đường Biên";
            this.btnSelectBien.Click   += new System.EventHandler(this.btnSelectBien_Click);

            this.btnClearBien.Location  = new System.Drawing.Point(148, 416);
            this.btnClearBien.Size      = new System.Drawing.Size(110, 30);
            this.btnClearBien.Text      = "Xóa Biên";
            this.btnClearBien.Click    += new System.EventHandler(this.btnClearBien_Click);

            // ── CONTROL ───────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblNen, this.listNen, this.btnSelectNen, this.btnClearNen,
                this.lblNoc, this.listNoc, this.btnSelectNoc, this.btnClearNoc,
                this.lblBien, this.listBien, this.btnSelectBien, this.btnClearBien
            });
            this.Name = "UCMineTopology";
            this.Size = new System.Drawing.Size(490, 470);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label  lblNen;
        private System.Windows.Forms.ListBox listNen;
        private System.Windows.Forms.Button btnSelectNen;
        private System.Windows.Forms.Button btnClearNen;

        private System.Windows.Forms.Label  lblNoc;
        private System.Windows.Forms.ListBox listNoc;
        private System.Windows.Forms.Button btnSelectNoc;
        private System.Windows.Forms.Button btnClearNoc;

        private System.Windows.Forms.Label  lblBien;
        private System.Windows.Forms.ListBox listBien;
        private System.Windows.Forms.Button btnSelectBien;
        private System.Windows.Forms.Button btnClearBien;
    }
}
