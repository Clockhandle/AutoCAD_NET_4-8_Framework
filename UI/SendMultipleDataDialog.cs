using MyMiningPlugin.Services;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MyMiningPlugin.UI
{
    /// <summary>
    /// Dialog for selecting multiple items to send to server.
    /// Fetches available maps from the server so the user can choose
    /// an existing map (or type a new name) rather than manually filling MapName.
    /// </summary>
    public partial class SendMultipleDataDialog : Form
    {
        // Only "Địa hình lò" carries a data date here — Địa hình lò Loại 2 doesn't need
        // this row, since each Đoạn đường lò already carries its own mined date, set
        // where the line itself is chosen (UCMineTopologyLoai2). The date row/label stay
        // in the layout either way (see Designer.cs) and are just hidden when not needed.
        private bool _hasDateField;

        public List<string> SelectedItems { get; private set; }
        public string MapName { get; private set; }
        public string ServerUrl { get; private set; }
        public DateTime? SelectedDate { get; private set; }

        public SendMultipleDataDialog(string title, List<string> items, string category, string defaultMapName = "")
        {
            InitializeComponent();
            ConfigureFor(title, items, category, defaultMapName);
        }

        private void ConfigureFor(string title, List<string> items, string category, string defaultMapName)
        {
            this.Text = title;

            lblSelect.Text = $"Chọn {category} để gửi (có thể chọn nhiều):";
            foreach (var item in items) chkItems.Items.Add(item, true);

            _hasDateField = category == "Địa hình lò";
            lblDate.Visible = _hasDateField;
            dtpDate.Visible = _hasDateField;
            dtpDate.Value = DateTime.Today;

            cmbMapName.Text = !string.IsNullOrEmpty(defaultMapName) ? defaultMapName : $"{category}_Map";
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkItems.Items.Count; i++) chkItems.SetItemChecked(i, true);
        }

        private void btnDeselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkItems.Items.Count; i++) chkItems.SetItemChecked(i, false);
        }

        // --- Map selector ---
        // Server URL used to be a field here the user retyped every time; it now
        // comes from ServerConfig.xml (next to the plugin DLL) via ServerConfigService.
        private async void btnRefreshMaps_Click(object sender, EventArgs e)
        {
            string mapsUrl = ServerConfigService.GetServerUrl();
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
        }

        // --- Action buttons ---
        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectedItems = new List<string>();
            foreach (var item in chkItems.CheckedItems) SelectedItems.Add(item.ToString());

            if (SelectedItems.Count == 0)
            { MessageBox.Show("Vui lòng chọn ít nhất một mục!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrWhiteSpace(cmbMapName.Text))
            { MessageBox.Show("Vui lòng chọn hoặc nhập tên bản đồ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            MapName      = cmbMapName.Text.Trim();
            ServerUrl    = ServerConfigService.GetServerUrl();
            SelectedDate = _hasDateField ? dtpDate.Value : (DateTime?)null;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
