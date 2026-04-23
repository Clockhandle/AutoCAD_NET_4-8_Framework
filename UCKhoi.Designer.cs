namespace AutoCAD_NET_4_8_Framework
{
    partial class UCKhoi
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
            this.btnDeleteBlock = new System.Windows.Forms.Button();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblnfoText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnDeleteBlock
            // 
            this.btnDeleteBlock.Location = new System.Drawing.Point(27, 71);
            this.btnDeleteBlock.Name = "btnDeleteBlock";
            this.btnDeleteBlock.Size = new System.Drawing.Size(116, 36);
            this.btnDeleteBlock.TabIndex = 10;
            this.btnDeleteBlock.Text = "Xóa Khối";
            this.btnDeleteBlock.UseVisualStyleBackColor = true;
            this.btnDeleteBlock.Click += new System.EventHandler(this.BtnDeleteKhoi_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(22, 16);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(100, 25);
            this.lblHeader.TabIndex = 6;
            this.lblHeader.Text = "Tên Khối";
            // 
            // lblnfoText
            // 
            this.lblnfoText.AutoSize = true;
            this.lblnfoText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblnfoText.Location = new System.Drawing.Point(24, 41);
            this.lblnfoText.Name = "lblnfoText";
            this.lblnfoText.Size = new System.Drawing.Size(79, 15);
            this.lblnfoText.TabIndex = 12;
            this.lblnfoText.Text = "Số vách / trụ: ";
            // 
            // UCKhoi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblnfoText);
            this.Controls.Add(this.btnDeleteBlock);
            this.Controls.Add(this.lblHeader);
            this.Name = "UCKhoi";
            this.Size = new System.Drawing.Size(328, 325);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnDeleteBlock;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblnfoText;
    }
}
