using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MyMiningPlugin.Models;

namespace AutoCAD_NET_4_8_Framework
{
    public class UCMineTopologyLoai2 : UserControl
    {
        // callbacks
        private Action<string> _onAddTietDien;
        private Action<TietDienData> _onSelectTietDienPoly;
        private Action<TietDienData> _onDeleteTietDien;
        private Action _onSaveTietDiens;
        private Action _onLoadTietDiens;
        private Action _onAddDoan;
        private Action<DoanDuongLoData> _onSelectDoanPolyline;
        private Action<DoanDuongLoData> _onDeleteDoan;
        private Action _onDelete;

        private MineTopologyLoai2Data _data;

        private const int ContentW = 580;
        private const int RowH_TD = 60;
        private const int RowH_D = 76;

        public UCMineTopologyLoai2() { }

        // -----------------------------------------------------------------------
        public void LoadData(
            MineTopologyLoai2Data data,
            Action<string> onAddTietDien,
            Action<TietDienData> onSelectTietDienPoly,
            Action<TietDienData> onDeleteTietDien,
            Action onSaveTietDiens,
            Action onLoadTietDiens,
            Action onAddDoan,
            Action<DoanDuongLoData> onSelectDoanPolyline,
            Action<DoanDuongLoData> onDeleteDoan,
            Action onDelete,
            Func<List<TietDienData>> getTietDienLibrary)
        {
            _data = data;
            _onAddTietDien = onAddTietDien;
            _onSelectTietDienPoly = onSelectTietDienPoly;
            _onDeleteTietDien = onDeleteTietDien;
            _onSaveTietDiens = onSaveTietDiens;
            _onLoadTietDiens = onLoadTietDiens;
            _onAddDoan = onAddDoan;
            _onSelectDoanPolyline = onSelectDoanPolyline;
            _onDeleteDoan = onDeleteDoan;
            _onDelete = onDelete;

            RebuildUI();
        }

        // -----------------------------------------------------------------------
        public void RebuildUI()
        {
            // Controls.Clear() zeroes out AutoScrollPosition (nothing left to scroll),
            // so without saving/restoring it, every rebuild snaps the view back to the
            // top. Capture it first (it reports negative values, so flip the sign) and
            // reapply it once the new controls are laid out.
            Point savedScroll = new Point(-this.AutoScrollPosition.X, -this.AutoScrollPosition.Y);

            this.SuspendLayout();
            this.Controls.Clear();
            this.AutoScroll = true;
            this.Dock = DockStyle.Fill;
            this.BackColor = SystemColors.Control;

            int x = 14;
            int y = 14;

            // ── Header ──────────────────────────────────────────────────────────
            this.Controls.Add(new Label
            {
                Text = $"Địa hình lò Loại 2:  {_data?.Name}",
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold)
            });
            y += 42;

            // ════════════════════════════════════════════════════════════════════
            // TIẾT DIỆN section
            // ════════════════════════════════════════════════════════════════════
            this.Controls.Add(SectionLabel("Tiết diện", x, y));
            y += 26;

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
            this.Controls.Add(btnAddTD);

            Button btnSaveTD = Btn("Lưu tiết diện", x + 173, y, 165, 34,
                Color.FromArgb(70, 130, 180), Color.White);
            btnSaveTD.Click += (s, e) => _onSaveTietDiens?.Invoke();
            this.Controls.Add(btnSaveTD);

            Button btnLoadTD = Btn("Tải tiết diện", x + 346, y, 165, 34,
                Color.FromArgb(128, 0, 128), Color.White);
            btnLoadTD.Click += (s, e) => _onLoadTietDiens?.Invoke();
            this.Controls.Add(btnLoadTD);
            y += 42;

            foreach (var td in _data?.TietDiens ?? new List<TietDienData>())
            {
                this.Controls.Add(BuildTietDienRow(td, x, y));
                y += RowH_TD + 5;
            }
            y += 12;

            // ════════════════════════════════════════════════════════════════════
            // ĐOẠN ĐƯỜNG LÒ section
            // ════════════════════════════════════════════════════════════════════
            this.Controls.Add(SectionLabel("Đoạn đường lò", x, y));
            y += 26;

            Button btnAddDoan = Btn("+ Thêm đoạn đường lò", x, y, 190, 34,
                Color.SteelBlue, Color.White);
            btnAddDoan.Click += (s, e) => _onAddDoan?.Invoke();
            this.Controls.Add(btnAddDoan);
            y += 42;

            foreach (var doan in _data?.DoanDuongLos ?? new List<DoanDuongLoData>())
            {
                this.Controls.Add(BuildDoanRow(doan, x, y));
                y += RowH_D + 5;
            }
            y += 16;

