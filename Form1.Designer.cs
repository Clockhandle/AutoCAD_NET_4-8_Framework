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
            this.OnClose = new System.Windows.Forms.Button();
            this.OnExportToCSV = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // OnSelectObject
            // 
            this.OnSelectObject.Location = new System.Drawing.Point(41, 26);
            this.OnSelectObject.Name = "OnSelectObject";
            this.OnSelectObject.Size = new System.Drawing.Size(186, 47);
            this.OnSelectObject.TabIndex = 0;
            this.OnSelectObject.Text = "SelectA";
            this.OnSelectObject.UseVisualStyleBackColor = true;
            this.OnSelectObject.Click += new System.EventHandler(this.OnSelectObject_A);
            // 
            // OnClose
            // 
            this.OnClose.Location = new System.Drawing.Point(309, 387);
            this.OnClose.Name = "OnClose";
            this.OnClose.Size = new System.Drawing.Size(187, 45);
            this.OnClose.TabIndex = 1;
            this.OnClose.Text = "Close";
            this.OnClose.UseVisualStyleBackColor = true;
            this.OnClose.Click += new System.EventHandler(this.OnClose_Click);
            // 
            // OnExportToCSV
            // 
            this.OnExportToCSV.Location = new System.Drawing.Point(41, 387);
            this.OnExportToCSV.Name = "OnExportToCSV";
            this.OnExportToCSV.Size = new System.Drawing.Size(186, 45);
            this.OnExportToCSV.TabIndex = 2;
            this.OnExportToCSV.Text = "ExportToJSON";
            this.OnExportToCSV.UseVisualStyleBackColor = true;
            this.OnExportToCSV.Click += new System.EventHandler(this.OnExportToJSON_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(41, 99);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(186, 47);
            this.button1.TabIndex = 3;
            this.button1.Text = "SelectB";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnSelectObject_B);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(41, 171);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(186, 47);
            this.button2.TabIndex = 4;
            this.button2.Text = "SelectC";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.OnSelectObject_C);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(966, 486);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.OnExportToCSV);
            this.Controls.Add(this.OnClose);
            this.Controls.Add(this.OnSelectObject);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button OnSelectObject;
        private System.Windows.Forms.Button OnClose;
        private System.Windows.Forms.Button OnExportToCSV;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}