namespace AutoCAD_NET_4_8_Framework
{
    partial class UCTietDienLibrary
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
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.topPanel = new System.Windows.Forms.Panel();
            this.btnExportJson = new System.Windows.Forms.Button();
            this.btnAddTietDien = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.dgvTietDien = new System.Windows.Forms.DataGridView();
            this.colTen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPolyline = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChonPolyline = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colXoa = new System.Windows.Forms.DataGridViewButtonColumn();
            this.layout.SuspendLayout();
            this.topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTietDien)).BeginInit();
            this.SuspendLayout();
            // 
            // layout
            // 
            this.layout.ColumnCount = 1;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.topPanel, 0, 0);
            this.layout.Controls.Add(this.dgvTietDien, 0, 1);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(0, 0);
            this.layout.Name = "layout";
            this.layout.RowCount = 2;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 109F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Size = new System.Drawing.Size(651, 433);
            this.layout.TabIndex = 0;
            // 
            // topPanel
            // 
            this.topPanel.BackColor = System.Drawing.SystemColors.Control;
            this.topPanel.Controls.Add(this.btnExportJson);
            this.topPanel.Controls.Add(this.btnAddTietDien);
            this.topPanel.Controls.Add(this.lblCount);
            this.topPanel.Controls.Add(this.lblTitle);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topPanel.Location = new System.Drawing.Point(3, 3);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(645, 103);
            this.topPanel.TabIndex = 0;
            // 
            // btnExportJson
            // 
            this.btnExportJson.BackColor = System.Drawing.Color.White;
            this.btnExportJson.ForeColor = System.Drawing.Color.Black;
            this.btnExportJson.Location = new System.Drawing.Point(160, 69);
            this.btnExportJson.Name = "btnExportJson";
            this.btnExportJson.Size = new System.Drawing.Size(141, 29);
            this.btnExportJson.TabIndex = 3;
            this.btnExportJson.Text = "Xuất JSON";
            this.btnExportJson.UseVisualStyleBackColor = false;
            this.btnExportJson.Click += new System.EventHandler(this.btnExportJson_Click);
            // 
            // btnAddTietDien
            // 
            this.btnAddTietDien.BackColor = System.Drawing.Color.White;
            this.btnAddTietDien.ForeColor = System.Drawing.Color.Black;
            this.btnAddTietDien.Location = new System.Drawing.Point(12, 69);
            this.btnAddTietDien.Name = "btnAddTietDien";
            this.btnAddTietDien.Size = new System.Drawing.Size(141, 29);
            this.btnAddTietDien.TabIndex = 2;
            this.btnAddTietDien.Text = "+ Thêm tiết diện";
            this.btnAddTietDien.UseVisualStyleBackColor = false;
            this.btnAddTietDien.Click += new System.EventHandler(this.btnAddTietDien_Click);
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.ForeColor = System.Drawing.Color.DimGray;
            this.lblCount.Location = new System.Drawing.Point(12, 40);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(72, 13);
            this.lblCount.TabIndex = 1;
            this.lblCount.Text = "Số tiết diện: 0";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(12, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(171, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Thư viện Tiết diện";
            // 
            // dgvTietDien
            // 
            this.dgvTietDien.AllowUserToAddRows = false;
            this.dgvTietDien.AllowUserToDeleteRows = false;
            this.dgvTietDien.AllowUserToResizeRows = false;
            this.dgvTietDien.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvTietDien.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTietDien.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTen,
            this.colPolyline,
            this.colChonPolyline,
            this.colXoa});
            this.dgvTietDien.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTietDien.Location = new System.Drawing.Point(12, 118);
            this.dgvTietDien.Margin = new System.Windows.Forms.Padding(12, 9, 12, 9);
            this.dgvTietDien.MultiSelect = false;
            this.dgvTietDien.Name = "dgvTietDien";
            this.dgvTietDien.ReadOnly = true;
            this.dgvTietDien.RowHeadersVisible = false;
            this.dgvTietDien.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTietDien.Size = new System.Drawing.Size(627, 306);
            this.dgvTietDien.TabIndex = 1;
            this.dgvTietDien.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTietDien_CellContentClick);
            // 
            // colTen
            // 
            this.colTen.HeaderText = "Tên";
            this.colTen.Name = "colTen";
            this.colTen.ReadOnly = true;
            this.colTen.Width = 160;
            // 
            // colPolyline
            // 
            this.colPolyline.HeaderText = "Polyline";
            this.colPolyline.Name = "colPolyline";
            this.colPolyline.ReadOnly = true;
            this.colPolyline.Width = 260;
            // 
            // colChonPolyline
            // 
            this.colChonPolyline.HeaderText = "";
            this.colChonPolyline.Name = "colChonPolyline";
            this.colChonPolyline.ReadOnly = true;
            this.colChonPolyline.Text = "Chọn polyline";
            this.colChonPolyline.UseColumnTextForButtonValue = true;
            this.colChonPolyline.Width = 120;
            // 
            // colXoa
            // 
            this.colXoa.HeaderText = "";
            this.colXoa.Name = "colXoa";
            this.colXoa.ReadOnly = true;
            this.colXoa.Text = "Xóa";
            this.colXoa.UseColumnTextForButtonValue = true;
            this.colXoa.Width = 60;
            // 
            // UCTietDienLibrary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layout);
            this.Name = "UCTietDienLibrary";
            this.Size = new System.Drawing.Size(651, 433);
            this.layout.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTietDien)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnAddTietDien;
        private System.Windows.Forms.Button btnExportJson;
        private System.Windows.Forms.DataGridView dgvTietDien;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPolyline;
        private System.Windows.Forms.DataGridViewButtonColumn colChonPolyline;
        private System.Windows.Forms.DataGridViewButtonColumn colXoa;
    }
}
