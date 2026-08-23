using System;
using System;
using System.Windows.Forms;

namespace AutoCAD_NET_4_8_Framework
{
    public partial class UCRootCategory : UserControl
    {
        private Action _onAddNew;
        private Action _onAddNew2;
        private Action _onSendToServer;
        private Action _onExportJson;
        private Action _onSaveProject;
        private Action _onLoadProject;
        private Action _onRunDQ;
        private Action _onClearMarkers;
        private bool   _markersActive = false;

        public UCRootCategory()
        {
            InitializeComponent();
        }

        public void LoadData(
            string title,
            string addBtnText,
            Action onAddNew,
            Action onSendToServer,
            Action onExportJson,
            Action onSaveProject,
            Action onLoadProject,
            Action onRunDQ = null,
            Action onClearMarkers = null,
            Action onAddNew2 = null,
            string addBtn2Text = null)
        {
            lblHeader.Text = title;
            btnAdd.Text    = addBtnText;

            _onAddNew       = onAddNew;
            _onAddNew2      = onAddNew2;
            _onSendToServer = onSendToServer;
            _onExportJson   = onExportJson;
            _onSaveProject  = onSaveProject;
            _onLoadProject  = onLoadProject;
            _onRunDQ        = onRunDQ;
            _onClearMarkers = onClearMarkers;

            btnClearMarkers.Visible = onRunDQ != null;
            btnClearMarkers.Enabled = onRunDQ != null;
            SetMarkersActive(false);

            if (onAddNew2 != null)
            {
                btnAdd2.Text    = addBtn2Text ?? "+ Thêm mới (Loại 2)";
                btnAdd2.Visible = true;
                // Shift the other buttons down to make room
                int shift = btnAdd2.Height + 6;
                btnSendServer.Top  += shift;
                btnExportJson.Top  += shift;
                btnSaveProject.Top += shift;
                btnLoadProject.Top += shift;
                btnClearMarkers.Top += shift;
            }
            else
            {
                btnAdd2.Visible = false;
            }
        }

        /// <summary>
        /// Called by the form after DQ check finishes to toggle button text.
        /// </summary>
        public void SetMarkersActive(bool active)
        {
            _markersActive = active;
            btnClearMarkers.Text = active ? "Xóa marker DQ" : "Kiểm tra DQ";
        }

        private void btnAdd_Click(object sender, EventArgs e)  => _onAddNew?.Invoke();
        private void btnAdd2_Click(object sender, EventArgs e) => _onAddNew2?.Invoke();
        private void btnSendToServer_Click(object sender, EventArgs e) => _onSendToServer?.Invoke();
        private void btnExportJson_Click(object sender, EventArgs e)   => _onExportJson?.Invoke();
        private void btnSaveProject_Click(object sender, EventArgs e)  => _onSaveProject?.Invoke();
        private void btnLoadProject_Click(object sender, EventArgs e)  => _onLoadProject?.Invoke();

        private void btnClearMarkers_Click(object sender, EventArgs e)
        {
            if (_markersActive)
                _onClearMarkers?.Invoke();
            else
                _onRunDQ?.Invoke();
        }
    }
}

