using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MyMiningPlugin.Services;

namespace AutoCAD_NET_4_8_Framework
{
    /// <summary>
    /// Centralized "Lưu trữ dữ liệu" node at the bottom of the tree — an RPG-style save
    /// system replacing the old single always-overwritten file (and the Lưu/Tải buttons
    /// that used to be duplicated on every root category). Each "+ Lưu dự án mới" click
    /// creates one named save file; the list below shows every save file found on disk
    /// so the user can browse, load, overwrite or delete any of them.
    ///
    /// This is also meant as the future home for cross-category bulk operations (mass
    /// export etc.) — anything that acts on "everything in the project" rather than one
    /// tree node, so those don't end up scattered across root categories either.
    /// </summary>
    public class UCSaveManager : UserControl
    {
        private Action<string> _onSaveNew;
        private Func<List<ProjectSaveSlot>> _getSaves;
        private Action<ProjectSaveSlot> _onLoadSlot;
        private Action<ProjectSaveSlot> _onOverwriteSlot;
        private Action<ProjectSaveSlot> _onDeleteSlot;
        private Action _onBrowseLoad;
        private Action _onOpenFolder;
        private string _savesFolderPath;

        private DataGridView _dgvSaves;
        private Label _lblCount;

        private const int ColTen = 0;
        private const int ColNgay = 1;
        private const int ColTai = 2;
        private const int ColGhiDe = 3;
        private const int ColXoa = 4;

        public UCSaveManager() { }

        // -----------------------------------------------------------------------
        public void LoadData(
            string savesFolderPath,
            Action<string> onSaveNew,
            Func<List<ProjectSaveSlot>> getSaves,
            Action<ProjectSaveSlot> onLoadSlot,
            Action<ProjectSaveSlot> onOverwriteSlot,
            Action<ProjectSaveSlot> onDeleteSlot,
            Action onBrowseLoad,
            Action onOpenFolder)
        {
            _savesFolderPath = savesFolderPath;
            _onSaveNew = onSaveNew;
            _getSaves = getSaves;
            _onLoadSlot = onLoadSlot;
            _onOverwriteSlot = onOverwriteSlot;
            _onDeleteSlot = onDeleteSlot;
            _onBrowseLoad = onBrowseLoad;
            _onOpenFolder = onOpenFolder;

            RebuildUI();
        }

        // -----------------------------------------------------------------------
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
                Text = "Lưu trữ dữ liệu dự án",
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold)
            });
            y += 32;

            topPanel.Controls.Add(new Label
            {
                Text = $"Vị trí lưu: {_savesFolderPath}",
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = Color.DimGray
            });
            y += 24;

            _lblCount = new Label
            {
                Text = CountText(),
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = Color.DimGray
            };
            topPanel.Controls.Add(_lblCount);
            y += 30;

            Button btnSaveNew = Btn("+ Lưu dự án mới", x, y, 165, 34,
                Color.FromArgb(34, 139, 34), Color.White);
            btnSaveNew.Click += (s, e) =>
            {
                string suggested = $"Dự án {DateTime.Now:yyyy-MM-dd HH-mm}";
                using (var dlg = new RenameDialog("Tên bản lưu:", suggested))
                {
                    if (dlg.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(dlg.NewName)) return;
                    _onSaveNew?.Invoke(dlg.NewName.Trim());
                }
            };
            topPanel.Controls.Add(btnSaveNew);

            Button btnBrowse = Btn("Tải từ file khác...", x + 173, y, 165, 34,
                Color.FromArgb(70, 130, 180), Color.White);
            btnBrowse.Click += (s, e) => _onBrowseLoad?.Invoke();
            topPanel.Controls.Add(btnBrowse);

            Button btnOpenFolder = Btn("Mở thư mục lưu", x + 346, y, 150, 34,
                SystemColors.ControlLight, Color.Black);
            btnOpenFolder.Click += (s, e) => _onOpenFolder?.Invoke();
            topPanel.Controls.Add(btnOpenFolder);
            y += 42;

            int topHeight = y + 4;

            // ── Save-slot grid — the bottom band, gets all remaining height ──────
            _dgvSaves = BuildSaveGrid();

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, topHeight));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            layout.Controls.Add(topPanel, 0, 0);
            layout.Controls.Add(_dgvSaves, 0, 1);
            this.Controls.Add(layout);

            RefreshList();

            this.ResumeLayout(true);
        }

        // -----------------------------------------------------------------------
        private DataGridView BuildSaveGrid()
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

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTen", HeaderText = "Tên bản lưu", Width = 220, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNgay", HeaderText = "Ngày lưu", Width = 150, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewButtonColumn { Name = "colTai", HeaderText = "", Text = "Tải", UseColumnTextForButtonValue = true, Width = 70 });
            dgv.Columns.Add(new DataGridViewButtonColumn { Name = "colGhiDe", HeaderText = "", Text = "Ghi đè", UseColumnTextForButtonValue = true, Width = 90 });
            dgv.Columns.Add(new DataGridViewButtonColumn { Name = "colXoa", HeaderText = "", Text = "Xóa", UseColumnTextForButtonValue = true, Width = 60 });

            dgv.CellContentClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                var slot = (ProjectSaveSlot)dgv.Rows[e.RowIndex].Tag;
                if (slot == null) return;
                if (e.ColumnIndex == ColTai) _onLoadSlot?.Invoke(slot);
                else if (e.ColumnIndex == ColGhiDe) _onOverwriteSlot?.Invoke(slot);
                else if (e.ColumnIndex == ColXoa) _onDeleteSlot?.Invoke(slot);
            };

            return dgv;
        }

        // -----------------------------------------------------------------------
        // Re-scans the Saves folder and repopulates the grid — called on first build
        // and again after any save/overwrite/delete, since a slot's identity (unlike
        // Đoạn đường lò or Tiết diện) is just a file on disk, not a live object the
        // form keeps around to patch a single row in place.
        public void RefreshList()
        {
            if (_dgvSaves == null) return;
            _dgvSaves.Rows.Clear();
            foreach (var slot in _getSaves?.Invoke() ?? new List<ProjectSaveSlot>())
            {
                int idx = _dgvSaves.Rows.Add();
                var row = _dgvSaves.Rows[idx];
                row.Tag = slot;
                row.Cells[ColTen].Value = slot.Name;
                row.Cells[ColNgay].Value = slot.SavedAt.ToString("yyyy-MM-dd HH:mm");
            }

            if (_lblCount != null) _lblCount.Text = CountText();
        }

        private string CountText()
            => $"Số bản lưu: {_getSaves?.Invoke()?.Count ?? 0}";

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
