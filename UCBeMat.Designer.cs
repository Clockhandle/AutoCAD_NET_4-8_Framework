namespace AutoCAD_NET_4_8_Framework
{
    partial class UCBeMat
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblPointCount = new System.Windows.Forms.Label();
            this.listBoxPoints = new System.Windows.Forms.ListBox();
            this.btnSelectPoints = new System.Windows.Forms.Button();
            this.btnClearPoints = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblHeader
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblHeader.Location = new System.Drawing.Point(10, 10);
            this.lblHeader.Size = new System.Drawing.Size(200, 15);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Text = "B? m?t: ...";

            // lblPointCount
            this.lblPointCount.AutoSize = true;
            this.lblPointCount.Location = new System.Drawing.Point(10, 35);
            this.lblPointCount.Size = new System.Drawing.Size(100, 13);
            this.lblPointCount.Name = "lblPointCount";
            this.lblPointCount.Text = "S? ?i?m: 0";

            // listBoxPoints
            this.listBoxPoints.FormattingEnabled = true;
            this.listBoxPoints.Location = new System.Drawing.Point(10, 58);
            this.listBoxPoints.Name = "listBoxPoints";
            this.listBoxPoints.Size = new System.Drawing.Size(360, 186);

            // btnSelectPoints
            this.btnSelectPoints.Location = new System.Drawing.Point(10, 255);
            this.btnSelectPoints.Name = "btnSelectPoints";
            this.btnSelectPoints.Size = new System.Drawing.Size(160, 28);
            this.btnSelectPoints.Text = "Ch?n ?i?m t? AutoCAD";
            this.btnSelectPoints.Click += new System.EventHandler(this.BtnSelectPoints_Click);

            // btnClearPoints
            this.btnClearPoints.Location = new System.Drawing.Point(180, 255);
            this.btnClearPoints.Name = "btnClearPoints";
            this.btnClearPoints.Size = new System.Drawing.Size(100, 28);
            this.btnClearPoints.Text = "Xóa ?i?m";
            this.btnClearPoints.Click += new System.EventHandler(this.BtnClearPoints_Click);

            // btnDelete
            this.btnDelete.Location = new System.Drawing.Point(10, 293);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(160, 28);
            this.btnDelete.Text = "Xóa B? m?t này";
            this.btnDelete.BackColor = System.Drawing.Color.MistyRose;
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);

            // UCBeMat
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.lblPointCount);
            this.Controls.Add(this.listBoxPoints);
            this.Controls.Add(this.btnSelectPoints);
            this.Controls.Add(this.btnClearPoints);
            this.Controls.Add(this.btnDelete);
            this.Name = "UCBeMat";
            this.Size = new System.Drawing.Size(390, 340);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblPointCount;
        private System.Windows.Forms.ListBox listBoxPoints;
        private System.Windows.Forms.Button btnSelectPoints;
        private System.Windows.Forms.Button btnClearPoints;
        private System.Windows.Forms.Button btnDelete;
    }
}
