using System;
using System.Windows.Forms;
using System.Linq;
using MyMiningPlugin.Models;

namespace AutoCAD_NET_4_8_Framework
{
    public partial class UCSurface : UserControl
    {
        private Action _onSelectLines;
        private Action _onClearLines;
        private Action _onSelectBorderlines; 
        public UCSurface()
        {
            InitializeComponent();
        }

        public void LoadData(SurfaceData surface, bool isDeletable, Action onSelectLines, Action onClearLines, Action onSelectBorderlines = null)
        {
            _onSelectLines = onSelectLines;
            _onClearLines = onClearLines;
            _onSelectBorderlines = onSelectBorderlines;

            // TODO: Update your UI labels here:
            // e.g. labelTitle.Text = surface.Type;

            // TODO: You need a ListBox to show the geometry references.
            listBoxIds.Items.Clear();
            foreach (var geoRef in surface.SelectedGeometry)
            {
                bool inCurrentDwg = geoRef.CurrentObjectId.HasValue && !geoRef.CurrentObjectId.Value.IsNull;
                listBoxIds.Items.Add($"{(inCurrentDwg ? "✓" : "⚠")} [{geoRef.Handle}] {geoRef.Layer}");
            }

            int totalCount = surface.SelectedGeometry.Count;
            int currentCount = surface.SelectedGeometry.Count(g => g.CurrentObjectId.HasValue && !g.CurrentObjectId.Value.IsNull);
            lblLineData.Text = $"Tổng: {totalCount} lines | Trong DWG này: {currentCount} lines";

            if (!isDeletable && btnDeleteLine != null) { btnDeleteLine.Visible = false; }
        }

        // TODO: Map these methods to your Button Click Events in the Designer!
        public void BtnSelectLines_Click(object sender, EventArgs e) => _onSelectLines?.Invoke();
        public void BtnSelectBorderLines_Click(object sender, EventArgs e) => _onSelectBorderlines?.Invoke();
        public void BtnClearLines_Click(object sender, EventArgs e) => _onClearLines?.Invoke();
    }
}
