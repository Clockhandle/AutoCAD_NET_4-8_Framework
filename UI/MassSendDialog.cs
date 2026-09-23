using MyMiningPlugin.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MyMiningPlugin.UI
{
    /// <summary>
    /// Mass-send dialog for "Gửi dữ liệu mới lên server" — the general, cross-category
    /// counterpart to SendMultipleDataDialog above. Instead of one category's items it
    /// lists every big branch in the project (Vỉa, Bề mặt, Địa hình lò, ...) as a
    /// checkable tree, so the user can send a batch across everything in one pass
    /// instead of opening each category's own send dialog.
    ///
    /// Only entities NOT already present in the active save slot (<paramref
    /// name="newItemsByCategory"/> — see PersistenceService.GetSavedEntityNames) start
    /// checked, shown in bold; everything already in that save file starts unchecked,
    /// since it's presumed already sent. The user can still check/uncheck anything by hand.
    /// </summary>
    public partial class MassSendDialog : Form
    {
        private bool _suppressCheckEvents;

        // Only relevant when the batch includes "Địa hình lò" items — see ConfigureFor.
        private bool _hasDateField;

        public Dictionary<string, List<string>> SelectedByCategory { get; private set; }
        public string MapName { get; private set; }
        public string ServerUrl { get; private set; }
        public DateTime? SelectedDate { get; private set; }

        public MassSendDialog(
            Dictionary<string, List<string>> itemsByCategory,
            Dictionary<string, HashSet<string>> newItemsByCategory,
            string defaultMapName = "")
        {
            InitializeComponent();
            ConfigureFor(itemsByCategory, newItemsByCategory, defaultMapName);
        }

        private void ConfigureFor(
            Dictionary<string, List<string>> itemsByCategory,
            Dictionary<string, HashSet<string>> newItemsByCategory,
            string defaultMapName)
        {
            int newCount = newItemsByCategory.Values.Sum(s => s.Count);
            lblInfo.Text = newCount > 0
                ? $"{newCount} mục chưa có trong bản lưu hiện tại đang được chọn sẵn (in đậm). Mục đã có trong bản lưu không được chọn sẵn — có thể tự chọn thêm nếu cần gửi lại."
                : "Không có mục nào mới so với bản lưu hiện tại. Chọn thủ công mục cần gửi lại:";

            var boldFont = new Font(this.Font, FontStyle.Bold);
            bool hasDateCategory = itemsByCategory.TryGetValue("Địa hình lò", out var diaHinhLo) && diaHinhLo.Count > 0;

            foreach (var kvp in itemsByCategory)
            {
                if (kvp.Value.Count == 0) continue;
                var newNames = newItemsByCategory.TryGetValue(kvp.Key, out var n) ? n : new HashSet<string>();

                var catNode = new TreeNode(kvp.Key) { Checked = kvp.Value.All(name => newNames.Contains(name)) };
                foreach (var name in kvp.Value)
                {
                    bool isNew = newNames.Contains(name);
                    var childNode = new TreeNode(name) { Checked = isNew };
                    if (isNew) childNode.NodeFont = boldFont;
                    catNode.Nodes.Add(childNode);
                }
                if (catNode.Checked) catNode.NodeFont = boldFont;
                catNode.Expand();
                tree.Nodes.Add(catNode);
            }

            _hasDateField = hasDateCategory;
            lblDate.Visible = _hasDateField;
            dtpDate.Visible = _hasDateField;
            dtpDate.Value = DateTime.Today;

            cmbMapName.Text = !string.IsNullOrEmpty(defaultMapName) ? defaultMapName : "Tong_hop_Map";
        }

        // WinForms TreeView has no indeterminate checkbox state, so a category node
        // just tracks "were all of its children checked the last time either changed" —
        // good enough for toggling a whole branch at once, which is the main use here.
        private void tree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (_suppressCheckEvents) return;
            _suppressCheckEvents = true;
            try
            {
                if (e.Node.Nodes.Count > 0)
                {
                    foreach (TreeNode child in e.Node.Nodes) child.Checked = e.Node.Checked;
                }
                else if (e.Node.Parent != null)
                {
                    bool allChecked = true;
                    foreach (TreeNode sib in e.Node.Parent.Nodes)
                        if (!sib.Checked) { allChecked = false; break; }
                    e.Node.Parent.Checked = allChecked;
                }
            }
            finally { _suppressCheckEvents = false; }
        }

        private void btnSelectAll_Click(object sender, EventArgs e) => SetAllChecked(true);

        private void btnDeselectAll_Click(object sender, EventArgs e) => SetAllChecked(false);

        private void SetAllChecked(bool value)
        {
            _suppressCheckEvents = true;
            try
            {
                foreach (TreeNode cat in tree.Nodes)
                {
                    cat.Checked = value;
                    foreach (TreeNode child in cat.Nodes) child.Checked = value;
                }
            }
            finally { _suppressCheckEvents = false; }
        }

        // --- Map selector — one shared map for the whole batch ---
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
            SelectedByCategory = new Dictionary<string, List<string>>();
            foreach (TreeNode catNode in tree.Nodes)
            {
                var names = new List<string>();
                foreach (TreeNode child in catNode.Nodes)
                    if (child.Checked) names.Add(child.Text);
                if (names.Count > 0) SelectedByCategory[catNode.Text] = names;
            }

            if (SelectedByCategory.Count == 0)
            { MessageBox.Show("Vui lòng chọn ít nhất một mục!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrWhiteSpace(cmbMapName.Text))
            { MessageBox.Show("Vui lòng chọn hoặc nhập tên bản đồ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            MapName = cmbMapName.Text.Trim();
            ServerUrl = ServerConfigService.GetServerUrl();
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
