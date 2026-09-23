namespace MyMiningPlugin.UI
{
    partial class SendMultipleDataDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblSelect = new System.Windows.Forms.Label();
            this.chkItems = new System.Windows.Forms.CheckedListBox();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnDeselectAll = new System.Windows.Forms.Button();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.lblMapLabel = new System.Windows.Forms.Label();
            this.cmbMapName = new System.Windows.Forms.ComboBox();
            this.btnRefreshMaps = new System.Windows.Forms.Button();
            this.lblMapStatus = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblSelect
            //
            this.lblSelect.AutoSize = true;
            this.lblSelect.Location = new System.Drawing.Point(20, 15);
            this.lblSelect.Name = "lblSelect";
            this.lblSelect.Size = new System.Drawing.Size(280, 15);
            this.lblSelect.TabIndex = 0;
            this.lblSelect.Text = "Chọn mục để gửi (có thể chọn nhiều):";
            //
            // chkItems
            //
            this.chkItems.CheckOnClick = true;
            this.chkItems.Location = new System.Drawing.Point(20, 38);
            this.chkItems.Name = "chkItems";
            this.chkItems.Size = new System.Drawing.Size(470, 180);
            this.chkItems.TabIndex = 1;
            //
            // btnSelectAll
            //
            this.btnSelectAll.Location = new System.Drawing.Point(20, 228);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(100, 25);
            this.btnSelectAll.TabIndex = 2;
            this.btnSelectAll.Text = "Chọn tất cả";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            //
            // btnDeselectAll
            //
            this.btnDeselectAll.Location = new System.Drawing.Point(130, 228);
            this.btnDeselectAll.Name = "btnDeselectAll";
            this.btnDeselectAll.Size = new System.Drawing.Size(110, 25);
            this.btnDeselectAll.TabIndex = 3;
            this.btnDeselectAll.Text = "Bỏ chọn tất cả";
            this.btnDeselectAll.UseVisualStyleBackColor = true;
            this.btnDeselectAll.Click += new System.EventHandler(this.btnDeselectAll_Click);
            //
            // lblDate
            //
            // Only shown for the "Địa hình lò" category — see ConfigureFor. The rest of
            // the layout below reserves this row's space permanently so nothing else
            // needs to shift when it's hidden.
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(20, 263);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(85, 15);
            this.lblDate.TabIndex = 4;
            this.lblDate.Text = "Ngày dữ liệu:";
            //
            // dtpDate
            //
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(20, 283);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(200, 23);
            this.dtpDate.TabIndex = 5;
            //
            // lblMapLabel
            //
            this.lblMapLabel.AutoSize = true;
            this.lblMapLabel.Location = new System.Drawing.Point(20, 323);
            this.lblMapLabel.Name = "lblMapLabel";
            this.lblMapLabel.Size = new System.Drawing.Size(320, 15);
            this.lblMapLabel.TabIndex = 6;
            this.lblMapLabel.Text = "Chọn bản đồ trên server (hoặc nhập tên mới):";
            //
            // cmbMapName
            //
            this.cmbMapName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbMapName.Location = new System.Drawing.Point(20, 343);
            this.cmbMapName.Name = "cmbMapName";
            this.cmbMapName.Size = new System.Drawing.Size(350, 23);
            this.cmbMapName.TabIndex = 7;
            //
            // btnRefreshMaps
            //
            this.btnRefreshMaps.Location = new System.Drawing.Point(378, 342);
            this.btnRefreshMaps.Name = "btnRefreshMaps";
            this.btnRefreshMaps.Size = new System.Drawing.Size(112, 26);
            this.btnRefreshMaps.TabIndex = 8;
            this.btnRefreshMaps.Text = "↻ Tải danh sách";
            this.btnRefreshMaps.UseVisualStyleBackColor = true;
            this.btnRefreshMaps.Click += new System.EventHandler(this.btnRefreshMaps_Click);
            //
            // lblMapStatus
            //
            this.lblMapStatus.AutoSize = true;
            this.lblMapStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblMapStatus.Location = new System.Drawing.Point(20, 376);
            this.lblMapStatus.Name = "lblMapStatus";
            this.lblMapStatus.Size = new System.Drawing.Size(280, 15);
            this.lblMapStatus.TabIndex = 9;
            this.lblMapStatus.Text = "Nhấn ↻ để tải danh sách bản đồ từ server.";
            //
            // btnOK
            //
            this.btnOK.BackColor = System.Drawing.Color.LightGreen;
            this.btnOK.Location = new System.Drawing.Point(300, 451);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 30);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "Gửi (Send)";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(400, 451);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // SendMultipleDataDialog
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 528);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblMapStatus);
            this.Controls.Add(this.btnRefreshMaps);
            this.Controls.Add(this.cmbMapName);
            this.Controls.Add(this.lblMapLabel);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.btnDeselectAll);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.chkItems);
            this.Controls.Add(this.lblSelect);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SendMultipleDataDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSelect;
        private System.Windows.Forms.CheckedListBox chkItems;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnDeselectAll;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label lblMapLabel;
        private System.Windows.Forms.ComboBox cmbMapName;
        private System.Windows.Forms.Button btnRefreshMaps;
        private System.Windows.Forms.Label lblMapStatus;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
