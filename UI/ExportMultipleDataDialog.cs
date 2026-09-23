using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MyMiningPlugin.UI
{
    /// <summary>
    /// Dialog for selecting multiple items to export
    /// </summary>
    public partial class ExportMultipleDataDialog : Form
    {
        public List<string> SelectedItems { get; private set; }
        public string MapName { get; private set; }

        public ExportMultipleDataDialog(string title, List<string> items, string category, string defaultMapName = "")
        {
            InitializeComponent();
            ConfigureFor(title, items, category, defaultMapName);
        }

        private void ConfigureFor(string title, List<string> items, string category, string defaultMapName)
        {
            this.Text = title;

            lblSelect.Text = $"Chọn {category} để export (có thể chọn nhiều):";
            foreach (var item in items) chkItems.Items.Add(item, true);

            txtMapName.Text = !string.IsNullOrEmpty(defaultMapName) ? defaultMapName : $"{category}_Map";
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkItems.Items.Count; i++)
                chkItems.SetItemChecked(i, true);
        }

        private void btnDeselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkItems.Items.Count; i++)
                chkItems.SetItemChecked(i, false);
        }

        private void btnOK_Click(object sender, EventArgs e)
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
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
