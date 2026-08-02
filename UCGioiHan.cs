using System;
using System.Windows.Forms;
using System.Linq;
using MyMiningPlugin.Models;

namespace AutoCAD_NET_4_8_Framework
{
    public partial class UCGioiHan : UserControl
    {
        private Action _onAddBlock;
        private Action _onDeleteGioiHan;

        public UCGioiHan()
        {
            InitializeComponent();
        }

        public void LoadData(GioiHanData gioiHan, Action onAddBlock, Action onDeleteGioiHan)
        {
            _onAddBlock       = onAddBlock;
            _onDeleteGioiHan  = onDeleteGioiHan;

            lblHeader.Text = $"Toàn bộ {gioiHan.Name}";
            lblInfo.Text   = $"Số Vùng: {gioiHan.Blocks.Count}\n" +
                             $"Tổng Vách: {gioiHan.Blocks.Sum(b => b.Vach.SelectedGeometry.Count)} lines\n" +
                             $"Tổng Trụ: {gioiHan.Blocks.Sum(b => b.Tru.SelectedGeometry.Count)} lines";
        }

        private void BtnAddBlock_Click(object sender, EventArgs e)      => _onAddBlock?.Invoke();
        private void BtnDeleteGioiHan_Click(object sender, EventArgs e) => _onDeleteGioiHan?.Invoke();
    }
}
