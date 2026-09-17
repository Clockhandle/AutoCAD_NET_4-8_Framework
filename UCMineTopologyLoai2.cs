using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MyMiningPlugin.Models;

namespace AutoCAD_NET_4_8_Framework
{
    public class UCMineTopologyLoai2 : UserControl
    {
        // callbacks
        private Action _onAddDoan;
        private Action<DoanDuongLoData> _onSelectDoanPolyline;
        private Action<DoanDuongLoData> _onDeleteDoan;
        private Action _onDelete;
        private Action _onLoadTietDienJson;
        private Func<List<TietDienData>> _getTietDienLibrary;

        private MineTopologyLoai2Data _data;

        // The Đoạn đường lò list can run into the hundreds. It used to be one hand-built
        // Panel per row (~10 real native controls each — a combobox, checkbox, date picker,
        // two buttons, several labels), which meant hundreds of rows added up to thousands
        // of live window handles. Past a certain count that started colliding with Windows'
        // per-process window-handle ceiling — new controls would silently fail to get a
        // handle and just not render, which looked like rows going missing/overlapping.
        // A DataGridView renders cells directly instead of instantiating a control per
        // field per row, so it stays cheap regardless of row count.
        private DataGridView _dgvDoan;

        private const string PlaceholderTietDien = "(chưa chọn)";
        private const int ColTen = 0;
        private const int ColPolyline = 1;
        private const int ColChonDuong = 2;
        private const int ColTietDien = 3;
        private const int ColNgayKhaiThac = 4;
        private const int ColXoa = 5;

        public UCMineTopologyLoai2() { }

        // -----------------------------------------------------------------------
        public void LoadData(
            MineTopologyLoai2Data data,
            Action onAddDoan,
            Action<DoanDuongLoData> onSelectDoanPolyline,
            Action<DoanDuongLoData> onDeleteDoan,
            Action onDelete,
            Func<List<TietDienData>> getTietDienLibrary,
            Action onLoadTietDienJson)
        {
            _data = data;
            _onAddDoan = onAddDoan;
            _onSelectDoanPolyline = onSelectDoanPolyline;
            _onDeleteDoan = onDeleteDoan;
            _onDelete = onDelete;
            _getTietDienLibrary = getTietDienLibrary;
            _onLoadTietDienJson = onLoadTietDienJson;

            RebuildUI();
        }

        // -----------------------------------------------------------------------
        // Full rebuild — only needed once, when the node is first selected. Adding,
        // deleting or refreshing a single Đoạn afterwards uses AddDoanRow/RemoveDoanRow/
        // RefreshDoanRow below instead, which touch that one grid row only.
        public void RebuildUI()
        {
            this.SuspendLayout();
            this.Controls.Clear();
            this.AutoScroll = false;
            this.Dock = DockStyle.Fill;
            this.BackColor = SystemColors.Control;

            // Three stacked, non-overlapping bands (header / grid / footer). This used
            // to be three siblings docked Top/Fill/Bottom directly on `this` — Dock
            // resolution order there turned out NOT to shrink the Fill grid around its
            // Top/Bottom siblings, so the grid was sized to the full control and its
            // first rows (and header) rendered underneath the header panel, its last
            // row underneath the footer panel. A TableLayoutPanel with explicit fixed-
            // height rows for header/footer and a Percent(100) row for the grid has no
            // such ambiguity — each row gets exactly its own non-overlapping band.
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = SystemColors.Control
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // ── Fixed header block ──────────────────────────────────────────────
            var topPanel = new Panel { Dock = DockStyle.Fill, BackColor = SystemColors.Control };
            int x = 14;
            int y = 14;

            topPanel.Controls.Add(new Label
            {
                Text = $"Địa hình lò Loại 2:  {_data?.Name}",
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold)
            });
            y += 42;

            // ════════════════════════════════════════════════════════════════════
            // TIẾT DIỆN now lives in its own "Tiết diện" node under Địa hình lò —
            // Đoạn đường lò below picks from that shared library, not a private copy.
            // ════════════════════════════════════════════════════════════════════
            topPanel.Controls.Add(new Label
            {
                Text = "Tiết diện được quản lý trong mục \"Tiết diện\" của Địa hình lò.",
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 8.75f, FontStyle.Italic)
            });
            y += 24;

