namespace AutoCAD_NET_4_8_Framework
{
    partial class UCVia
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
            this.btnDeleteVia = new System.Windows.Forms.Button();
            this.btnAddBlock = new System.Windows.Forms.Button();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnDeleteVia
            // 
            this.btnDeleteVia.Location = new System.Drawing.Point(28, 162);
            this.btnDeleteVia.Name = "btnDeleteVia";
            this.btnDeleteVia.Size = new System.Drawing.Size(116, 36);
            this.btnDeleteVia.TabIndex = 10;
            this.btnDeleteVia.Text = "Xóa vỉa";
            this.btnDeleteVia.UseVisualStyleBackColor = true;
            this.btnDeleteVia.Click += new System.EventHandler(this.BtnDeleteVia_Click);
            // 
            // btnAddBlock
            // 
            this.btnAddBlock.Location = new System.Drawing.Point(28, 120);
            this.btnAddBlock.Name = "btnAddBlock";
            this.btnAddBlock.Size = new System.Drawing.Size(116, 36);
            this.btnAddBlock.TabIndex = 8;
            this.btnAddBlock.Text = "+ Thêm khối";
            this.btnAddBlock.UseVisualStyleBackColor = true;
            this.btnAddBlock.Click += new System.EventHandler(this.BtnAddBlock_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(23, 31);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(84, 25);
            this.lblHeader.TabIndex = 6;
            this.lblHeader.Text = "Tên vỉa";
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.Location = new System.Drawing.Point(25, 56);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(82, 15);
            this.lblInfo.TabIndex = 11;
            this.lblInfo.Text = "Thông số vỉa: ";
            // 
            // UCVia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnDeleteVia);
            this.Controls.Add(this.btnAddBlock);
            this.Controls.Add(this.lblHeader);
            this.Name = "UCVia";
            this.Size = new System.Drawing.Size(316, 343);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnDeleteVia;
        private System.Windows.Forms.Button btnAddBlock;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblInfo;
    }
}
