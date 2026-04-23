namespace AutoCAD_NET_4_8_Framework
{
    partial class UCBoreholes
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
            this.components = new System.ComponentModel.Container();
            this.btnDeleteBorehole = new System.Windows.Forms.Button();
            this.btnReadBoreholeData = new System.Windows.Forms.Button();
            this.lblHeader = new System.Windows.Forms.Label();
            this.txtBoxExcelPath = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnReadExcel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDeleteBorehole
            // 
            this.btnDeleteBorehole.Location = new System.Drawing.Point(29, 127);
            this.btnDeleteBorehole.Name = "btnDeleteBorehole";
            this.btnDeleteBorehole.Size = new System.Drawing.Size(116, 36);
            this.btnDeleteBorehole.TabIndex = 8;
            this.btnDeleteBorehole.Text = "Xóa lỗ khoan";
            this.btnDeleteBorehole.UseVisualStyleBackColor = true;
            this.btnDeleteBorehole.Click += new System.EventHandler(this.BtnDeleteBorehole_Click);
            // 
            // btnReadBoreholeData
            // 
            this.btnReadBoreholeData.Location = new System.Drawing.Point(29, 87);
            this.btnReadBoreholeData.Name = "btnReadBoreholeData";
            this.btnReadBoreholeData.Size = new System.Drawing.Size(116, 34);
            this.btnReadBoreholeData.TabIndex = 7;
            this.btnReadBoreholeData.Text = "Đọc dữ liệu";
            this.btnReadBoreholeData.UseVisualStyleBackColor = true;
            this.btnReadBoreholeData.Click += new System.EventHandler(this.BtnParseExcel_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(24, 24);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(125, 25);
            this.lblHeader.TabIndex = 6;
            this.lblHeader.Text = "Quản lý Vỉa";
            // 
            // txtBoxExcelPath
            // 
            this.txtBoxExcelPath.Location = new System.Drawing.Point(29, 61);
            this.txtBoxExcelPath.Name = "txtBoxExcelPath";
            this.txtBoxExcelPath.Size = new System.Drawing.Size(257, 20);
            this.txtBoxExcelPath.TabIndex = 9;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // btnReadExcel
            // 
            this.btnReadExcel.Location = new System.Drawing.Point(292, 61);
            this.btnReadExcel.Name = "btnReadExcel";
            this.btnReadExcel.Size = new System.Drawing.Size(103, 23);
            this.btnReadExcel.TabIndex = 11;
            this.btnReadExcel.Text = "Đọc file excel";
            this.btnReadExcel.UseVisualStyleBackColor = true;
            this.btnReadExcel.Click += new System.EventHandler(this.BtnSelectExcel_Click);
            // 
            // UCBoreholes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnReadExcel);
            this.Controls.Add(this.txtBoxExcelPath);
            this.Controls.Add(this.btnDeleteBorehole);
            this.Controls.Add(this.btnReadBoreholeData);
            this.Controls.Add(this.lblHeader);
            this.Name = "UCBoreholes";
            this.Size = new System.Drawing.Size(492, 350);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnDeleteBorehole;
        private System.Windows.Forms.Button btnReadBoreholeData;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.TextBox txtBoxExcelPath;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button btnReadExcel;
    }
}
