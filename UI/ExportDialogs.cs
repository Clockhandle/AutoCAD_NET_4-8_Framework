using MyMiningPlugin.Models;
using MyMiningPlugin.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MyMiningPlugin.UI
{
    /// <summary>
    /// Dialog for selecting multiple items to send to server.
    /// Fetches available maps from the server so the user can choose
    /// an existing map (or type a new name) rather than manually filling MapName.
    /// </summary>
    public class SendMultipleDataDialog : Form
    {
        private CheckedListBox chkItems;
        private ComboBox cmbMapName;
        private Button btnRefreshMaps;
        private Button btnOK;
        private Button btnCancel;
        private Button btnSelectAll;
        private Button btnDeselectAll;
        private TextBox txtServerUrl;
        private Label lblMapStatus;
        private DateTimePicker dtpDate;

        public List<string> SelectedItems { get; private set; }
        public string MapName { get; private set; }
        public string ServerUrl { get; private set; }
        public DateTime? SelectedDate { get; private set; }

        public SendMultipleDataDialog(string title, List<string> items, string category, string defaultMapName = "")
        {
            InitializeUI(title, items, category, defaultMapName);
        }

        private void InitializeUI(string title, List<string> items, string category, string defaultMapName)
        {
            this.Text = title;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);

            // --- Item selection list ---
            this.Controls.Add(new Label { Text = $"Chọn {category} để gửi (có thể chọn nhiều):", Location = new Point(20, 15), AutoSize = true });

            chkItems = new CheckedListBox { Location = new Point(20, 38), Size = new Size(470, 180), CheckOnClick = true };
            foreach (var item in items) chkItems.Items.Add(item, true);
            this.Controls.Add(chkItems);

            btnSelectAll = new Button { Text = "Chọn tất cả", Location = new Point(20, 228), Size = new Size(100, 25) };
            btnSelectAll.Click += (s, e) => { for (int i = 0; i < chkItems.Items.Count; i++) chkItems.SetItemChecked(i, true); };
            this.Controls.Add(btnSelectAll);

            btnDeselectAll = new Button { Text = "Bỏ chọn tất cả", Location = new Point(130, 228), Size = new Size(110, 25) };
            btnDeselectAll.Click += (s, e) => { for (int i = 0; i < chkItems.Items.Count; i++) chkItems.SetItemChecked(i, false); };
            this.Controls.Add(btnDeselectAll);

            // --- Date input (only for Địa hình lò) ---
            int dateOffset = 0;
            if (category == "Địa hình lò")
            {
                dateOffset = 55;
                this.Controls.Add(new Label { Text = "Ngày dữ liệu:", Location = new Point(20, 263), AutoSize = true });
                dtpDate = new DateTimePicker
                {
                    Location = new Point(20, 283),
                    Size = new Size(200, 25),
                    Format = DateTimePickerFormat.Short,
                    Value = DateTime.Today
                };
                this.Controls.Add(dtpDate);
            }

            // --- Single server URL (used for both upload and map listing) ---
            this.Controls.Add(new Label { Text = "Server URL:", Location = new Point(20, 268 + dateOffset), AutoSize = true });
            txtServerUrl = new TextBox { Location = new Point(20, 288 + dateOffset), Size = new Size(470, 25), Text = "http://mica.edu.vn:55320/" };
            this.Controls.Add(txtServerUrl);

            // --- Map selector ---
            this.Controls.Add(new Label { Text = "Chọn bản đồ trên server (hoặc nhập tên mới):", Location = new Point(20, 325 + dateOffset), AutoSize = true });

            cmbMapName = new ComboBox
            {
                Location = new Point(20, 345 + dateOffset),
                Size = new Size(350, 25),
                DropDownStyle = ComboBoxStyle.DropDown,
                Text = !string.IsNullOrEmpty(defaultMapName) ? defaultMapName : $"{category}_Map"
            };
            this.Controls.Add(cmbMapName);

            btnRefreshMaps = new Button { Text = "↻ Tải danh sách", Location = new Point(378, 344 + dateOffset), Size = new Size(112, 26) };
            btnRefreshMaps.Click += async (s, e) =>
            {
                string mapsUrl = txtServerUrl.Text.Trim();
                if (string.IsNullOrWhiteSpace(mapsUrl))
                {
                    MessageBox.Show("Vui lòng nhập Server URL trước.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                btnRefreshMaps.Enabled = false;
                lblMapStatus.Text = "Đang tải...";
                try
                {
                    var maps = await MapApiService.FetchMapsAsync(mapsUrl);
                    string current = cmbMapName.Text;
                    cmbMapName.Items.Clear();
                    foreach (var m in maps) cmbMapName.Items.Add(m.Name);
                    cmbMapName.Text = current;
                    lblMapStatus.Text = $"{maps.Count} bản đồ trên server.";
                }
                catch (Exception ex)
                {
                    lblMapStatus.Text = "Lỗi kết nối.";
                    MessageBox.Show($"Không thể tải danh sách bản đồ:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally { btnRefreshMaps.Enabled = true; }
            };
            this.Controls.Add(btnRefreshMaps);

            lblMapStatus = new Label { Location = new Point(20, 378 + dateOffset), AutoSize = true, ForeColor = Color.Gray, Text = "Nhấn ↻ để tải danh sách bản đồ từ server." };
            this.Controls.Add(lblMapStatus);

            // --- Action buttons ---
            btnOK = new Button { Text = "Gửi (Send)", Location = new Point(300, 453 + dateOffset), Size = new Size(90, 30), BackColor = Color.LightGreen };
            btnOK.Click += (s, e) =>
            {
                SelectedItems = new List<string>();
                foreach (var item in chkItems.CheckedItems) SelectedItems.Add(item.ToString());

                if (SelectedItems.Count == 0)
                { MessageBox.Show("Vui lòng chọn ít nhất một mục!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                if (string.IsNullOrWhiteSpace(cmbMapName.Text))
                { MessageBox.Show("Vui lòng chọn hoặc nhập tên bản đồ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                if (string.IsNullOrWhiteSpace(txtServerUrl.Text))
                { MessageBox.Show("Vui lòng nhập Server URL!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                MapName      = cmbMapName.Text.Trim();
                ServerUrl    = txtServerUrl.Text.Trim();
                SelectedDate = dtpDate?.Value;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(btnOK);

            btnCancel = new Button { Text = "Hủy", Location = new Point(400, 453 + dateOffset), Size = new Size(90, 30) };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancel);

            this.Size = new Size(520, 530 + dateOffset);
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

        public List<string> SelectedItems { get; private set; }
        public string MapName { get; private set; }

        public ExportMultipleDataDialog(string title, List<string> items, string category, string defaultMapName = "")
        {
            InitializeUI(title, items, category, defaultMapName);
        }

        private void InitializeUI(string title, List<string> items, string category, string defaultMapName)
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
                SelectedItems = new List<string>();
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
