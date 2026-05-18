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
        public UCSurface()
        {
            InitializeComponent();
        }

        public void LoadData(SurfaceData surface, bool isDeletable, Action onSelectLines, Action onClearSurfaceLines, Action onClearBoundaryLines, Action onSelectBorderlines = null)
        {
            _onSelectLines = onSelectLines;
            _onClearSurfaceLines = onClearSurfaceLines;
            _onClearBoundaryLines = onClearBoundaryLines;
            _onSelectBorderlines = onSelectBorderlines;

            // TODO: Update your UI labels here:
            // e.g. labelTitle.Text = surface.Type;

            // TODO: You need a ListBox to show the geometry references.
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

            int totalCount = surface.SelectedGeometry.Count;
            int currentCount = surface.SelectedGeometry.Count(g => g.CurrentObjectId.HasValue && !g.CurrentObjectId.Value.IsNull);
            lblLineData.Text = $"Tổng: {totalCount} lines | Trong DWG này: {currentCount} lines";
        }

        // TODO: Map these methods to your Button Click Events in the Designer!
        public void BtnSelectLines_Click(object sender, EventArgs e) => _onSelectLines?.Invoke();
        public void btnDeleteSurfaceLines_Click(object sender, EventArgs e) => _onClearSurfaceLines?.Invoke();
        public void btnDeleteBoundaryLines_Click(object sender, EventArgs e) => _onClearBoundaryLines?.Invoke();
        public void BtnAddBoundaryLine_Click(object sender, EventArgs e) => _onSelectBorderlines?.Invoke();

    }
}
