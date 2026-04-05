using MyMiningPlugin.Models;
using System.Drawing;
using System.Windows.Forms;

namespace MyMiningPlugin.UI
{
    /// <summary>
    /// Dialog for selecting multiple items to send to server
    /// </summary>
    public class SendMultipleDataDialog : Form
    {
        private CheckedListBox chkItems;
        private TextBox txtMapName;
        private TextBox txtIP;
        private TextBox txtPort;
        private Button btnOK;
        private Button btnCancel;
        private Button btnSelectAll;
        private Button btnDeselectAll;

        public System.Collections.Generic.List<string> SelectedItems { get; private set; }
        public string MapName { get; private set; }
        public string ServerIP { get; private set; }
        public string ServerPort { get; private set; }

        public SendMultipleDataDialog(string title, System.Collections.Generic.List<string> items, string category)
        {
            InitializeUI(title, items, category);
        }

        private void InitializeUI(string title, System.Collections.Generic.List<string> items, string category)
        {
            this.Text = title;
            this.Size = new Size(450, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            // Set form font to support Unicode characters
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            // Label for selection
            Label lblSelect = new Label
            {
                Text = $"Chọn {category} để gửi (có thể chọn nhiều):",
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblSelect);

            // CheckedListBox for items
            chkItems = new CheckedListBox
            {
                Location = new Point(20, 45),
                Size = new Size(390, 250),
                CheckOnClick = true
            };
            foreach (var item in items)
            {
                chkItems.Items.Add(item, true);
            }
            this.Controls.Add(chkItems);

            // Select/Deselect buttons
            btnSelectAll = new Button
            {
                Text = "Chọn tất cả",
                Location = new Point(20, 305),
                Size = new Size(100, 25)
            };
            btnSelectAll.Click += (s, e) =>
            {
                for (int i = 0; i < chkItems.Items.Count; i++)
                    chkItems.SetItemChecked(i, true);
            };
            this.Controls.Add(btnSelectAll);

            btnDeselectAll = new Button
            {
                Text = "Bỏ chọn tất cả",
                Location = new Point(130, 305),
                Size = new Size(110, 25)
            };
            btnDeselectAll.Click += (s, e) =>
            {
                for (int i = 0; i < chkItems.Items.Count; i++)
                    chkItems.SetItemChecked(i, false);
            };
            this.Controls.Add(btnDeselectAll);

            // Map name
            Label lblMapName = new Label
            {
                Text = "Tên bản đồ (Map Name):",
                Location = new Point(20, 345),
                AutoSize = true
            };
            this.Controls.Add(lblMapName);

            txtMapName = new TextBox
            {
                Location = new Point(20, 370),
                Size = new Size(390, 25),
                Text = $"{category}_Map"
            };
            this.Controls.Add(txtMapName);

            // Server IP and Port
            Label lblServer = new Label
            {
                Text = "Server IP và Port:",
                Location = new Point(20, 405),
                AutoSize = true
            };
            this.Controls.Add(lblServer);

            txtIP = new TextBox
            {
                Location = new Point(20, 430),
                Size = new Size(200, 25),
                Text = "127.0.0.1"
            };
            this.Controls.Add(txtIP);

            txtPort = new TextBox
            {
                Location = new Point(230, 430),
                Size = new Size(80, 25),
                Text = "3000"
            };
            this.Controls.Add(txtPort);

            // OK Button
            btnOK = new Button
            {
                Text = "Gửi (Send)",
                Location = new Point(230, 465),
                Size = new Size(90, 30),
                BackColor = Color.LightGreen
            };
            btnOK.Click += (s, e) =>
            {
                SelectedItems = new System.Collections.Generic.List<string>();
                foreach (var item in chkItems.CheckedItems)
                {
                    SelectedItems.Add(item.ToString());
                }

                if (SelectedItems.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một mục!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtMapName.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên bản đồ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                MapName = txtMapName.Text.Trim();
                ServerIP = txtIP.Text.Trim();
                ServerPort = txtPort.Text.Trim();
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(btnOK);

            // Cancel Button
            btnCancel = new Button
            {
                Text = "Hủy",
                Location = new Point(330, 465),
                Size = new Size(80, 30)
            };
            btnCancel.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            this.Controls.Add(btnCancel);
        }
    }

    /// <summary>
    /// Dialog for selecting multiple items to export
    /// </summary>
    public class ExportMultipleDataDialog : Form
    {
        private CheckedListBox chkItems;
        private TextBox txtMapName;
        private Button btnOK;
        private Button btnCancel;
        private Button btnSelectAll;
        private Button btnDeselectAll;

        public System.Collections.Generic.List<string> SelectedItems { get; private set; }
        public string MapName { get; private set; }

        public ExportMultipleDataDialog(string title, System.Collections.Generic.List<string> items, string category)
        {
            InitializeUI(title, items, category);
        }

        private void InitializeUI(string title, System.Collections.Generic.List<string> items, string category)
        {
            this.Text = title;
            this.Size = new Size(450, 430);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            // Set form font to support Unicode characters
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            // Label for selection
            Label lblSelect = new Label
            {
                Text = $"Chọn {category} để export (có thể chọn nhiều):",
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblSelect);

            // CheckedListBox
            chkItems = new CheckedListBox
            {
                Location = new Point(20, 45),
                Size = new Size(390, 200),
                CheckOnClick = true
            };
            foreach (var item in items)
            {
                chkItems.Items.Add(item, true);
            }
            this.Controls.Add(chkItems);

            // Select/Deselect buttons
            btnSelectAll = new Button
            {
                Text = "Chọn tất cả",
                Location = new Point(20, 255),
                Size = new Size(100, 25)
            };
            btnSelectAll.Click += (s, e) =>
            {
                for (int i = 0; i < chkItems.Items.Count; i++)
                    chkItems.SetItemChecked(i, true);
            };
            this.Controls.Add(btnSelectAll);

            btnDeselectAll = new Button
            {
                Text = "Bỏ chọn tất cả",
                Location = new Point(130, 255),
                Size = new Size(110, 25)
            };
            btnDeselectAll.Click += (s, e) =>
            {
                for (int i = 0; i < chkItems.Items.Count; i++)
                    chkItems.SetItemChecked(i, false);
            };
            this.Controls.Add(btnDeselectAll);

            // Map name
            Label lblMapName = new Label
            {
                Text = "Tên file JSON:",
                Location = new Point(20, 295),
                AutoSize = true
            };
            this.Controls.Add(lblMapName);

            txtMapName = new TextBox
            {
                Location = new Point(20, 320),
                Size = new Size(390, 25),
                Text = $"{category}_Map"
            };
            this.Controls.Add(txtMapName);

            // OK Button
            btnOK = new Button
            {
                Text = "Xuất",
                Location = new Point(230, 355),
                Size = new Size(90, 30),
                BackColor = Color.LightYellow
            };
            btnOK.Click += (s, e) =>
            {
                SelectedItems = new System.Collections.Generic.List<string>();
                foreach (var item in chkItems.CheckedItems)
                {
                    SelectedItems.Add(item.ToString());
                }

                if (SelectedItems.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một mục!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtMapName.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên file!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                MapName = txtMapName.Text.Trim();
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(btnOK);

            // Cancel Button
            btnCancel = new Button
            {
                Text = "Hủy",
                Location = new Point(330, 355),
                Size = new Size(80, 30)
            };
            btnCancel.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            this.Controls.Add(btnCancel);
        }
    }
}
