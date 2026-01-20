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
            this.lblSeamCount = new System.Windows.Forms.Label();
            this.OnStoreSeams = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.OnAddSeams = new System.Windows.Forms.Button();
            this.ListOfSeams = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtServerUrl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Roof = new System.Windows.Forms.GroupBox();
            this.lblRoofCount = new System.Windows.Forms.Label();
            this.OnStoreRoofs = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.OnAddRoofs = new System.Windows.Forms.Button();
            this.ListOfRoofs = new System.Windows.Forms.ComboBox();
            this.Floor = new System.Windows.Forms.GroupBox();
            this.lblFloorCount = new System.Windows.Forms.Label();
            this.OnStoreFloors = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.OnAddFloors = new System.Windows.Forms.Button();
            this.ListOfFloors = new System.Windows.Forms.ComboBox();
            this.Fault = new System.Windows.Forms.GroupBox();
            this.lblFaultCount = new System.Windows.Forms.Label();
            this.OnStoreFaults = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.OnAddFaults = new System.Windows.Forms.Button();
            this.ListOfFaults = new System.Windows.Forms.ComboBox();
            this.Seam.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.Roof.SuspendLayout();
            this.Floor.SuspendLayout();
            this.Fault.SuspendLayout();
            this.SuspendLayout();
            // 
            // OnClose
            // 
            this.OnClose.Location = new System.Drawing.Point(373, 371);
            this.OnClose.Name = "OnClose";
            this.OnClose.Size = new System.Drawing.Size(192, 45);
            this.OnClose.TabIndex = 1;
            this.OnClose.Text = "Close";
            this.OnClose.UseVisualStyleBackColor = true;
            this.OnClose.Click += new System.EventHandler(this.OnClose_Click);
            // 
            // OnExportToCSV
            // 
            this.OnExportToCSV.Location = new System.Drawing.Point(373, 311);
            this.OnExportToCSV.Name = "OnExportToCSV";
            this.OnExportToCSV.Size = new System.Drawing.Size(192, 45);
            this.OnExportToCSV.TabIndex = 2;
            this.OnExportToCSV.Text = "ExportToJSON";
            this.OnExportToCSV.UseVisualStyleBackColor = true;
            this.OnExportToCSV.Click += new System.EventHandler(this.OnExportToJSON_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(92, 51);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(86, 25);
            this.button3.TabIndex = 5;
            this.button3.Text = "SendToServer";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.OnSendToServer_Click);
            // 
            // Seam
            // 
            this.Seam.Controls.Add(this.lblSeamCount);
            this.Seam.Controls.Add(this.OnStoreSeams);
            this.Seam.Controls.Add(this.label1);
            this.Seam.Controls.Add(this.OnAddSeams);
            this.Seam.Controls.Add(this.ListOfSeams);
            this.Seam.Location = new System.Drawing.Point(12, 12);
            this.Seam.Name = "Seam";
            this.Seam.Size = new System.Drawing.Size(295, 162);
            this.Seam.TabIndex = 6;
            this.Seam.TabStop = false;
            this.Seam.Text = "Vía";
            // 
            // lblSeamCount
            // 
            this.lblSeamCount.AutoSize = true;
            this.lblSeamCount.Location = new System.Drawing.Point(141, 123);
            this.lblSeamCount.Name = "lblSeamCount";
            this.lblSeamCount.Size = new System.Drawing.Size(131, 13);
            this.lblSeamCount.TabIndex = 6;
            this.lblSeamCount.Text = "Số vía trong danh sách: 0";
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtServerUrl);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(373, 219);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(192, 86);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SendToServer";
            // 
            // txtServerUrl
            // 
            this.txtServerUrl.Location = new System.Drawing.Point(38, 25);
            this.txtServerUrl.Name = "txtServerUrl";
            this.txtServerUrl.Size = new System.Drawing.Size(140, 20);
            this.txtServerUrl.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "IP : ";
            // 
            // Roof
            // 
            this.Roof.Controls.Add(this.lblRoofCount);
            this.Roof.Controls.Add(this.OnStoreRoofs);
            this.Roof.Controls.Add(this.label4);
            this.Roof.Controls.Add(this.OnAddRoofs);
            this.Roof.Controls.Add(this.ListOfRoofs);
            this.Roof.Location = new System.Drawing.Point(12, 180);
            this.Roof.Name = "Roof";
            this.Roof.Size = new System.Drawing.Size(295, 162);
            this.Roof.TabIndex = 7;
            this.Roof.TabStop = false;
            this.Roof.Text = "Vách";
            // 
            // lblRoofCount
            // 
            this.lblRoofCount.AutoSize = true;
            this.lblRoofCount.Location = new System.Drawing.Point(141, 123);
            this.lblRoofCount.Name = "lblRoofCount";
            this.lblRoofCount.Size = new System.Drawing.Size(139, 13);
            this.lblRoofCount.TabIndex = 6;
            this.lblRoofCount.Text = "Số vách trong danh sách: 0";
            // 
            // OnStoreRoofs
            // 
            this.OnStoreRoofs.Location = new System.Drawing.Point(144, 84);
            this.OnStoreRoofs.Name = "OnStoreRoofs";
            this.OnStoreRoofs.Size = new System.Drawing.Size(75, 23);
            this.OnStoreRoofs.TabIndex = 5;
            this.OnStoreRoofs.Text = "Lưu";
            this.OnStoreRoofs.UseVisualStyleBackColor = true;
            this.OnStoreRoofs.Click += new System.EventHandler(this.OnStoreRoofs_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Danh sách vách:";
            // 
            // OnAddRoofs
            // 
            this.OnAddRoofs.Location = new System.Drawing.Point(144, 55);
            this.OnAddRoofs.Name = "OnAddRoofs";
            this.OnAddRoofs.Size = new System.Drawing.Size(75, 23);
            this.OnAddRoofs.TabIndex = 2;
            this.OnAddRoofs.Text = "Thêm vách";
            this.OnAddRoofs.UseVisualStyleBackColor = true;
            this.OnAddRoofs.Click += new System.EventHandler(this.OnAddRoofs_Click);
            // 
            // ListOfRoofs
            // 
            this.ListOfRoofs.FormattingEnabled = true;
            this.ListOfRoofs.Location = new System.Drawing.Point(17, 55);
            this.ListOfRoofs.Name = "ListOfRoofs";
            this.ListOfRoofs.Size = new System.Drawing.Size(121, 21);
            this.ListOfRoofs.TabIndex = 1;
            // 
            // Floor
            // 
            this.Floor.Controls.Add(this.lblFloorCount);
            this.Floor.Controls.Add(this.OnStoreFloors);
            this.Floor.Controls.Add(this.label6);
            this.Floor.Controls.Add(this.OnAddFloors);
            this.Floor.Controls.Add(this.ListOfFloors);
            this.Floor.Location = new System.Drawing.Point(12, 353);
            this.Floor.Name = "Floor";
            this.Floor.Size = new System.Drawing.Size(295, 162);
            this.Floor.TabIndex = 7;
            this.Floor.TabStop = false;
            this.Floor.Text = "Trụ";
            // 
            // lblFloorCount
            // 
            this.lblFloorCount.AutoSize = true;
            this.lblFloorCount.Location = new System.Drawing.Point(141, 123);
            this.lblFloorCount.Name = "lblFloorCount";
            this.lblFloorCount.Size = new System.Drawing.Size(127, 13);
            this.lblFloorCount.TabIndex = 6;
            this.lblFloorCount.Text = "Số trụ trong danh sách: 0";
            // 
            // OnStoreFloors
            // 
            this.OnStoreFloors.Location = new System.Drawing.Point(144, 84);
            this.OnStoreFloors.Name = "OnStoreFloors";
            this.OnStoreFloors.Size = new System.Drawing.Size(75, 23);
            this.OnStoreFloors.TabIndex = 5;
            this.OnStoreFloors.Text = "Lưu";
            this.OnStoreFloors.UseVisualStyleBackColor = true;
            this.OnStoreFloors.Click += new System.EventHandler(this.OnStoreFloors_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Danh sách trụ:";
            // 
            // OnAddFloors
            // 
            this.OnAddFloors.Location = new System.Drawing.Point(144, 55);
            this.OnAddFloors.Name = "OnAddFloors";
            this.OnAddFloors.Size = new System.Drawing.Size(75, 23);
            this.OnAddFloors.TabIndex = 2;
            this.OnAddFloors.Text = "Thêm trụ";
            this.OnAddFloors.UseVisualStyleBackColor = true;
            this.OnAddFloors.Click += new System.EventHandler(this.OnAddFloors_Click);
            // 
            // ListOfFloors
            // 
            this.ListOfFloors.FormattingEnabled = true;
            this.ListOfFloors.Location = new System.Drawing.Point(17, 55);
            this.ListOfFloors.Name = "ListOfFloors";
            this.ListOfFloors.Size = new System.Drawing.Size(121, 21);
            this.ListOfFloors.TabIndex = 1;
            // 
            // Fault
            // 
            this.Fault.Controls.Add(this.lblFaultCount);
            this.Fault.Controls.Add(this.OnStoreFaults);
            this.Fault.Controls.Add(this.label5);
            this.Fault.Controls.Add(this.OnAddFaults);
            this.Fault.Controls.Add(this.ListOfFaults);
            this.Fault.Location = new System.Drawing.Point(313, 12);
            this.Fault.Name = "Fault";
            this.Fault.Size = new System.Drawing.Size(295, 162);
            this.Fault.TabIndex = 7;
            this.Fault.TabStop = false;
            this.Fault.Text = "Đứt gãy";
            // 
            // lblFaultCount
            // 
            this.lblFaultCount.AutoSize = true;
            this.lblFaultCount.Location = new System.Drawing.Point(141, 123);
            this.lblFaultCount.Name = "lblFaultCount";
            this.lblFaultCount.Size = new System.Drawing.Size(151, 13);
            this.lblFaultCount.TabIndex = 6;
            this.lblFaultCount.Text = "Số đứt gãy trong danh sách: 0";
            // 
            // OnStoreFaults
            // 
            this.OnStoreFaults.Location = new System.Drawing.Point(144, 84);
            this.OnStoreFaults.Name = "OnStoreFaults";
            this.OnStoreFaults.Size = new System.Drawing.Size(88, 23);
            this.OnStoreFaults.TabIndex = 5;
            this.OnStoreFaults.Text = "Lưu";
            this.OnStoreFaults.UseVisualStyleBackColor = true;
            this.OnStoreFaults.Click += new System.EventHandler(this.OnStoreFaults_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Danh sách đứt gãy:";
            // 
            // OnAddFaults
            // 
            this.OnAddFaults.Location = new System.Drawing.Point(144, 55);
            this.OnAddFaults.Name = "OnAddFaults";
            this.OnAddFaults.Size = new System.Drawing.Size(88, 23);
            this.OnAddFaults.TabIndex = 2;
            this.OnAddFaults.Text = "Thêm đứt gãy";
            this.OnAddFaults.UseVisualStyleBackColor = true;
            this.OnAddFaults.Click += new System.EventHandler(this.OnAddFaults_Click);
            // 
            // ListOfFaults
            // 
            this.ListOfFaults.FormattingEnabled = true;
            this.ListOfFaults.Location = new System.Drawing.Point(17, 55);
            this.ListOfFaults.Name = "ListOfFaults";
            this.ListOfFaults.Size = new System.Drawing.Size(121, 21);
            this.ListOfFaults.TabIndex = 1;
            // 
            // UI_Events
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(624, 530);
            this.Controls.Add(this.Fault);
            this.Controls.Add(this.Floor);
            this.Controls.Add(this.Roof);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Seam);
            this.Controls.Add(this.OnExportToCSV);
            this.Controls.Add(this.OnClose);
            this.Name = "UI_Events";
            this.Text = "Bảng Chọn Data";
            this.Load += new System.EventHandler(this.UI_Events_Load);
            this.Seam.ResumeLayout(false);
            this.Seam.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.Roof.ResumeLayout(false);
            this.Roof.PerformLayout();
            this.Floor.ResumeLayout(false);
            this.Floor.PerformLayout();
            this.Fault.ResumeLayout(false);
            this.Fault.PerformLayout();
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
        private System.Windows.Forms.Label lblSeamCount;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServerUrl;
        private System.Windows.Forms.GroupBox Roof;
        private System.Windows.Forms.Label lblRoofCount;
        private System.Windows.Forms.Button OnStoreRoofs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button OnAddRoofs;
        private System.Windows.Forms.ComboBox ListOfRoofs;
        private System.Windows.Forms.GroupBox Floor;
        private System.Windows.Forms.Label lblFloorCount;
        private System.Windows.Forms.Button OnStoreFloors;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button OnAddFloors;
        private System.Windows.Forms.ComboBox ListOfFloors;
        private System.Windows.Forms.GroupBox Fault;
        private System.Windows.Forms.Label lblFaultCount;
        private System.Windows.Forms.Button OnStoreFaults;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button OnAddFaults;
        private System.Windows.Forms.ComboBox ListOfFaults;
    }
}