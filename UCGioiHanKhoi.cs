using System;
using System.Windows.Forms;
using MyMiningPlugin.Models;

namespace AutoCAD_NET_4_8_Framework
{
    public partial class UCGioiHanKhoi : UserControl
    {
        private Action _onDeleteKhoi;

        public UCGioiHanKhoi()
        {
            InitializeComponent();
        }

        public void LoadData(GioiHanKhoiData khoi, Action onDeleteKhoi)
        {
            _onDeleteKhoi = onDeleteKhoi;

            lblHeader.Text   = khoi.Name;
            lblInfoText.Text = $"Vách: {khoi.Vach.SelectedGeometry.Count} lines\n" +
                               $"Trụ: {khoi.Tru.SelectedGeometry.Count} lines";
        }

        private void BtnDeleteKhoi_Click(object sender, EventArgs e) => _onDeleteKhoi?.Invoke();
    }
}
