namespace AutoCAD_NET_4_8_Framework
{
    partial class UCSaveManager
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
            this.btnMassSend = new System.Windows.Forms.Button();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnSaveNew = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();
            this.btnBrowseFolder = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.lblViTriLuu = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.dgvSaves = new System.Windows.Forms.DataGridView();
            this.colTen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNgay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTai = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colGhiDe = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colXoa = new System.Windows.Forms.DataGridViewButtonColumn();
            this.layout.SuspendLayout();
            this.topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSaves)).BeginInit();
            this.SuspendLayout();
            // 
            // layout
            // 
            this.layout.ColumnCount = 1;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.topPanel, 0, 0);
            this.layout.Controls.Add(this.dgvSaves, 0, 1);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(0, 0);
            this.layout.Name = "layout";
            this.layout.RowCount = 2;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 173F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Size = new System.Drawing.Size(703, 485);
            this.layout.TabIndex = 0;
            // 
            // topPanel
            // 
            this.topPanel.BackColor = System.Drawing.SystemColors.Control;
            this.topPanel.Controls.Add(this.btnMassSend);
            this.topPanel.Controls.Add(this.btnOpenFolder);
            this.topPanel.Controls.Add(this.btnBrowse);
            this.topPanel.Controls.Add(this.btnSaveNew);
            this.topPanel.Controls.Add(this.lblCount);
            this.topPanel.Controls.Add(this.btnBrowseFolder);
            this.topPanel.Controls.Add(this.txtFolder);
            this.topPanel.Controls.Add(this.lblViTriLuu);
            this.topPanel.Controls.Add(this.lblTitle);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topPanel.Location = new System.Drawing.Point(3, 3);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(697, 167);
            this.topPanel.TabIndex = 0;
            // 
            // btnMassSend
            // 
            this.btnMassSend.BackColor = System.Drawing.Color.White;
            this.btnMassSend.ForeColor = System.Drawing.Color.Black;
            this.btnMassSend.Location = new System.Drawing.Point(12, 133);
            this.btnMassSend.Name = "btnMassSend";
            this.btnMassSend.Size = new System.Drawing.Size(223, 29);
            this.btnMassSend.TabIndex = 8;
            this.btnMassSend.Text = "⇪ Gửi dữ liệu mới lên server";
            this.btnMassSend.UseVisualStyleBackColor = false;
            this.btnMassSend.Click += new System.EventHandler(this.btnMassSend_Click);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnOpenFolder.ForeColor = System.Drawing.Color.Black;
            this.btnOpenFolder.Location = new System.Drawing.Point(309, 97);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(129, 29);
            this.btnOpenFolder.TabIndex = 7;
            this.btnOpenFolder.Text = "Mở thư mục lưu";
            this.btnOpenFolder.UseVisualStyleBackColor = false;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.BackColor = System.Drawing.Color.White;
            this.btnBrowse.ForeColor = System.Drawing.Color.Black;
            this.btnBrowse.Location = new System.Drawing.Point(160, 97);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(141, 29);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "Tải từ file khác...";
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnSaveNew
            // 
            this.btnSaveNew.BackColor = System.Drawing.Color.White;
            this.btnSaveNew.ForeColor = System.Drawing.Color.Black;
            this.btnSaveNew.Location = new System.Drawing.Point(12, 97);
            this.btnSaveNew.Name = "btnSaveNew";
            this.btnSaveNew.Size = new System.Drawing.Size(141, 29);
            this.btnSaveNew.TabIndex = 5;
            this.btnSaveNew.Text = "+ Lưu dự án mới";
            this.btnSaveNew.UseVisualStyleBackColor = false;
            this.btnSaveNew.Click += new System.EventHandler(this.btnSaveNew_Click);
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.ForeColor = System.Drawing.Color.DimGray;
            this.lblCount.Location = new System.Drawing.Point(12, 71);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(70, 13);
            this.lblCount.TabIndex = 4;
            this.lblCount.Text = "Số bản lưu: 0";
            // 
            // btnBrowseFolder
            // 
            this.btnBrowseFolder.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnBrowseFolder.ForeColor = System.Drawing.Color.Black;
            this.btnBrowseFolder.Location = new System.Drawing.Point(387, 42);
            this.btnBrowseFolder.Name = "btnBrowseFolder";
            this.btnBrowseFolder.Size = new System.Drawing.Size(29, 21);
            this.btnBrowseFolder.TabIndex = 3;
            this.btnBrowseFolder.Text = "...";
            this.btnBrowseFolder.UseVisualStyleBackColor = false;
            this.btnBrowseFolder.Click += new System.EventHandler(this.btnBrowseFolder_Click);
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(74, 43);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.ReadOnly = true;
            this.txtFolder.Size = new System.Drawing.Size(309, 20);
            this.txtFolder.TabIndex = 2;
            // 
            // lblViTriLuu
            // 
            this.lblViTriLuu.AutoSize = true;
            this.lblViTriLuu.ForeColor = System.Drawing.Color.DimGray;
            this.lblViTriLuu.Location = new System.Drawing.Point(12, 46);
            this.lblViTriLuu.Name = "lblViTriLuu";
            this.lblViTriLuu.Size = new System.Drawing.Size(49, 13);
            this.lblViTriLuu.TabIndex = 1;
            this.lblViTriLuu.Text = "Vị trí lưu:";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(12, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(204, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Quản lý dữ liệu dự án";
            // 
            // dgvSaves
            // 
            this.dgvSaves.AllowUserToAddRows = false;
            this.dgvSaves.AllowUserToDeleteRows = false;
            this.dgvSaves.AllowUserToResizeRows = false;
            this.dgvSaves.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvSaves.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSaves.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTen,
            this.colNgay,
            this.colTai,
            this.colGhiDe,
            this.colXoa});
            this.dgvSaves.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSaves.Location = new System.Drawing.Point(12, 182);
            this.dgvSaves.Margin = new System.Windows.Forms.Padding(12, 9, 12, 9);
            this.dgvSaves.MultiSelect = false;
            this.dgvSaves.Name = "dgvSaves";
            this.dgvSaves.ReadOnly = true;
            this.dgvSaves.RowHeadersVisible = false;
            this.dgvSaves.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSaves.Size = new System.Drawing.Size(679, 294);
            this.dgvSaves.TabIndex = 1;
            this.dgvSaves.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSaves_CellContentClick);
            // 
            // colTen
            // 
            this.colTen.HeaderText = "Tên bản lưu";
            this.colTen.Name = "colTen";
            this.colTen.ReadOnly = true;
            this.colTen.Width = 220;
            // 
            // colNgay
            // 
            this.colNgay.HeaderText = "Ngày lưu";
            this.colNgay.Name = "colNgay";
            this.colNgay.ReadOnly = true;
            this.colNgay.Width = 150;
            // 
            // colTai
            // 
            this.colTai.HeaderText = "";
            this.colTai.Name = "colTai";
            this.colTai.ReadOnly = true;
            this.colTai.Text = "Tải";
            this.colTai.UseColumnTextForButtonValue = true;
            this.colTai.Width = 70;
            // 
            // colGhiDe
            // 
            this.colGhiDe.HeaderText = "";
            this.colGhiDe.Name = "colGhiDe";
            this.colGhiDe.ReadOnly = true;
            this.colGhiDe.Text = "Ghi đè";
            this.colGhiDe.UseColumnTextForButtonValue = true;
            this.colGhiDe.Width = 90;
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
            // UCSaveManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layout);
            this.Name = "UCSaveManager";
            this.Size = new System.Drawing.Size(703, 485);
            this.layout.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSaves)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblViTriLuu;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button btnBrowseFolder;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnSaveNew;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.Button btnMassSend;
        private System.Windows.Forms.DataGridView dgvSaves;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNgay;
        private System.Windows.Forms.DataGridViewButtonColumn colTai;
        private System.Windows.Forms.DataGridViewButtonColumn colGhiDe;
        private System.Windows.Forms.DataGridViewButtonColumn colXoa;
    }
}
