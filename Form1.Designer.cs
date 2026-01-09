namespace AutoCAD_NET_4_8_Framework
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.OnSelectObject = new System.Windows.Forms.Button();
            this.OnConfirmSelectedObject = new System.Windows.Forms.Button();
            this.OnExportToCSV = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // OnSelectObject
            // 
            this.OnSelectObject.Location = new System.Drawing.Point(330, 43);
            this.OnSelectObject.Name = "OnSelectObject";
            this.OnSelectObject.Size = new System.Drawing.Size(262, 101);
            this.OnSelectObject.TabIndex = 0;
            this.OnSelectObject.Text = "SelectObject";
            this.OnSelectObject.UseVisualStyleBackColor = true;
            this.OnSelectObject.Click += new System.EventHandler(this.OnSelectObject_Click);
            // 
            // OnConfirmSelectedObject
            // 
            this.OnConfirmSelectedObject.Location = new System.Drawing.Point(330, 322);
            this.OnConfirmSelectedObject.Name = "OnConfirmSelectedObject";
            this.OnConfirmSelectedObject.Size = new System.Drawing.Size(262, 110);
            this.OnConfirmSelectedObject.TabIndex = 1;
            this.OnConfirmSelectedObject.Text = "Confirm";
            this.OnConfirmSelectedObject.UseVisualStyleBackColor = true;
            this.OnConfirmSelectedObject.Click += new System.EventHandler(this.OnConfirmSelectedObject_Click);
            // 
            // OnExportToCSV
            // 
            this.OnExportToCSV.Location = new System.Drawing.Point(330, 183);
            this.OnExportToCSV.Name = "OnExportToCSV";
            this.OnExportToCSV.Size = new System.Drawing.Size(262, 101);
            this.OnExportToCSV.TabIndex = 2;
            this.OnExportToCSV.Text = "ExportToCSV";
            this.OnExportToCSV.UseVisualStyleBackColor = true;
            this.OnExportToCSV.Click += new System.EventHandler(this.OnExportToCSV_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(966, 486);
            this.Controls.Add(this.OnExportToCSV);
            this.Controls.Add(this.OnConfirmSelectedObject);
            this.Controls.Add(this.OnSelectObject);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button OnSelectObject;
        private System.Windows.Forms.Button OnConfirmSelectedObject;
        private System.Windows.Forms.Button OnExportToCSV;
    }
}