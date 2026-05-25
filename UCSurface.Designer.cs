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
            this.btnDeleteSurfaceLines = new System.Windows.Forms.Button();
            this.btnAddLine = new System.Windows.Forms.Button();
            this.lblLineData = new System.Windows.Forms.Label();
            this.listBoxIds = new System.Windows.Forms.ListBox();
            this.btnAddBoundaryLine = new System.Windows.Forms.Button();
            this.listBoundaryLineId = new System.Windows.Forms.ListBox();
            this.btnDeleteBoundaryLines = new System.Windows.Forms.Button();
            this.btnDeleteGapingBound = new System.Windows.Forms.Button();
            this.listGapingBoundId = new System.Windows.Forms.ListBox();
            this.btnAddGapingBound = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDeleteSurfaceLines
            // 
            this.btnDeleteSurfaceLines.Location = new System.Drawing.Point(166, 197);
            this.btnDeleteSurfaceLines.Name = "btnDeleteSurfaceLines";
            this.btnDeleteSurfaceLines.Size = new System.Drawing.Size(116, 36);
            this.btnDeleteSurfaceLines.TabIndex = 8;
            this.btnDeleteSurfaceLines.Text = "Xóa đường vách";
            this.btnDeleteSurfaceLines.UseVisualStyleBackColor = true;
            this.btnDeleteSurfaceLines.Click += new System.EventHandler(this.btnDeleteSurfaceLines_Click);
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
            // btnAddBoundaryLine
            // 
            this.btnAddBoundaryLine.Location = new System.Drawing.Point(44, 295);
            this.btnAddBoundaryLine.Name = "btnAddBoundaryLine";
            this.btnAddBoundaryLine.Size = new System.Drawing.Size(116, 34);
            this.btnAddBoundaryLine.TabIndex = 13;
            this.btnAddBoundaryLine.Text = "Chọn đường bao";
            this.btnAddBoundaryLine.UseVisualStyleBackColor = true;
            this.btnAddBoundaryLine.Click += new System.EventHandler(this.BtnAddBoundaryLine_Click);
            // 
            // listBoundaryLineId
            // 
            this.listBoundaryLineId.FormattingEnabled = true;
            this.listBoundaryLineId.Location = new System.Drawing.Point(44, 246);
            this.listBoundaryLineId.Name = "listBoundaryLineId";
            this.listBoundaryLineId.Size = new System.Drawing.Size(423, 43);
            this.listBoundaryLineId.TabIndex = 14;
            // 
            // btnDeleteBoundaryLines
            // 
            this.btnDeleteBoundaryLines.Location = new System.Drawing.Point(166, 295);
            this.btnDeleteBoundaryLines.Name = "btnDeleteBoundaryLines";
            this.btnDeleteBoundaryLines.Size = new System.Drawing.Size(116, 36);
            this.btnDeleteBoundaryLines.TabIndex = 15;
            this.btnDeleteBoundaryLines.Text = "Xóa đường bao";
            this.btnDeleteBoundaryLines.UseVisualStyleBackColor = true;
            this.btnDeleteBoundaryLines.Click += new System.EventHandler(this.btnDeleteBoundaryLines_Click);
            // 
            // btnDeleteGapingBound
            // 
            this.btnDeleteGapingBound.Location = new System.Drawing.Point(166, 384);
            this.btnDeleteGapingBound.Name = "btnDeleteGapingBound";
            this.btnDeleteGapingBound.Size = new System.Drawing.Size(116, 36);
            this.btnDeleteGapingBound.TabIndex = 18;
            this.btnDeleteGapingBound.Text = "Xóa đường hố";
            this.btnDeleteGapingBound.UseVisualStyleBackColor = true;
            // 
            // listGapingBoundId
            // 
            this.listGapingBoundId.FormattingEnabled = true;
            this.listGapingBoundId.Location = new System.Drawing.Point(44, 335);
            this.listGapingBoundId.Name = "listGapingBoundId";
            this.listGapingBoundId.Size = new System.Drawing.Size(423, 43);
            this.listGapingBoundId.TabIndex = 17;
            // 
            // btnAddGapingBound
            // 
            this.btnAddGapingBound.Location = new System.Drawing.Point(44, 384);
            this.btnAddGapingBound.Name = "btnAddGapingBound";
            this.btnAddGapingBound.Size = new System.Drawing.Size(116, 34);
            this.btnAddGapingBound.TabIndex = 16;
            this.btnAddGapingBound.Text = "Chọn đường hố";
            this.btnAddGapingBound.UseVisualStyleBackColor = true;
            // 
            // UCSurface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnDeleteGapingBound);
            this.Controls.Add(this.listGapingBoundId);
            this.Controls.Add(this.btnAddGapingBound);
            this.Controls.Add(this.btnDeleteBoundaryLines);
            this.Controls.Add(this.listBoundaryLineId);
            this.Controls.Add(this.btnAddBoundaryLine);
            this.Controls.Add(this.listBoxIds);
            this.Controls.Add(this.btnDeleteSurfaceLines);
            this.Controls.Add(this.btnAddLine);
            this.Controls.Add(this.lblLineData);
            this.Name = "UCSurface";
            this.Size = new System.Drawing.Size(616, 461);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnDeleteSurfaceLines;
        private System.Windows.Forms.Button btnAddLine;
        private System.Windows.Forms.Label lblLineData;
        private System.Windows.Forms.ListBox listBoxIds;
        private System.Windows.Forms.Button btnAddBoundaryLine;
        private System.Windows.Forms.ListBox listBoundaryLineId;
        private System.Windows.Forms.Button btnDeleteBoundaryLines;
        private System.Windows.Forms.Button btnDeleteGapingBound;
        private System.Windows.Forms.ListBox listGapingBoundId;
        private System.Windows.Forms.Button btnAddGapingBound;
    }
}
