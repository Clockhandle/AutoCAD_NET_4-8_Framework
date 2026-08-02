using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MyMiningPlugin.Models;

namespace AutoCAD_NET_4_8_Framework
{
    public partial class UCMineTopology : UserControl
    {
        private Action _onSelectNen;
        private Action _onClearNen;
        private Action _onSelectNoc;
        private Action _onClearNoc;
        private Action _onSelectBien;
        private Action _onClearBien;

        public UCMineTopology()
        {
            InitializeComponent();
        }

        public void LoadData(MineTopologyData topology,
            Action onSelectNen, Action onClearNen,
            Action onSelectNoc, Action onClearNoc,
            Action onSelectBien, Action onClearBien)
        {
            _onSelectNen  = onSelectNen;
            _onClearNen   = onClearNen;
            _onSelectNoc  = onSelectNoc;
            _onClearNoc   = onClearNoc;
            _onSelectBien = onSelectBien;
            _onClearBien  = onClearBien;

            RefreshList(listNen,  topology.Nen,  lblNen,  "Nền");
            RefreshList(listNoc,  topology.Noc,  lblNoc,  "Nóc");
            RefreshList(listBien, topology.Bien, lblBien, "Biên");
        }

        private static void RefreshList(ListBox lb, List<GeometryReference> refs, Label lbl, string name)
        {
            lb.BeginUpdate();
            lb.Items.Clear();
            foreach (var g in refs)
            {
                bool inDwg = g.CurrentObjectId.HasValue && !g.CurrentObjectId.Value.IsNull;
                lb.Items.Add($"{(inDwg ? "✓" : "⚠")} [{g.Handle}] {g.Layer}");
            }
            lb.EndUpdate();
            lbl.Text = $"{name}: {refs.Count} đường";
        }

        private void btnSelectNen_Click(object sender, EventArgs e)  => _onSelectNen?.Invoke();
        private void btnClearNen_Click(object sender, EventArgs e)   => _onClearNen?.Invoke();
        private void btnSelectNoc_Click(object sender, EventArgs e)  => _onSelectNoc?.Invoke();
        private void btnClearNoc_Click(object sender, EventArgs e)   => _onClearNoc?.Invoke();
        private void btnSelectBien_Click(object sender, EventArgs e) => _onSelectBien?.Invoke();
        private void btnClearBien_Click(object sender, EventArgs e)  => _onClearBien?.Invoke();
    }
}
