namespace MyMiningPlugin.UI
{
    partial class ExportMultipleDataDialog
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
            this.lblMapName = new System.Windows.Forms.Label();
            this.txtMapName = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblSelect
            //
            this.lblSelect.AutoSize = true;
            this.lblSelect.Location = new System.Drawing.Point(20, 20);
            this.lblSelect.Name = "lblSelect";
            this.lblSelect.Size = new System.Drawing.Size(280, 15);
            this.lblSelect.TabIndex = 0;
            this.lblSelect.Text = "Chọn mục để export (có thể chọn nhiều):";
            //
            // chkItems
            //
            this.chkItems.CheckOnClick = true;
            this.chkItems.Location = new System.Drawing.Point(20, 45);
            this.chkItems.Name = "chkItems";
            this.chkItems.Size = new System.Drawing.Size(390, 200);
            this.chkItems.TabIndex = 1;
            //
            // btnSelectAll
            //
            this.btnSelectAll.Location = new System.Drawing.Point(20, 255);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(100, 25);
            this.btnSelectAll.TabIndex = 2;
            this.btnSelectAll.Text = "Chọn tất cả";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            //
            // btnDeselectAll
            //
            this.btnDeselectAll.Location = new System.Drawing.Point(130, 255);
            this.btnDeselectAll.Name = "btnDeselectAll";
            this.btnDeselectAll.Size = new System.Drawing.Size(110, 25);
            this.btnDeselectAll.TabIndex = 3;
            this.btnDeselectAll.Text = "Bỏ chọn tất cả";
            this.btnDeselectAll.UseVisualStyleBackColor = true;
            this.btnDeselectAll.Click += new System.EventHandler(this.btnDeselectAll_Click);
            //
            // lblMapName
            //
            this.lblMapName.AutoSize = true;
            this.lblMapName.Location = new System.Drawing.Point(20, 295);
            this.lblMapName.Name = "lblMapName";
            this.lblMapName.Size = new System.Drawing.Size(90, 15);
            this.lblMapName.TabIndex = 4;
            this.lblMapName.Text = "Tên file JSON:";
            //
            // txtMapName
            //
            this.txtMapName.Location = new System.Drawing.Point(20, 320);
            this.txtMapName.Name = "txtMapName";
            this.txtMapName.Size = new System.Drawing.Size(390, 23);
            this.txtMapName.TabIndex = 5;
            //
            // btnOK
            //
            this.btnOK.BackColor = System.Drawing.Color.LightYellow;
            this.btnOK.Location = new System.Drawing.Point(230, 355);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 30);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "Xuất";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(330, 355);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // ExportMultipleDataDialog
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 430);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtMapName);
            this.Controls.Add(this.lblMapName);
            this.Controls.Add(this.btnDeselectAll);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.chkItems);
            this.Controls.Add(this.lblSelect);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportMultipleDataDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSelect;
        private System.Windows.Forms.CheckedListBox chkItems;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnDeselectAll;
        private System.Windows.Forms.Label lblMapName;
        private System.Windows.Forms.TextBox txtMapName;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
