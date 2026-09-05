using System;
using System.Linq;
using System.Windows.Forms;
using MyMiningPlugin.Models;

namespace AutoCAD_NET_4_8_Framework
{
    public partial class UCBoreholes : UserControl
    {
        private Action _onSelectExcel;
        private Action _onParseExcel;
        private Action _onDelete;

        public UCBoreholes()
        {
            InitializeComponent();
        }

        public void LoadData(BoreholeData borehole, Action onSelectExcel, Action onParseExcel, Action onDelete)
        {
            _onSelectExcel = onSelectExcel;
            _onParseExcel = onParseExcel;
            _onDelete = onDelete;

            // TODO: In your Designer, you should have labels/textboxes to display the data. Add them here:
            lblHeader.Text = borehole.Name;
            txtBoxExcelPath.Text = borehole.ExcelFilePath;
            if (borehole.ImportedBoreholes != null && borehole.ImportedBoreholes.Count > 0)
            {
                int intervalCount = borehole.ImportedBoreholes.Sum(b => b.Intervals.Count);
                int trajCount = borehole.ImportedBoreholes.Sum(b => b.Trajectory.Count);
                lblDataSummary.Text = $"Đã đọc {borehole.ImportedBoreholes.Count} lỗ khoan từ Excel " +
                    $"({intervalCount} khoảng vỉa, {trajCount} điểm đo)";
            }
            else
            {
                lblDataSummary.Text = (borehole.Intervals.Count > 0 || borehole.Trajectory.Count > 0)
                    ? $"Đã đọc: {borehole.Intervals.Count} khoảng vỉa, {borehole.Trajectory.Count} điểm đo"
                    : "Chưa đọc dữ liệu từ Excel";
            }
        }

        // TODO: In the Visual Studio Designer, select your corresponding buttons, 
        // click the "Events" lightning bolt, and point their "Click" events to these methods!
        public void BtnSelectExcel_Click(object sender, EventArgs e) => _onSelectExcel?.Invoke();
        public void BtnParseExcel_Click(object sender, EventArgs e) => _onParseExcel?.Invoke();
        public void BtnDeleteBorehole_Click(object sender, EventArgs e) => _onDelete?.Invoke();
    }
}
