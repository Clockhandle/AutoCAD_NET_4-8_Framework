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
        private Button btnOK;
        private Button btnCancel;
        private Button btnSelectAll;
        private Button btnDeselectAll;
        private TextBox txtServerUrl;

        public System.Collections.Generic.List<string> SelectedItems { get; private set; }
        public string MapName { get; private set; }
        public string ServerUrl { get; private set; }

        public SendMultipleDataDialog(string title, System.Collections.Generic.List<string> items, string category, string defaultMapName = "")
        {
            InitializeUI(title, items, category, defaultMapName);
        }

        private void InitializeUI(string title, System.Collections.Generic.List<string> items, string category, string defaultMapName)
        {
            this.Text = title;
            this.Size = new Size(450, 530);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
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
                Text = !string.IsNullOrEmpty(defaultMapName) ? defaultMapName : $"{category}_Map"
            };
            this.Controls.Add(txtMapName);

            // Server URL
            Label lblServerUrl = new Label
            {
                Text = "Server URL:",
                Location = new Point(20, 405),
                AutoSize = true
            };
            this.Controls.Add(lblServerUrl);

            txtServerUrl = new TextBox
            {
                Location = new Point(20, 425),
                Size = new Size(390, 25),
                Text = "http://mica.edu.vn:55322/"
            };
            this.Controls.Add(txtServerUrl);

            // OK Button
            btnOK = new Button
            {
                Text = "Gửi (Send)",
                Location = new Point(230, 462),
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
                if (string.IsNullOrWhiteSpace(txtServerUrl.Text))
                {
                    MessageBox.Show("Vui lòng nhập Server URL!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                MapName = txtMapName.Text.Trim();
                ServerUrl = txtServerUrl.Text.Trim();
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(btnOK);

            // Cancel Button
            btnCancel = new Button
            {
                Text = "Hủy",
                Location = new Point(330, 462),
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

        public ExportMultipleDataDialog(string title, System.Collections.Generic.List<string> items, string category, string defaultMapName = "")
        {
            InitializeUI(title, items, category, defaultMapName);
        }

        private void InitializeUI(string title, System.Collections.Generic.List<string> items, string category, string defaultMapName)
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
                Text = !string.IsNullOrEmpty(defaultMapName) ? defaultMapName : $"{category}_Map"
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
