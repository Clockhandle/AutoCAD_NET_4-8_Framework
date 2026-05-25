using System;
using System.Windows.Forms;
using MyMiningPlugin.Models;

namespace AutoCAD_NET_4_8_Framework
{
    public partial class UCBeMat : UserControl
    {
        private Action _onSelectPoints;
        private Action _onClearPoints;
        private Action _onDelete;

        public UCBeMat()
        {
            InitializeComponent();
        }

        public void LoadData(BeMatData bemat, Action onSelectPoints, Action onClearPoints, Action onDelete)
        {
            _onSelectPoints = onSelectPoints;
            _onClearPoints = onClearPoints;
            _onDelete = onDelete;

            lblHeader.Text = $"B? m?t: {bemat.Name}";
            lblPointCount.Text = $"S? ?i?m: {bemat.Points.Count}";

            listBoxPoints.BeginUpdate();
            listBoxPoints.Items.Clear();
            foreach (var pt in bemat.Points)
            {
                listBoxPoints.Items.Add($"[{pt.Handle}] {pt.Layer}  X={pt.X:F2}  Y={pt.Y:F2}  Z={pt.Z:F2}");
            }
            listBoxPoints.EndUpdate();
        }

        // TODO: Wire these to button Click events in the Designer
        public void BtnSelectPoints_Click(object sender, EventArgs e) => _onSelectPoints?.Invoke();
        public void BtnClearPoints_Click(object sender, EventArgs e) => _onClearPoints?.Invoke();
        public void BtnDelete_Click(object sender, EventArgs e) => _onDelete?.Invoke();
    }
}
