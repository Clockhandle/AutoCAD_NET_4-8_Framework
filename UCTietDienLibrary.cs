using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MyMiningPlugin.Models;

namespace AutoCAD_NET_4_8_Framework
{
    /// <summary>
    /// Global, reusable Tiết diện library — lives under Địa hình lò as its own permanent
    /// node (regardless of how many Loại 1 / Loại 2 entries exist). Each entry here is a
    /// named cross-section polyline; Địa hình lò Loại 2 entries pick their Đoạn đường lò's
    /// Tiết diện from this same list rather than keeping their own private copy.
    /// </summary>
    public partial class UCTietDienLibrary : UserControl
    {
        private Action<string> _onAddTietDien;
        private Action<TietDienData> _onSelectTietDienPoly;
        private Action<TietDienData> _onDeleteTietDien;
        private Action _onExportJson;

        private List<TietDienData> _library;

        private const int ColTen = 0;
        private const int ColPolyline = 1;
        private const int ColChonPolyline = 2;
        private const int ColXoa = 3;

        public UCTietDienLibrary()
        {
            InitializeComponent();
        }

        public void LoadData(
            List<TietDienData> library,
            Action<string> onAddTietDien,
            Action<TietDienData> onSelectTietDienPoly,
            Action<TietDienData> onDeleteTietDien,
            Action onExportJson)
        {
            _library = library;
            _onAddTietDien = onAddTietDien;
            _onSelectTietDienPoly = onSelectTietDienPoly;
            _onDeleteTietDien = onDeleteTietDien;
            _onExportJson = onExportJson;

            PopulateFromData();
        }

        // Repopulates the grid from _library — called on first load. Adding, deleting or
        // refreshing a single Tiết diện afterwards uses AddTietDienRow/RemoveTietDienRow/
        // RefreshTietDienRow below instead, which touch that one grid row only.
        private void PopulateFromData()
        {
            dgvTietDien.Rows.Clear();
            foreach (var td in _library ?? new List<TietDienData>())
                AddTietDienRow(td);
        }

        private void btnAddTietDien_Click(object sender, EventArgs e)
        {
            using (var dlg = new RenameDialog("Tên tiết diện mới:"))
            {
                if (dlg.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(dlg.NewName)) return;
                _onAddTietDien?.Invoke(dlg.NewName.Trim());
            }
        }

        private void btnExportJson_Click(object sender, EventArgs e) => _onExportJson?.Invoke();

        private void dgvTietDien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var td = (TietDienData)dgvTietDien.Rows[e.RowIndex].Tag;
            if (td == null) return;
            if (e.ColumnIndex == ColChonPolyline) _onSelectTietDienPoly?.Invoke(td);
            else if (e.ColumnIndex == ColXoa) _onDeleteTietDien?.Invoke(td);
        }

        // Appends one new row without touching any of the existing ones.
        public void AddTietDienRow(TietDienData td)
        {
            int idx = dgvTietDien.Rows.Add();
            UpdateRowContent(dgvTietDien.Rows[idx], td);
            UpdateCount();
        }

        // Removes one row in place — DataGridView re-flows the remaining rows on its own.
        public void RemoveTietDienRow(TietDienData td)
        {
            var row = FindRow(td);
            if (row != null) dgvTietDien.Rows.Remove(row);
            UpdateCount();
        }

        // Updates one row's cell values in place (e.g. after picking a new polyline
        // changes its status) — just new cell values, no control recreation.
        public void RefreshTietDienRow(TietDienData td)
        {
            var row = FindRow(td);
            if (row != null) UpdateRowContent(row, td);
        }

        private DataGridViewRow FindRow(TietDienData td)
        {
            foreach (DataGridViewRow row in dgvTietDien.Rows)
                if (ReferenceEquals(row.Tag, td)) return row;
            return null;
        }

        private void UpdateRowContent(DataGridViewRow row, TietDienData td)
        {
            row.Tag = td;

            bool has = td.Polyline != null;
            bool inDwg = has && td.Polyline.CurrentObjectId.HasValue
                             && !td.Polyline.CurrentObjectId.Value.IsNull;
            string status = has
                ? $"{(inDwg ? "✓" : "⚠")} [{td.Polyline.Handle}]  {td.Polyline.Layer}  ({td.Polyline.SourceDwgName})"
                : "(chưa chọn polyline)";

            row.Cells[ColTen].Value = td.Name;
            row.Cells[ColPolyline].Value = status;
            row.Cells[ColPolyline].Style.ForeColor = has ? (inDwg ? Color.DarkGreen : Color.DarkOrange) : Color.Gray;
        }

        private void UpdateCount()
        {
            lblCount.Text = CountText();
        }

        private string CountText()
            => $"Số tiết diện: {_library?.Count ?? 0}. Dùng để gán cho Đoạn đường lò trong Địa hình lò Loại 2.";
    }
}
