using System;
using System.Windows.Forms;
using System.Linq;
using MyMiningPlugin.Models;

namespace AutoCAD_NET_4_8_Framework
{
    public partial class UCVia : UserControl
    {
        private Action _onAddBlock;
        private Action _onDeleteVia;

        public UCVia()
        {
            InitializeComponent();
        }

        public void LoadData(ViaData via, Action onAddBlock, Action onDeleteVia)
        {
            _onAddBlock = onAddBlock;
            _onDeleteVia = onDeleteVia;

            lblHeader.Text = $"Toàn bộ {via.Name}";
            lblInfo.Text = $"Số Khối: {via.Blocks.Count}\nTổng Vách: {via.Blocks.Sum(b => b.Vach.SelectedGeometry.Count)} lines\nTổng Trụ: {via.Blocks.Sum(b => b.Tru.SelectedGeometry.Count)} lines\nTổng Đứt gãy: {via.Blocks.Sum(b => b.DutGay?.SelectedGeometry.Count ?? 0)} lines";
        }

        // TODO: Map these methods to your Button Click Events in the Designer!
        public void BtnAddBlock_Click(object sender, EventArgs e) => _onAddBlock?.Invoke();
        public void BtnDeleteVia_Click(object sender, EventArgs e) => _onDeleteVia?.Invoke();
    }
}
