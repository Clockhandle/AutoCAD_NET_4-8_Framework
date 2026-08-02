using System;
using System.Windows.Forms;
using System.Linq;
using MyMiningPlugin.Models;

namespace AutoCAD_NET_4_8_Framework
{
    public partial class UCSurface : UserControl
    {
        private Action _onSelectLines;
        private Action _onClearSurfaceLines;
        private Action _onSelectBoundaryLines;
        private Action _onClearBoundaryLines;
        private Action _onSelectHoles;
        private Action _onClearHoleLines;
        private Action _onSelectBreakLines;
        private Action _onClearBreakLines;

        public UCSurface()
        {
            InitializeComponent();
            btnAddHoleLine.Click += new System.EventHandler(BtnAddHole_Click);
            btnDeleteHoleLine.Click += new System.EventHandler(BtnDeleteHoleLines_Click);
        }

        public void LoadData(SurfaceData surface, bool isDeletable,
            Action onSelectLines,
            Action onClearSurfaceLines,
            Action onClearBoundaryLines,
            Action onSelectBoundaryLines = null,
            Action onSelectHoles = null,
            Action onClearHoleLines = null,
            Action onSelectBreakLines = null,
            Action onClearBreakLines = null)
        {
            _onSelectLines = onSelectLines;
            _onClearSurfaceLines = onClearSurfaceLines;
            _onClearBoundaryLines = onClearBoundaryLines;
            _onSelectBoundaryLines = onSelectBoundaryLines;
            _onSelectHoles = onSelectHoles;
            _onClearHoleLines = onClearHoleLines;
            _onSelectBreakLines = onSelectBreakLines;
            _onClearBreakLines = onClearBreakLines;

            bool isPointMode = surface.Type == "Bề mặt";

            btnAddLine.Text = isPointMode ? "Chọn thêm điểm" : "Chọn thêm đường";
            btnDeleteSurfaceLines.Text = isPointMode ? "Xóa điểm" : $"Xóa đường {surface.Type.ToLower()}";

            // Populate the main ListBox
            listBoxIds.BeginUpdate();
            listBoxIds.Items.Clear();
            const int maxDisplayItems = 500;
            if (isPointMode && surface.SelectedGeometry.Count > maxDisplayItems)
            {
                // For large point clouds, only show a summary to avoid UI freeze
                listBoxIds.Items.Add($"[Hiển thị {maxDisplayItems} / {surface.SelectedGeometry.Count} điểm]");
                foreach (var geoRef in surface.SelectedGeometry.Take(maxDisplayItems))
                {
                    bool inCurrentDwg = geoRef.CurrentObjectId.HasValue && !geoRef.CurrentObjectId.Value.IsNull;
                    listBoxIds.Items.Add($"{(inCurrentDwg ? "✓" : "⚠")} [{geoRef.Handle}] {geoRef.Layer}");
                }
            }
            else
            {
                foreach (var geoRef in surface.SelectedGeometry)
                {
                    bool inCurrentDwg = geoRef.CurrentObjectId.HasValue && !geoRef.CurrentObjectId.Value.IsNull;
                    listBoxIds.Items.Add($"{(inCurrentDwg ? "✓" : "⚠")} [{geoRef.Handle}] {geoRef.Layer}");
                }
            }
            listBoxIds.EndUpdate();

            // Populate the Boundary Lines ListBox
            if (listBoundaryLineId != null)
            {
                listBoundaryLineId.BeginUpdate();
                listBoundaryLineId.Items.Clear();
                if (surface.BoundaryGeometry != null)
                {
                    foreach (var geoRef in surface.BoundaryGeometry)
                    {
                        bool inCurrentDwg = geoRef.CurrentObjectId.HasValue && !geoRef.CurrentObjectId.Value.IsNull;
                        listBoundaryLineId.Items.Add($"{(inCurrentDwg ? "✓" : "⚠")} [{geoRef.Handle}] {geoRef.Layer}");
                    }
                }
                listBoundaryLineId.EndUpdate();
            }

            // Populate the Hole Lines ListBox
            if (listHoleLineId != null)
            {
                listHoleLineId.BeginUpdate();
                listHoleLineId.Items.Clear();
                if (surface.HoleGeometry != null)
                {
                    foreach (var geoRef in surface.HoleGeometry)
                    {
                        bool inCurrentDwg = geoRef.CurrentObjectId.HasValue && !geoRef.CurrentObjectId.Value.IsNull;
                        listHoleLineId.Items.Add($"{(inCurrentDwg ? "✓" : "⚠")} [{geoRef.Handle}] {geoRef.Layer}");
                    }
                }
                listHoleLineId.EndUpdate();
            }

            // Populate the Breakline ListBox
            if (listBreakLineId != null)
            {
                listBreakLineId.BeginUpdate();
                listBreakLineId.Items.Clear();
                if (surface.BreaklineGeometry != null)
                {
                    foreach (var geoRef in surface.BreaklineGeometry)
                    {
                        bool inCurrentDwg = geoRef.CurrentObjectId.HasValue && !geoRef.CurrentObjectId.Value.IsNull;
                        listBreakLineId.Items.Add($"{(inCurrentDwg ? "✓" : "⚠")} [{geoRef.Handle}] {geoRef.Layer}");
                    }
                }
                listBreakLineId.EndUpdate();
            }

            int totalCount = surface.SelectedGeometry.Count;
            int currentCount = surface.SelectedGeometry.Count(g => g.CurrentObjectId.HasValue && !g.CurrentObjectId.Value.IsNull);
            lblLineData.Text = isPointMode
                ? $"Tổng: {totalCount} điểm | Trong DWG này: {currentCount} điểm"
                : $"Tổng: {totalCount} lines | Trong DWG này: {currentCount} lines";
        }

        public void BtnSelectLines_Click(object sender, EventArgs e) => _onSelectLines?.Invoke();
        public void btnDeleteSurfaceLines_Click(object sender, EventArgs e) => _onClearSurfaceLines?.Invoke();
        public void btnDeleteBoundaryLines_Click(object sender, EventArgs e) => _onClearBoundaryLines?.Invoke();
        public void BtnAddBoundaryLine_Click(object sender, EventArgs e) => _onSelectBoundaryLines?.Invoke();
        public void BtnAddHole_Click(object sender, EventArgs e) => _onSelectHoles?.Invoke();
        public void BtnDeleteHoleLines_Click(object sender, EventArgs e) => _onClearHoleLines?.Invoke();
        private void btnAddBreakLine_Click(object sender, EventArgs e) => _onSelectBreakLines?.Invoke();
        private void btnDeleteBreakLines_Click(object sender, EventArgs e) => _onClearBreakLines?.Invoke();

        /// <summary>
        /// Hides boundary, hole and breakline sections so only the main
        /// polyline list and its select/clear buttons are visible.
        /// Used for surfaces that only accept a single polyline set (e.g. Đứt gãy in a khối).
        /// </summary>
        public void HideExtraSections()
        {
            listBoundaryLineId.Visible = false;
            btnAddBoundaryLine.Visible = false;
            btnDeleteBoundaryLines.Visible = false;
            listHoleLineId.Visible = false;
            btnAddHoleLine.Visible = false;
            btnDeleteHoleLine.Visible = false;
            listBreakLineId.Visible = false;
            btnAddBreakLine.Visible = false;
            btnDeleteBreakLine.Visible = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
