namespace AutoCAD_NET_4_8_Framework
{
    partial class UCMineTopologyLoai2
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
            this.btnAddDoan = new System.Windows.Forms.Button();
            this.lblDoanSection = new System.Windows.Forms.Label();
            this.btnLoadTietDienJson = new System.Windows.Forms.Button();
            this.lblTietDienNote = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.dgvDoan = new System.Windows.Forms.DataGridView();
            this.colTen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPolyline = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChonDuong = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colTietDien = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colNgay = new AutoCAD_NET_4_8_Framework.UCMineTopologyLoai2.DataGridViewCalendarColumn();
            this.colXoa = new System.Windows.Forms.DataGridViewButtonColumn();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.btnDeleteTopo = new System.Windows.Forms.Button();
            this.layout.SuspendLayout();
            this.topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDoan)).BeginInit();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // layout
            // 
            this.layout.ColumnCount = 1;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.topPanel, 0, 0);
            this.layout.Controls.Add(this.dgvDoan, 0, 1);
            this.layout.Controls.Add(this.bottomPanel, 0, 2);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(0, 0);
            this.layout.Name = "layout";
            this.layout.RowCount = 3;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 168F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.layout.Size = new System.Drawing.Size(771, 537);
            this.layout.TabIndex = 0;
            // 
            // topPanel
            // 
            this.topPanel.BackColor = System.Drawing.SystemColors.Control;
            this.topPanel.Controls.Add(this.btnAddDoan);
            this.topPanel.Controls.Add(this.lblDoanSection);
            this.topPanel.Controls.Add(this.btnLoadTietDienJson);
            this.topPanel.Controls.Add(this.lblTietDienNote);
            this.topPanel.Controls.Add(this.lblTitle);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topPanel.Location = new System.Drawing.Point(3, 3);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(765, 162);
            this.topPanel.TabIndex = 0;
            // 
            // btnAddDoan
            // 
            this.btnAddDoan.BackColor = System.Drawing.Color.White;
            this.btnAddDoan.ForeColor = System.Drawing.Color.Black;
            this.btnAddDoan.Location = new System.Drawing.Point(12, 128);
            this.btnAddDoan.Name = "btnAddDoan";
            this.btnAddDoan.Size = new System.Drawing.Size(163, 29);
            this.btnAddDoan.TabIndex = 4;
            this.btnAddDoan.Text = "+ Thêm đoạn đường lò";
            this.btnAddDoan.UseVisualStyleBackColor = false;
            this.btnAddDoan.Click += new System.EventHandler(this.btnAddDoan_Click);
            // 
            // lblDoanSection
            // 
            this.lblDoanSection.AutoSize = true;
            this.lblDoanSection.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblDoanSection.ForeColor = System.Drawing.Color.DimGray;
            this.lblDoanSection.Location = new System.Drawing.Point(12, 106);
            this.lblDoanSection.Name = "lblDoanSection";
            this.lblDoanSection.Size = new System.Drawing.Size(124, 15);
            this.lblDoanSection.TabIndex = 3;
            this.lblDoanSection.Text = "── Đoạn đường lò ──";
            // 
            // btnLoadTietDienJson
            // 
            this.btnLoadTietDienJson.BackColor = System.Drawing.Color.White;
            this.btnLoadTietDienJson.ForeColor = System.Drawing.Color.Black;
            this.btnLoadTietDienJson.Location = new System.Drawing.Point(12, 69);
            this.btnLoadTietDienJson.Name = "btnLoadTietDienJson";
            this.btnLoadTietDienJson.Size = new System.Drawing.Size(197, 28);
            this.btnLoadTietDienJson.TabIndex = 2;
            this.btnLoadTietDienJson.Text = "Tải thư viện Tiết diện (.t3d)";
            this.btnLoadTietDienJson.UseVisualStyleBackColor = false;
            this.btnLoadTietDienJson.Click += new System.EventHandler(this.btnLoadTietDienJson_Click);
            // 
            // lblTietDienNote
            // 
            this.lblTietDienNote.AutoSize = true;
            this.lblTietDienNote.Font = new System.Drawing.Font("Segoe UI", 8.75F, System.Drawing.FontStyle.Italic);
            this.lblTietDienNote.ForeColor = System.Drawing.Color.DimGray;
            this.lblTietDienNote.Location = new System.Drawing.Point(12, 49);
            this.lblTietDienNote.Name = "lblTietDienNote";
            this.lblTietDienNote.Size = new System.Drawing.Size(322, 15);
            this.lblTietDienNote.TabIndex = 1;
            this.lblTietDienNote.Text = "Tiết diện được quản lý trong mục \"Tiết diện\" của Địa hình lò.";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(12, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(170, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Địa hình lò Loại 2:";
            // 
            // dgvDoan
            // 
            this.dgvDoan.AllowUserToAddRows = false;
            this.dgvDoan.AllowUserToDeleteRows = false;
            this.dgvDoan.AllowUserToResizeRows = false;
            this.dgvDoan.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDoan.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvDoan.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDoan.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTen,
            this.colPolyline,
            this.colChonDuong,
            this.colTietDien,
            this.colNgay,
            this.colXoa});
            this.dgvDoan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDoan.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvDoan.Location = new System.Drawing.Point(12, 177);
            this.dgvDoan.Margin = new System.Windows.Forms.Padding(12, 9, 12, 9);
            this.dgvDoan.MultiSelect = false;
            this.dgvDoan.Name = "dgvDoan";
            this.dgvDoan.RowHeadersVisible = false;
            this.dgvDoan.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvDoan.Size = new System.Drawing.Size(747, 306);
            this.dgvDoan.TabIndex = 1;
            this.dgvDoan.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDoan_CellContentClick);
            this.dgvDoan.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDoan_CellEndEdit);
            this.dgvDoan.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDoan_CellValueChanged);
            this.dgvDoan.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvDoan_CurrentCellDirtyStateChanged);
            this.dgvDoan.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvDoan_DataError);
            this.dgvDoan.Leave += new System.EventHandler(this.dgvDoan_Leave);
            // 
            // colTen
            // 
            this.colTen.FillWeight = 120F;
            this.colTen.HeaderText = "Tên";
            this.colTen.MinimumWidth = 70;
            this.colTen.Name = "colTen";
            // 
            // colPolyline
            // 
            this.colPolyline.FillWeight = 210F;
            this.colPolyline.HeaderText = "Polyline";
            this.colPolyline.MinimumWidth = 120;
            this.colPolyline.Name = "colPolyline";
            this.colPolyline.ReadOnly = true;
            // 
            // colChonDuong
            // 
            this.colChonDuong.HeaderText = "";
            this.colChonDuong.MinimumWidth = 90;
            this.colChonDuong.Name = "colChonDuong";
            this.colChonDuong.Text = "Chọn đường";
            this.colChonDuong.UseColumnTextForButtonValue = true;
            // 
            // colTietDien
            // 
            this.colTietDien.FillWeight = 160F;
            this.colTietDien.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colTietDien.HeaderText = "Tiết diện";
            this.colTietDien.MinimumWidth = 100;
            this.colTietDien.Name = "colTietDien";
            // 
            // colNgay
            // 
            this.colNgay.FillWeight = 170F;
            this.colNgay.HeaderText = "Ngày khai thác";
            this.colNgay.MinimumWidth = 110;
            this.colNgay.Name = "colNgay";
            // 
            // colXoa
            // 
            this.colXoa.FillWeight = 60F;
            this.colXoa.HeaderText = "";
            this.colXoa.MinimumWidth = 50;
            this.colXoa.Name = "colXoa";
            this.colXoa.Text = "Xóa";
            this.colXoa.UseColumnTextForButtonValue = true;
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.SystemColors.Control;
            this.bottomPanel.Controls.Add(this.btnDeleteTopo);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomPanel.Location = new System.Drawing.Point(3, 495);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(765, 39);
            this.bottomPanel.TabIndex = 2;
            // 
            // btnDeleteTopo
            // 
            this.btnDeleteTopo.BackColor = System.Drawing.Color.White;
            this.btnDeleteTopo.ForeColor = System.Drawing.Color.DarkRed;
            this.btnDeleteTopo.Location = new System.Drawing.Point(12, 9);
            this.btnDeleteTopo.Name = "btnDeleteTopo";
            this.btnDeleteTopo.Size = new System.Drawing.Size(150, 29);
            this.btnDeleteTopo.TabIndex = 0;
            this.btnDeleteTopo.Text = "Xóa địa hình lò này";
            this.btnDeleteTopo.UseVisualStyleBackColor = false;
            this.btnDeleteTopo.Click += new System.EventHandler(this.btnDeleteTopo_Click);
            // 
            // UCMineTopologyLoai2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layout);
            this.Name = "UCMineTopologyLoai2";
            this.Size = new System.Drawing.Size(771, 537);
            this.layout.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDoan)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblTietDienNote;
        private System.Windows.Forms.Button btnLoadTietDienJson;
        private System.Windows.Forms.Label lblDoanSection;
        private System.Windows.Forms.Button btnAddDoan;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button btnDeleteTopo;
        private System.Windows.Forms.DataGridView dgvDoan;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPolyline;
        private System.Windows.Forms.DataGridViewButtonColumn colChonDuong;
        private System.Windows.Forms.DataGridViewComboBoxColumn colTietDien;
        private DataGridViewCalendarColumn colNgay;
        private System.Windows.Forms.DataGridViewButtonColumn colXoa;
    }
}
