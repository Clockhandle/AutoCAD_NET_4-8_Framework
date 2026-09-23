using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MyMiningPlugin.Services;

namespace AutoCAD_NET_4_8_Framework
{
    /// <summary>
    /// Centralized "Quản lý dữ liệu" node at the bottom of the tree — the general home for
    /// functions that act on everything in the project rather than one tree branch, so
    /// those don't end up scattered across root categories. Currently holds:
    ///  - the RPG-style save-slot system replacing the old single always-overwritten file
    ///    (and the Lưu/Tải buttons that used to be duplicated on every root category). Each
    ///    "+ Lưu dự án mới" click creates one named save file; the list below shows every
    ///    save file found on disk so the user can browse, load, overwrite or delete any of them.
    ///  - "Gửi dữ liệu mới lên server" (mass send), the cross-category counterpart to each
    ///    category's own "Gửi lên server" — see MiningManagerDForm.ShowMassSendDialog.
    ///  - the "..." button next to the save-location path lets the user point future
    ///    saves at a different folder — see MiningManagerDForm.ChangeSavesFolder /
    ///    PersistenceService.SetSavesFolder.
    /// </summary>
    public partial class UCSaveManager : UserControl
    {
        private Action<string> _onSaveNew;
        private Func<List<ProjectSaveSlot>> _getSaves;
        private Action<ProjectSaveSlot> _onLoadSlot;
        private Action<ProjectSaveSlot> _onOverwriteSlot;
        private Action<ProjectSaveSlot> _onDeleteSlot;
        private Action _onBrowseLoad;
        private Action _onOpenFolder;
        private Action _onMassSend;
        private Action _onChangeFolder;
        private string _savesFolderPath;

        private const int ColTen = 0;
        private const int ColNgay = 1;
        private const int ColTai = 2;
        private const int ColGhiDe = 3;
        private const int ColXoa = 4;

        public UCSaveManager()
        {
            InitializeComponent();
        }

        // Note: this can be called more than once on the same instance — e.g.
        // ChangeSavesFolder re-calls it after switching the saves folder — so it must
        // fully refresh every displayed value, not just set things up once.
        public void LoadData(
            string savesFolderPath,
            Action<string> onSaveNew,
            Func<List<ProjectSaveSlot>> getSaves,
            Action<ProjectSaveSlot> onLoadSlot,
            Action<ProjectSaveSlot> onOverwriteSlot,
            Action<ProjectSaveSlot> onDeleteSlot,
            Action onBrowseLoad,
            Action onOpenFolder,
            Action onMassSend,
            Action onChangeFolder)
        {
            _savesFolderPath = savesFolderPath;
            _onSaveNew = onSaveNew;
            _getSaves = getSaves;
            _onLoadSlot = onLoadSlot;
            _onOverwriteSlot = onOverwriteSlot;
            _onDeleteSlot = onDeleteSlot;
            _onBrowseLoad = onBrowseLoad;
            _onOpenFolder = onOpenFolder;
            _onMassSend = onMassSend;
            _onChangeFolder = onChangeFolder;

            txtFolder.Text = _savesFolderPath;
            RefreshList();
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e) => _onChangeFolder?.Invoke();

        private void btnSaveNew_Click(object sender, EventArgs e)
        {
            string suggested = $"Dự án {DateTime.Now:yyyy-MM-dd HH-mm}";
            using (var dlg = new RenameDialog("Tên bản lưu:", suggested))
            {
                if (dlg.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(dlg.NewName)) return;
                _onSaveNew?.Invoke(dlg.NewName.Trim());
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e) => _onBrowseLoad?.Invoke();

        private void btnOpenFolder_Click(object sender, EventArgs e) => _onOpenFolder?.Invoke();

        private void btnMassSend_Click(object sender, EventArgs e) => _onMassSend?.Invoke();

        private void dgvSaves_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var slot = (ProjectSaveSlot)dgvSaves.Rows[e.RowIndex].Tag;
            if (slot == null) return;
            if (e.ColumnIndex == ColTai) _onLoadSlot?.Invoke(slot);
            else if (e.ColumnIndex == ColGhiDe) _onOverwriteSlot?.Invoke(slot);
            else if (e.ColumnIndex == ColXoa) _onDeleteSlot?.Invoke(slot);
        }

        // Re-scans the Saves folder and repopulates the grid — called on first build
        // and again after any save/overwrite/delete, since a slot's identity (unlike
        // Đoạn đường lò or Tiết diện) is just a file on disk, not a live object the
        // form keeps around to patch a single row in place.
        public void RefreshList()
        {
            dgvSaves.Rows.Clear();
            foreach (var slot in _getSaves?.Invoke() ?? new List<ProjectSaveSlot>())
            {
                int idx = dgvSaves.Rows.Add();
                var row = dgvSaves.Rows[idx];
                row.Tag = slot;
                row.Cells[ColTen].Value = slot.Name;
                row.Cells[ColNgay].Value = slot.SavedAt.ToString("yyyy-MM-dd HH:mm");
            }

            lblCount.Text = CountText();
        }

        private string CountText()
            => $"Số bản lưu: {_getSaves?.Invoke()?.Count ?? 0}";
    }
}