            Button btnLoadTDJson = Btn("Tải thư viện Tiết diện (JSON)", x, y, 230, 32,
                Color.FromArgb(128, 0, 128), Color.White);
            btnLoadTDJson.Click += (s, e) => _onLoadTietDienJson?.Invoke();
            topPanel.Controls.Add(btnLoadTDJson);
            y += 42;

            // ════════════════════════════════════════════════════════════════════
            // ĐOẠN ĐƯỜNG LÒ section
            // ════════════════════════════════════════════════════════════════════
            topPanel.Controls.Add(SectionLabel("Đoạn đường lò", x, y));
            y += 26;

            Button btnAddDoan = Btn("+ Thêm đoạn đường lò", x, y, 190, 34,
                Color.SteelBlue, Color.White);
            btnAddDoan.Click += (s, e) => _onAddDoan?.Invoke();
            topPanel.Controls.Add(btnAddDoan);
            y += 42;

            int topHeight = y + 4;

            // ── Fixed delete-entry footer ────────────────────────────────────────
            var bottomPanel = new Panel { Dock = DockStyle.Fill, BackColor = SystemColors.Control };
            const int bottomHeight = 52;
            Button btnDel = Btn("Xóa địa hình lò này", 14, 10, 175, 34, Color.White, Color.DarkRed);
            btnDel.Click += (s, e) => _onDelete?.Invoke();
            bottomPanel.Controls.Add(btnDel);

