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
    public class UCTietDienLibrary : UserControl
    {
        private Action<string> _onAddTietDien;
        private Action<TietDienData> _onSelectTietDienPoly;
        private Action<TietDienData> _onDeleteTietDien;
        private Action _onExportJson;

        private List<TietDienData> _library;

        // Same reasoning as UCMineTopologyLoai2's _dgvDoan: this library can grow large,
        // and one hand-built Panel (~5 native controls) per row risked running into
        // Windows' per-process window-handle ceiling. A DataGridView renders cells
        // directly instead of instantiating a control per field per row.
        private DataGridView _dgvTietDien;
        private Label _lblCount;

        private const int ColTen = 0;
        private const int ColPolyline = 1;
        private const int ColChonPolyline = 2;
        private const int ColXoa = 3;

        public UCTietDienLibrary() { }

        // -----------------------------------------------------------------------
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

            RebuildUI();
        }

        // -----------------------------------------------------------------------
        // Full rebuild — only needed once, when the node is first selected. Adding,
        // deleting or refreshing a single Tiết diện afterwards uses AddTietDienRow/
        // RemoveTietDienRow/RefreshTietDienRow below instead, which touch that one
        // grid row only.
        public void RebuildUI()
        {
            this.SuspendLayout();
            this.Controls.Clear();
            this.AutoScroll = false;
            this.Dock = DockStyle.Fill;
            this.BackColor = SystemColors.Control;

            // Two stacked, non-overlapping bands (header / grid) — see
            // UCMineTopologyLoai2.RebuildUI for why a TableLayoutPanel with fixed/
            // percent rows is used instead of Dock Top/Fill siblings directly on `this`.
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = SystemColors.Control
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // ── Fixed header block ──────────────────────────────────────────────
            var topPanel = new Panel { Dock = DockStyle.Fill, BackColor = SystemColors.Control };
            int x = 14;
            int y = 14;

            topPanel.Controls.Add(new Label
            {
                Text = "Thư viện Tiết diện",
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold)
            });
            y += 32;

            _lblCount = new Label
            {
                Text = CountText(),
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = Color.DimGray
            };
            topPanel.Controls.Add(_lblCount);
            y += 34;

            Button btnAddTD = Btn("+ Thêm tiết diện", x, y, 165, 34,
                Color.FromArgb(34, 139, 34), Color.White);
            btnAddTD.Click += (s, e) =>
            {
                using (var dlg = new RenameDialog("Tên tiết diện mới:"))
                {
                    if (dlg.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(dlg.NewName)) return;
                    _onAddTietDien?.Invoke(dlg.NewName.Trim());
                }
            };
            topPanel.Controls.Add(btnAddTD);

            Button btnExport = Btn("Xuất JSON", x + 173, y, 165, 34,
                Color.FromArgb(70, 130, 180), Color.White);
            btnExport.Click += (s, e) => _onExportJson?.Invoke();
            topPanel.Controls.Add(btnExport);
            y += 42;

            int topHeight = y + 4;

            // ── Tiết diện grid — the bottom band, gets all remaining height ──────
            _dgvTietDien = BuildTietDienGrid();

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, topHeight));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            layout.Controls.Add(topPanel, 0, 0);
            layout.Controls.Add(_dgvTietDien, 0, 1);
            this.Controls.Add(layout);

            foreach (var td in _library ?? new List<TietDienData>())
                AddTietDienRow(td);

            this.ResumeLayout(true);
        }

        // -----------------------------------------------------------------------
        private DataGridView BuildTietDienGrid()
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(14, 10, 14, 10),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ReadOnly = true,
                BackgroundColor = SystemColors.Control,
                BorderStyle = BorderStyle.None
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTen", HeaderText = "Tên", Width = 160, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPolyline", HeaderText = "Polyline", Width = 260, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewButtonColumn { Name = "colChonPolyline", HeaderText = "", Text = "Chọn polyline", UseColumnTextForButtonValue = true, Width = 120 });
            dgv.Columns.Add(new DataGridViewButtonColumn { Name = "colXoa", HeaderText = "", Text = "Xóa", UseColumnTextForButtonValue = true, Width = 60 });

            dgv.CellContentClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                var td = (TietDienData)dgv.Rows[e.RowIndex].Tag;
                if (td == null) return;
                if (e.ColumnIndex == ColChonPolyline) _onSelectTietDienPoly?.Invoke(td);
                else if (e.ColumnIndex == ColXoa) _onDeleteTietDien?.Invoke(td);
            };

            return dgv;
        }

        // -----------------------------------------------------------------------
        // Appends one new row without touching any of the existing ones.
        public void AddTietDienRow(TietDienData td)
        {
            int idx = _dgvTietDien.Rows.Add();
            UpdateRowContent(_dgvTietDien.Rows[idx], td);
            UpdateCount();
        }

        // -----------------------------------------------------------------------
        // Removes one row in place — DataGridView re-flows the remaining rows on its own.
        public void RemoveTietDienRow(TietDienData td)
        {
            var row = FindRow(td);
            if (row != null) _dgvTietDien.Rows.Remove(row);
            UpdateCount();
        }

        // -----------------------------------------------------------------------
        // Updates one row's cell values in place (e.g. after picking a new polyline
        // changes its status) — just new cell values, no control recreation.
        public void RefreshTietDienRow(TietDienData td)
        {
            var row = FindRow(td);
            if (row != null) UpdateRowContent(row, td);
        }

        private DataGridViewRow FindRow(TietDienData td)
        {
            foreach (DataGridViewRow row in _dgvTietDien.Rows)
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
            if (_lblCount != null) _lblCount.Text = CountText();
        }

        private string CountText()
            => $"Số tiết diện: {_library?.Count ?? 0}. Dùng để gán cho Đoạn đường lò trong Địa hình lò Loại 2.";

        // -----------------------------------------------------------------------
        private static Button Btn(string text, int x, int y, int w, int h, Color back, Color fore)
            => new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = back,
                ForeColor = fore,
                UseVisualStyleBackColor = false
            };
    }
}