            // ── Delete entry ────────────────────────────────────────────────────
            Button btnDel = Btn("Xóa địa hình lò này", x, y, 175, 34,
                Color.White, Color.DarkRed);
            btnDel.Click += (s, e) => _onDelete?.Invoke();
            this.Controls.Add(btnDel);

            this.ResumeLayout(true);

            // Reapply the scroll offset now that the content (and scrollable area) exists again.
            this.AutoScrollPosition = savedScroll;
        }

        // -----------------------------------------------------------------------
        private Panel BuildTietDienRow(TietDienData td, int x, int y)
        {
            bool has = td.Polyline != null;
            bool inDwg = has && td.Polyline.CurrentObjectId.HasValue
                             && !td.Polyline.CurrentObjectId.Value.IsNull;

            var row = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(ContentW, RowH_TD),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            row.Controls.Add(new Label
            {
                Text = td.Name,
                Location = new Point(6, 6),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            });

            string status = has
                ? $"{(inDwg ? "✓" : "⚠")} [{td.Polyline.Handle}]  {td.Polyline.Layer}  ({td.Polyline.SourceDwgName})"
                : "(chưa chọn polyline)";
            row.Controls.Add(new Label
            {
                Text = status,
                Location = new Point(6, 32),
                Size = new Size(ContentW - 195, 20),
                ForeColor = has ? (inDwg ? Color.DarkGreen : Color.DarkOrange) : Color.Gray
            });

            var btnPick = new Button
            {
                Text = "Chọn polyline",
                Location = new Point(ContentW - 182, 10),
                Size = new Size(120, 30)
            };
            btnPick.Click += (s, e) => _onSelectTietDienPoly?.Invoke(td);
            row.Controls.Add(btnPick);

            var btnX = new Button
            {
                Text = "Xóa",
                Location = new Point(ContentW - 56, 10),
                Size = new Size(48, 30),
                ForeColor = Color.DarkRed
            };
            btnX.Click += (s, e) => _onDeleteTietDien?.Invoke(td);
            row.Controls.Add(btnX);

            return row;
        }

        // -----------------------------------------------------------------------
        private Panel BuildDoanRow(DoanDuongLoData doan, int x, int y)
        {
            bool has = doan.Polyline != null;
            bool inDwg = has && doan.Polyline.CurrentObjectId.HasValue
                             && !doan.Polyline.CurrentObjectId.Value.IsNull;

            var row = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(ContentW, RowH_D),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            // Name
            row.Controls.Add(new Label
            {
                Text = doan.Name,
                Location = new Point(6, 6),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            });

            // Polyline status
            string polStatus = has
                ? $"{(inDwg ? "✓" : "⚠")} [{doan.Polyline.Handle}]  {doan.Polyline.Layer}"
                : "(chưa chọn đường)";
            row.Controls.Add(new Label
            {
                Text = polStatus,
                Location = new Point(6, 30),
                Size = new Size(250, 20),
                ForeColor = has ? (inDwg ? Color.DarkGreen : Color.DarkOrange) : Color.Gray
            });

            // Tiết diện label + dropdown — sourced from THIS entry's TietDiens list
            row.Controls.Add(new Label
            {
                Text = "Tiết diện:",
                Location = new Point(6, 52),
                AutoSize = true
            });

            var cbTD = new ComboBox
            {
                Location = new Point(76, 50),
                Size = new Size(210, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbTD.Items.Add("(chưa chọn)");
            foreach (var td in _data?.TietDiens ?? new List<TietDienData>())
                cbTD.Items.Add(td.Name);
            int selIdx = string.IsNullOrEmpty(doan.TietDienName) ? 0
                : cbTD.Items.Cast<string>().ToList().IndexOf(doan.TietDienName);
            cbTD.SelectedIndex = selIdx < 0 ? 0 : selIdx;
            cbTD.SelectedIndexChanged += (s, e) =>
                doan.TietDienName = cbTD.SelectedIndex == 0 ? null : cbTD.SelectedItem.ToString();
            row.Controls.Add(cbTD);

            // [Chọn đường]
            var btnSel = new Button
            {
                Text = "Chọn đường",
                Location = new Point(ContentW - 182, 6),
                Size = new Size(120, 30)
            };
            btnSel.Click += (s, e) => _onSelectDoanPolyline?.Invoke(doan);
            row.Controls.Add(btnSel);

            // [Xóa]
            var btnX = new Button
            {
                Text = "Xóa",
                Location = new Point(ContentW - 56, 6),
                Size = new Size(48, 30),
                ForeColor = Color.DarkRed
            };
            btnX.Click += (s, e) => _onDeleteDoan?.Invoke(doan);
            row.Controls.Add(btnX);

            return row;
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
    }
}