            // ── Đoạn đường lò grid — the middle band, gets all remaining height ──
            _dgvDoan = BuildDoanGrid();

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, topHeight));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, bottomHeight));

            layout.Controls.Add(topPanel, 0, 0);
            layout.Controls.Add(_dgvDoan, 0, 1);
            layout.Controls.Add(bottomPanel, 0, 2);
            this.Controls.Add(layout);

            foreach (var doan in _data?.DoanDuongLos ?? new List<DoanDuongLoData>())
                AddDoanRow(doan);

            this.ResumeLayout(true);
        }

        // -----------------------------------------------------------------------
        private DataGridView BuildDoanGrid()
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
                EditMode = DataGridViewEditMode.EditOnEnter,
                BackgroundColor = SystemColors.Control,
                BorderStyle = BorderStyle.None
            };

            // A stray combo/format mismatch here (e.g. a Tiết diện name that no longer
            // exists in the library) should never crash the whole panel.
            dgv.DataError += (s, e) => { e.ThrowException = false; };

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTen", HeaderText = "Tên", Width = 100, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPolyline", HeaderText = "Polyline", Width = 210, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewButtonColumn { Name = "colChonDuong", HeaderText = "", Text = "Chọn đường", UseColumnTextForButtonValue = true, Width = 100 });

            var colTietDien = new DataGridViewComboBoxColumn
            {
                Name = "colTietDien",
                HeaderText = "Tiết diện",
                Width = 160,
                FlatStyle = FlatStyle.Flat,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
            };
            dgv.Columns.Add(colTietDien);

            dgv.Columns.Add(new DataGridViewCalendarColumn { Name = "colNgay", HeaderText = "Ngày khai thác", Width = 170 });
            dgv.Columns.Add(new DataGridViewButtonColumn { Name = "colXoa", HeaderText = "", Text = "Xóa", UseColumnTextForButtonValue = true, Width = 60 });

            RefreshTietDienOptions(dgv);

            dgv.CellContentClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                var doan = (DoanDuongLoData)dgv.Rows[e.RowIndex].Tag;
                if (doan == null) return;
                if (e.ColumnIndex == ColChonDuong) _onSelectDoanPolyline?.Invoke(doan);
                else if (e.ColumnIndex == ColXoa) _onDeleteDoan?.Invoke(doan);
            };

            // ComboBox cells only raise CellValueChanged after the cell leaves edit
            // mode unless the edit is committed as soon as the selection changes.
            dgv.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgv.CurrentCell != null && dgv.CurrentCell.ColumnIndex == ColTietDien && dgv.IsCurrentCellDirty)
                    dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            dgv.CellValueChanged += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex != ColTietDien) return;
                var doan = (DoanDuongLoData)dgv.Rows[e.RowIndex].Tag;
                if (doan == null) return;
                var val = dgv.Rows[e.RowIndex].Cells[ColTietDien].Value as string;
                doan.TietDienName = (string.IsNullOrEmpty(val) || val == PlaceholderTietDien) ? null : val;
            };

            dgv.CellEndEdit += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex != ColNgayKhaiThac) return;
                var doan = (DoanDuongLoData)dgv.Rows[e.RowIndex].Tag;
                if (doan == null) return;

                // The cell edits through a DateTimePicker (see DataGridViewCalendarColumn
                // below), so the value arriving here is already a valid DateTime? or null —
                // no more hand-typed "yyyy-MM-dd" text to mis-parse or reject.
                doan.MinedDate = dgv.Rows[e.RowIndex].Cells[ColNgayKhaiThac].Value as DateTime?;
            };

            return dgv;
        }

        // -----------------------------------------------------------------------
        // Repopulates the Tiết diện column's dropdown options from the shared library —
        // called once when the grid is built, and again whenever the library changes
        // (e.g. after importing a Tiết diện JSON) without needing a full grid rebuild.
        public void RefreshTietDienOptions() => RefreshTietDienOptions(_dgvDoan);

        private void RefreshTietDienOptions(DataGridView dgv)
        {
            var col = (DataGridViewComboBoxColumn)dgv.Columns[ColTietDien];
            col.Items.Clear();
            col.Items.Add(PlaceholderTietDien);
            foreach (var td in _getTietDienLibrary?.Invoke() ?? new List<TietDienData>())
                col.Items.Add(td.Name);
        }

        // -----------------------------------------------------------------------
        // Appends one new row without touching any of the existing ones.
        public void AddDoanRow(DoanDuongLoData doan)
        {
            int idx = _dgvDoan.Rows.Add();
            UpdateRowContent(_dgvDoan.Rows[idx], doan);
        }

        // -----------------------------------------------------------------------
        // Removes one row in place — DataGridView re-flows the remaining rows on its own.
        public void RemoveDoanRow(DoanDuongLoData doan)
        {
            var row = FindRow(doan);
            if (row != null) _dgvDoan.Rows.Remove(row);
        }

        // -----------------------------------------------------------------------
        // Updates one row's cell values in place (e.g. after picking a new polyline
        // changes its status) — just new cell values, no control recreation.
        public void RefreshDoanRow(DoanDuongLoData doan)
        {
            var row = FindRow(doan);
            if (row != null) UpdateRowContent(row, doan);
        }

        private DataGridViewRow FindRow(DoanDuongLoData doan)
        {
            foreach (DataGridViewRow row in _dgvDoan.Rows)
                if (ReferenceEquals(row.Tag, doan)) return row;
            return null;
        }

        private void UpdateRowContent(DataGridViewRow row, DoanDuongLoData doan)
        {
            // Tag must be set before the cell values below — setting a cell's Value
            // fires CellValueChanged synchronously, and that handler reads row.Tag.
            row.Tag = doan;

            bool has = doan.Polyline != null;
            bool inDwg = has && doan.Polyline.CurrentObjectId.HasValue
                             && !doan.Polyline.CurrentObjectId.Value.IsNull;
            string polStatus = has
                ? $"{(inDwg ? "✓" : "⚠")} [{doan.Polyline.Handle}]  {doan.Polyline.Layer}"
                : "(chưa chọn đường)";

            row.Cells[ColTen].Value = doan.Name;
            row.Cells[ColPolyline].Value = polStatus;
            row.Cells[ColPolyline].Style.ForeColor = has ? (inDwg ? Color.DarkGreen : Color.DarkOrange) : Color.Gray;

            // A Tiết diện name that's no longer in the shared library (e.g. deleted
            // after being assigned) falls back to the placeholder for display only —
            // doan.TietDienName itself is left alone so it can re-match if re-added.
            var colTietDien = (DataGridViewComboBoxColumn)_dgvDoan.Columns[ColTietDien];
            bool knownTietDien = !string.IsNullOrEmpty(doan.TietDienName) && colTietDien.Items.Contains(doan.TietDienName);
            row.Cells[ColTietDien].Value = knownTietDien ? doan.TietDienName : PlaceholderTietDien;

            row.Cells[ColNgayKhaiThac].Value = doan.MinedDate;
        }

        // -----------------------------------------------------------------------
        private static Label SectionLabel(string text, int x, int y)
            => new Label
            {
                Text = $"── {text} ──",
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };

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

        // =========================================================================
        // Ngày khai thác column — edits through a DateTimePicker (with a checkbox
        // so the date can be cleared back to "not set") instead of free-typed text,
        // so it's no longer possible to fat-finger a date into MinedDate.
        //
        // Like DataGridViewComboBoxColumn, DataGridView reuses ONE shared editing
        // control for whichever cell is currently being edited — it does not create
        // a DateTimePicker per row — so this doesn't reintroduce the per-row-control
        // window-handle exhaustion problem the DataGridView switch (above) was for.
        // =========================================================================
        private class DataGridViewCalendarColumn : DataGridViewColumn
        {
            public DataGridViewCalendarColumn() : base(new DataGridViewCalendarCell()) { }

            public override DataGridViewCell CellTemplate
            {
                get => base.CellTemplate;
                set
                {
                    if (value != null && !(value is DataGridViewCalendarCell))
                        throw new InvalidCastException("CellTemplate must be a DataGridViewCalendarCell");
                    base.CellTemplate = value;
                }
            }
        }

        private class DataGridViewCalendarCell : DataGridViewTextBoxCell
        {
            public override Type EditType => typeof(DateTimePickerEditingControl);
            public override Type ValueType => typeof(DateTime?);
            public override object DefaultNewRowValue => null;

            public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
            {
                base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
                var ctl = (DateTimePickerEditingControl)DataGridView.EditingControl;
                var current = Value as DateTime?;
                ctl.Checked = current.HasValue;
                ctl.Value = current ?? DateTime.Today;
            }

            protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle,
                TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
            {
                var d = value as DateTime?;
                return d.HasValue ? d.Value.ToString("yyyy-MM-dd") : "(chưa có ngày)";
            }

            public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle,
                TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
            {
                if (formattedValue is string s && DateTime.TryParse(s, out DateTime parsed))
                    return parsed.Date;
                return null;
            }
        }

        private class DateTimePickerEditingControl : DateTimePicker, IDataGridViewEditingControl
        {
            private DataGridView _dgv;
            private bool _valueChanged;

            public DateTimePickerEditingControl()
            {
                Format = DateTimePickerFormat.Custom;
                CustomFormat = "yyyy-MM-dd";
                ShowCheckBox = true; // unchecked = no mined date yet, matches nullable MinedDate
            }

            public object EditingControlFormattedValue
            {
                get => Checked ? Value.Date.ToString("yyyy-MM-dd") : "";
                set
                {
                    if (value is string s && !string.IsNullOrEmpty(s) && DateTime.TryParse(s, out DateTime parsed))
                    {
                        Checked = true;
                        Value = parsed;
                    }
                    else
                    {
                        Checked = false;
                    }
                }
            }

            public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => EditingControlFormattedValue;

            public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle) => Font = dataGridViewCellStyle.Font;

            public int EditingControlRowIndex { get; set; }

            public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey) => false;

            public void PrepareEditingControlForEdit(bool selectAll) { }

            public bool RepositionEditingControlOnValueChange => false;

            public DataGridView EditingControlDataGridView { get => _dgv; set => _dgv = value; }

            public bool EditingControlValueChanged { get => _valueChanged; set => _valueChanged = value; }

            public Cursor EditingPanelCursor => Cursor;

            protected override void OnValueChanged(EventArgs eventargs)
            {
                _valueChanged = true;
                _dgv?.NotifyCurrentCellDirty(true);
                base.OnValueChanged(eventargs);
            }

            protected override void OnCloseUp(EventArgs eventargs)
            {
                // Toggling the checkbox doesn't raise ValueChanged on its own.
                _valueChanged = true;
                _dgv?.NotifyCurrentCellDirty(true);
                base.OnCloseUp(eventargs);
            }
        }
    }
}
