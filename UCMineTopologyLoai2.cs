using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MyMiningPlugin.Models;

namespace AutoCAD_NET_4_8_Framework
{
    public partial class UCMineTopologyLoai2 : UserControl
    {
        // callbacks
        private Action _onAddDoan;
        private Action<DoanDuongLoData> _onSelectDoanPolyline;
        private Action<DoanDuongLoData> _onDeleteDoan;
        private Action _onDelete;
        private Action _onLoadTietDienJson;
        private Func<List<TietDienData>> _getTietDienLibrary;

        private MineTopologyLoai2Data _data;

        private const string PlaceholderTietDien = "(chưa chọn)";
        private const int ColTen = 0;
        private const int ColPolyline = 1;
        private const int ColChonDuong = 2;
        private const int ColTietDien = 3;
        private const int ColNgayKhaiThac = 4;
        private const int ColXoa = 5;

        public UCMineTopologyLoai2()
        {
            InitializeComponent();
        }

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

            PopulateFromData();
        }

        // Repopulates header text, the Tiết diện dropdown options and every Đoạn đường lò
        // row from _data — called once, when the node is first selected. Adding, deleting
        // or refreshing a single Đoạn afterwards uses AddDoanRow/RemoveDoanRow/
        // RefreshDoanRow below instead, which touch that one grid row only.
        private void PopulateFromData()
        {
            lblTitle.Text = $"Địa hình lò Loại 2:  {_data?.Name}";

            RefreshTietDienOptions();

            dgvDoan.Rows.Clear();
            foreach (var doan in _data?.DoanDuongLos ?? new List<DoanDuongLoData>())
                AddDoanRow(doan);
        }

        private void btnLoadTietDienJson_Click(object sender, EventArgs e) => _onLoadTietDienJson?.Invoke();

        private void btnAddDoan_Click(object sender, EventArgs e) => _onAddDoan?.Invoke();

        private void btnDeleteTopo_Click(object sender, EventArgs e) => _onDelete?.Invoke();

        // A stray combo/format mismatch here (e.g. a Tiết diện name that no longer
        // exists in the library) should never crash the whole panel.
        private void dgvDoan_DataError(object sender, DataGridViewDataErrorEventArgs e) => e.ThrowException = false;

        // Otherwise the last-clicked cell stays highlighted (in the "inactive
        // selection" grey/blue) even after focus moves elsewhere in the form.
        private void dgvDoan_Leave(object sender, EventArgs e) => dgvDoan.ClearSelection();

        private void dgvDoan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var doan = (DoanDuongLoData)dgvDoan.Rows[e.RowIndex].Tag;
            if (doan == null) return;
            if (e.ColumnIndex == ColChonDuong) _onSelectDoanPolyline?.Invoke(doan);
            else if (e.ColumnIndex == ColXoa) _onDeleteDoan?.Invoke(doan);
        }

        // ComboBox cells only raise CellValueChanged after the cell leaves edit
        // mode unless the edit is committed as soon as the selection changes.
        private void dgvDoan_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvDoan.CurrentCell != null && dgvDoan.CurrentCell.ColumnIndex == ColTietDien && dgvDoan.IsCurrentCellDirty)
                dgvDoan.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvDoan_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != ColTietDien) return;
            var doan = (DoanDuongLoData)dgvDoan.Rows[e.RowIndex].Tag;
            if (doan == null) return;
            var val = dgvDoan.Rows[e.RowIndex].Cells[ColTietDien].Value as string;
            doan.TietDienName = (string.IsNullOrEmpty(val) || val == PlaceholderTietDien) ? null : val;
        }

        private void dgvDoan_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var doan = (DoanDuongLoData)dgvDoan.Rows[e.RowIndex].Tag;
            if (doan == null) return;

            if (e.ColumnIndex == ColTen)
            {
                var cell = dgvDoan.Rows[e.RowIndex].Cells[ColTen];
                string newName = (cell.Value as string)?.Trim();
                // An emptied name would leave the segment unidentifiable, so fall
                // back to whatever it was named before rather than accepting blank.
                cell.Value = doan.Name = string.IsNullOrEmpty(newName) ? doan.Name : newName;
            }
            else if (e.ColumnIndex == ColNgayKhaiThac)
            {
                // The cell edits through a DateTimePicker (see DataGridViewCalendarColumn
                // below), so the value arriving here is already a valid DateTime? or null —
                // no more hand-typed "yyyy-MM-dd" text to mis-parse or reject.
                doan.MinedDate = dgvDoan.Rows[e.RowIndex].Cells[ColNgayKhaiThac].Value as DateTime?;
            }
        }

        // Repopulates the Tiết diện column's dropdown options from the shared library —
        // called once when the grid is built, and again whenever the library changes
        // (e.g. after importing a Tiết diện JSON) without needing a full grid rebuild.
        public void RefreshTietDienOptions()
        {
            var col = (DataGridViewComboBoxColumn)dgvDoan.Columns[ColTietDien];
            col.Items.Clear();
            col.Items.Add(PlaceholderTietDien);
            foreach (var td in _getTietDienLibrary?.Invoke() ?? new List<TietDienData>())
                col.Items.Add(td.Name);
        }

        // Appends one new row without touching any of the existing ones.
        public void AddDoanRow(DoanDuongLoData doan)
        {
            int idx = dgvDoan.Rows.Add();
            UpdateRowContent(dgvDoan.Rows[idx], doan);
        }

        // Removes one row in place — DataGridView re-flows the remaining rows on its own.
        public void RemoveDoanRow(DoanDuongLoData doan)
        {
            var row = FindRow(doan);
            if (row != null) dgvDoan.Rows.Remove(row);
        }

        // Updates one row's cell values in place (e.g. after picking a new polyline
        // changes its status) — just new cell values, no control recreation.
        public void RefreshDoanRow(DoanDuongLoData doan)
        {
            var row = FindRow(doan);
            if (row != null) UpdateRowContent(row, doan);
        }

        private DataGridViewRow FindRow(DoanDuongLoData doan)
        {
            foreach (DataGridViewRow row in dgvDoan.Rows)
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
            var colTietDien = (DataGridViewComboBoxColumn)dgvDoan.Columns[ColTietDien];
            bool knownTietDien = !string.IsNullOrEmpty(doan.TietDienName) && colTietDien.Items.Contains(doan.TietDienName);
            row.Cells[ColTietDien].Value = knownTietDien ? doan.TietDienName : PlaceholderTietDien;

            row.Cells[ColNgayKhaiThac].Value = doan.MinedDate;
        }

        // =========================================================================
        // Ngày khai thác column — edits through a DateTimePicker (with a checkbox
        // so the date can be cleared back to "not set") instead of free-typed text,
        // so it's no longer possible to fat-finger a date into MinedDate.
        //
        // Like DataGridViewComboBoxColumn, DataGridView reuses ONE shared editing
        // control for whichever cell is currently being edited — it does not create
        // a DateTimePicker per row — so this doesn't reintroduce the per-row-control
        // window-handle exhaustion problem the DataGridView switch (above) was for.
        //
        // Instantiated by name from InitializeComponent (Designer.cs) — being a
        // nested class doesn't stop the designer from wiring it up, since Designer.cs
        // is just another part of this same partial class.
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
