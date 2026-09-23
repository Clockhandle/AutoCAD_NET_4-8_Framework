using System.Windows.Forms;

namespace AutoCAD_NET_4_8_Framework
{
    /// <summary>
    /// Minimal single-input dialog used to prompt for a name.
    /// </summary>
    public partial class RenameDialog : Form
    {
        public string NewName => txtName.Text;

        public RenameDialog(string prompt, string defaultValue = null)
        {
            InitializeComponent();

            lblPrompt.Text = prompt;
            txtName.Text = defaultValue ?? "";
            txtName.SelectAll();
        }
    }
}
