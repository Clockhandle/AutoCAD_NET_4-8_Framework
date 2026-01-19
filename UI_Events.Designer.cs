namespace AutoCAD_NET_4_8_Framework
{
    partial class UI_Events
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
            this.OnClose = new System.Windows.Forms.Button();
            this.OnExportToCSV = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.Seam = new System.Windows.Forms.GroupBox();
            this.SelectedObjectsLabel = new System.Windows.Forms.Label();
            this.OnStoreSeams = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.OnAddSeams = new System.Windows.Forms.Button();
            this.ListOfSeams = new System.Windows.Forms.ComboBox();
            this.Seam.SuspendLayout();
            this.SuspendLayout();
            // 
            // OnClose
            // 
            this.OnClose.Location = new System.Drawing.Point(636, 387);
            this.OnClose.Name = "OnClose";
            this.OnClose.Size = new System.Drawing.Size(187, 45);
            this.OnClose.TabIndex = 1;
            this.OnClose.Text = "Close";
            this.OnClose.UseVisualStyleBackColor = true;
            this.OnClose.Click += new System.EventHandler(this.OnClose_Click);
            // 
            // OnExportToCSV
            // 
            this.OnExportToCSV.Location = new System.Drawing.Point(636, 284);
            this.OnExportToCSV.Name = "OnExportToCSV";
            this.OnExportToCSV.Size = new System.Drawing.Size(186, 45);
            this.OnExportToCSV.TabIndex = 2;
            this.OnExportToCSV.Text = "ExportToJSON";
            this.OnExportToCSV.UseVisualStyleBackColor = true;
            this.OnExportToCSV.Click += new System.EventHandler(this.OnExportToJSON_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(636, 174);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(187, 45);
            this.button3.TabIndex = 5;
            this.button3.Text = "SendToServer";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.OnSendToServer_Click);
            // 
            // Seam
            // 
            this.Seam.Controls.Add(this.SelectedObjectsLabel);
            this.Seam.Controls.Add(this.OnStoreSeams);
            this.Seam.Controls.Add(this.label1);
            this.Seam.Controls.Add(this.OnAddSeams);
            this.Seam.Controls.Add(this.ListOfSeams);
            this.Seam.Location = new System.Drawing.Point(12, 12);
            this.Seam.Name = "Seam";
            this.Seam.Size = new System.Drawing.Size(247, 462);
            this.Seam.TabIndex = 6;
            this.Seam.TabStop = false;
            this.Seam.Text = "Vía";
            // 
            // SelectedObjectsLabel
            // 
            this.SelectedObjectsLabel.AutoSize = true;
            this.SelectedObjectsLabel.Location = new System.Drawing.Point(14, 143);
            this.SelectedObjectsLabel.Name = "SelectedObjectsLabel";
            this.SelectedObjectsLabel.Size = new System.Drawing.Size(107, 13);
            this.SelectedObjectsLabel.TabIndex = 6;
            this.SelectedObjectsLabel.Text = "Số vía trong mảng: 0";
            // 
            // OnStoreSeams
            // 
            this.OnStoreSeams.Location = new System.Drawing.Point(144, 84);
            this.OnStoreSeams.Name = "OnStoreSeams";
            this.OnStoreSeams.Size = new System.Drawing.Size(75, 23);
            this.OnStoreSeams.TabIndex = 5;
            this.OnStoreSeams.Text = "Lưu";
            this.OnStoreSeams.UseVisualStyleBackColor = true;
            this.OnStoreSeams.Click += new System.EventHandler(this.OnStoreSeams_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Danh sách vía:";
            // 
            // OnAddSeams
            // 
            this.OnAddSeams.Location = new System.Drawing.Point(144, 55);
            this.OnAddSeams.Name = "OnAddSeams";
            this.OnAddSeams.Size = new System.Drawing.Size(75, 23);
            this.OnAddSeams.TabIndex = 2;
            this.OnAddSeams.Text = "Thêm vía";
            this.OnAddSeams.UseVisualStyleBackColor = true;
            this.OnAddSeams.Click += new System.EventHandler(this.OnAddSeams_Click);
            // 
            // ListOfSeams
            // 
            this.ListOfSeams.FormattingEnabled = true;
            this.ListOfSeams.Location = new System.Drawing.Point(17, 55);
            this.ListOfSeams.Name = "ListOfSeams";
            this.ListOfSeams.Size = new System.Drawing.Size(121, 21);
            this.ListOfSeams.TabIndex = 1;
            // 
            // UI_Events
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(966, 486);
            this.Controls.Add(this.Seam);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.OnExportToCSV);
            this.Controls.Add(this.OnClose);
            this.Name = "UI_Events";
            this.Text = "Bảng Chọn Data";
            this.Load += new System.EventHandler(this.UI_Events_Load);
            this.Seam.ResumeLayout(false);
            this.Seam.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button OnClose;
        private System.Windows.Forms.Button OnExportToCSV;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox Seam;
        private System.Windows.Forms.Button OnAddSeams;
        private System.Windows.Forms.ComboBox ListOfSeams;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button OnStoreSeams;
        private System.Windows.Forms.Label SelectedObjectsLabel;
    }
}