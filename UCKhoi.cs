using System;
using System.Windows.Forms;
using MyMiningPlugin.Models;

namespace AutoCAD_NET_4_8_Framework
{
    public partial class UCKhoi : UserControl
    {
        private Action _onDeleteKhoi;

        public UCKhoi()
        {
            InitializeComponent();
        }

        public void LoadData(KhoiData khoi, Action onDeleteKhoi)
        {
            _onDeleteKhoi = onDeleteKhoi;

            lblHeader.Text = khoi.Name;
            int dutGayCount = khoi.DutGay?.SelectedGeometry.Count ?? 0;
            lblnfoText.Text = $"Vách: {khoi.Vach.SelectedGeometry.Count} lines\nTrụ: {khoi.Tru.SelectedGeometry.Count} lines\nĐứt gãy: {dutGayCount} lines";
        }

        // TODO: Map this method to your "Xóa Khối" Button's Click Event in the Designer!
        public void BtnDeleteKhoi_Click(object sender, EventArgs e) => _onDeleteKhoi?.Invoke();
    }
}
