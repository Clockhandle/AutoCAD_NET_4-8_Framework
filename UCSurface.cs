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
        private Action _onClearBoundaryLines;
        private Action _onSelectBorderlines;
        private Action _onSelectHoles;
        private Action _onClearHoleLines;

        public UCSurface()
        {
            InitializeComponent();
            btnAddGapingBound.Click += new System.EventHandler(BtnAddHole_Click);
            btnDeleteGapingBound.Click += new System.EventHandler(BtnDeleteHoleLines_Click);
        }

        public void LoadData(SurfaceData surface, bool isDeletable,
            Action onSelectLines,
            Action onClearSurfaceLines,
            Action onClearBoundaryLines,
            Action onSelectBorderlines = null,
            Action onSelectHoles = null,
            Action onClearHoleLines = null)
        {
            _onSelectLines = onSelectLines;
            _onClearSurfaceLines = onClearSurfaceLines;
            _onClearBoundaryLines = onClearBoundaryLines;
            _onSelectBorderlines = onSelectBorderlines;
            _onSelectHoles = onSelectHoles;
            _onClearHoleLines = onClearHoleLines;

            // Populate the Surface Lines ListBox
            listBoxIds.BeginUpdate();
            listBoxIds.Items.Clear();
            foreach (var geoRef in surface.SelectedGeometry)
            {
                bool inCurrentDwg = geoRef.CurrentObjectId.HasValue && !geoRef.CurrentObjectId.Value.IsNull;
                listBoxIds.Items.Add($"{(inCurrentDwg ? "✓" : "⚠")} [{geoRef.Handle}] {geoRef.Layer}");
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
            if (listGapingBoundId != null)
            {
                listGapingBoundId.BeginUpdate();
                listGapingBoundId.Items.Clear();
                if (surface.HoleGeometry != null)
                {
                    foreach (var geoRef in surface.HoleGeometry)
                    {
                        bool inCurrentDwg = geoRef.CurrentObjectId.HasValue && !geoRef.CurrentObjectId.Value.IsNull;
                        listGapingBoundId.Items.Add($"{(inCurrentDwg ? "✓" : "⚠")} [{geoRef.Handle}] {geoRef.Layer}");
                    }
                }
                listGapingBoundId.EndUpdate();
            }

            int totalCount = surface.SelectedGeometry.Count;
            int currentCount = surface.SelectedGeometry.Count(g => g.CurrentObjectId.HasValue && !g.CurrentObjectId.Value.IsNull);
            lblLineData.Text = $"Tổng: {totalCount} lines | Trong DWG này: {currentCount} lines";
        }

        public void BtnSelectLines_Click(object sender, EventArgs e) => _onSelectLines?.Invoke();
        public void btnDeleteSurfaceLines_Click(object sender, EventArgs e) => _onClearSurfaceLines?.Invoke();
        public void btnDeleteBoundaryLines_Click(object sender, EventArgs e) => _onClearBoundaryLines?.Invoke();
        public void BtnAddBoundaryLine_Click(object sender, EventArgs e) => _onSelectBorderlines?.Invoke();
        public void BtnAddHole_Click(object sender, EventArgs e) => _onSelectHoles?.Invoke();
        public void BtnDeleteHoleLines_Click(object sender, EventArgs e) => _onClearHoleLines?.Invoke();
    }
}
