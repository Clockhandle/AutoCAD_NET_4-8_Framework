using System.Drawing;
using System.Windows.Forms;

namespace AutoCAD_NET_4_8_Framework
{
    /// <summary>
    /// Minimal single-input dialog used to prompt for a name.
    /// </summary>
    public class RenameDialog : Form
    {
        private TextBox _txt;
        public string NewName => _txt.Text;

        public RenameDialog(string prompt)
        {
            this.Text = "Nhập tên";
            this.Size = new Size(320, 130);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lbl = new Label { Text = prompt, Location = new Point(12, 12), AutoSize = true };
            this.Controls.Add(lbl);

            _txt = new TextBox { Location = new Point(12, 32), Size = new Size(278, 22) };
            this.Controls.Add(_txt);

            var btnOK = new Button
            {
                Text = "OK", DialogResult = DialogResult.OK,
                Location = new Point(130, 62), Size = new Size(75, 26)
            };
            var btnCancel = new Button
            {
                Text = "Hủy", DialogResult = DialogResult.Cancel,
                Location = new Point(215, 62), Size = new Size(75, 26)
            };

            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    }
}
