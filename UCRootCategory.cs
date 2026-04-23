using System;
using System.Windows.Forms;

namespace AutoCAD_NET_4_8_Framework
{
    public partial class UCRootCategory : UserControl
    {
        // Private fields to hold the actions passed from the main form
        private Action _onAddNew;
        private Action _onSendToServer;
        private Action _onExportJson;
        private Action _onSaveProject;
        private Action _onLoadProject;

        public UCRootCategory()
        {
            InitializeComponent();
        }

        // Call this method from the Main Form to configure the control
        public void LoadData(
            string title, 
            string addBtnText, 
            Action onAddNew, 
            Action onSendToServer, 
            Action onExportJson, 
            Action onSaveProject, 
            Action onLoadProject)
        {
            // 1. Update the UI text (Make sure you named your controls in the Designer like this)
            lblHeader.Text = title;
            btnAdd.Text = addBtnText;

            // 2. Store the business logic callbacks
            _onAddNew = onAddNew;
            _onSendToServer = onSendToServer;
            _onExportJson = onExportJson;
            _onSaveProject = onSaveProject;
            _onLoadProject = onLoadProject;
        }

        // --- Wire these up to the Click events of the buttons in your Designer ---

        private void btnAdd_Click(object sender, EventArgs e) => _onAddNew?.Invoke();
        
        private void btnSendToServer_Click(object sender, EventArgs e) => _onSendToServer?.Invoke();
        
        private void btnExportJson_Click(object sender, EventArgs e) => _onExportJson?.Invoke();
        
        private void btnSaveProject_Click(object sender, EventArgs e) => _onSaveProject?.Invoke();
        
        private void btnLoadProject_Click(object sender, EventArgs e) => _onLoadProject?.Invoke();
    }
}
