namespace AutoCAD_NET_4_8_Framework
{
    partial class UCSurface
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
            this.btnDeleteLine = new System.Windows.Forms.Button();
            this.btnAddLine = new System.Windows.Forms.Button();
            this.lblLineData = new System.Windows.Forms.Label();
            this.listBoxIds = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnDeleteLine
            // 
            this.btnDeleteLine.Location = new System.Drawing.Point(44, 237);
            this.btnDeleteLine.Name = "btnDeleteLine";
            this.btnDeleteLine.Size = new System.Drawing.Size(116, 36);
            this.btnDeleteLine.TabIndex = 8;
            this.btnDeleteLine.Text = "Xóa dữ liệu";
            this.btnDeleteLine.UseVisualStyleBackColor = true;
            this.btnDeleteLine.Click += new System.EventHandler(this.BtnClearLines_Click);
            // 
            // btnAddLine
            // 
            this.btnAddLine.Location = new System.Drawing.Point(44, 197);
            this.btnAddLine.Name = "btnAddLine";
            this.btnAddLine.Size = new System.Drawing.Size(116, 34);
            this.btnAddLine.TabIndex = 7;
            this.btnAddLine.Text = "Chọn thêm đường";
            this.btnAddLine.UseVisualStyleBackColor = true;
            this.btnAddLine.Click += new System.EventHandler(this.BtnSelectLines_Click);
            // 
            // lblLineData
            // 
            this.lblLineData.AutoSize = true;
            this.lblLineData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLineData.Location = new System.Drawing.Point(41, 179);
            this.lblLineData.Name = "lblLineData";
            this.lblLineData.Size = new System.Drawing.Size(66, 15);
            this.lblLineData.TabIndex = 6;
            this.lblLineData.Text = "Số đường: ";
            // 
            // listBoxIds
            // 
            this.listBoxIds.FormattingEnabled = true;
            this.listBoxIds.Location = new System.Drawing.Point(44, 29);
            this.listBoxIds.Name = "listBoxIds";
            this.listBoxIds.Size = new System.Drawing.Size(423, 147);
            this.listBoxIds.TabIndex = 12;
            // 
            // UCSurface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listBoxIds);
            this.Controls.Add(this.btnDeleteLine);
            this.Controls.Add(this.btnAddLine);
            this.Controls.Add(this.lblLineData);
            this.Name = "UCSurface";
            this.Size = new System.Drawing.Size(616, 461);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnDeleteLine;
        private System.Windows.Forms.Button btnAddLine;
        private System.Windows.Forms.Label lblLineData;
        private System.Windows.Forms.ListBox listBoxIds;
    }
}
