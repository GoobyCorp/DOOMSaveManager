using System;
using System.Windows.Forms;

using System.IO;

namespace DOOMSaveManager
{
    public partial class TransferForm : Form
    {
        private string[] uids = { };

        public TransferForm() {
            InitializeComponent();
        }

        private void TransferForm_Load(object sender, EventArgs e) {
            uids = DoomEternal.GetUserIDs();

            if (Directory.Exists(Path.Combine(DoomEternal.SavePath, "savegame.unencrypted")))
                srcComboBox.Items.Add("savegame.unencrypted");
            srcComboBox.Items.AddRange(uids);
            dstComboBox.Items.AddRange(uids);
            dstComboBox.Items.Add("savegame.unencrypted");

            if (srcComboBox.Items.Count > 0)
                srcComboBox.SelectedIndex = 0;
            if (dstComboBox.Items.Count > 0)
                dstComboBox.SelectedIndex = 0;
        }

        private void transferOkBtn_Click(object sender, EventArgs e) {
            bool res = true;
            if (srcComboBox.Text != "savegame.unencrypted" && !Utilities.CheckUUID(srcComboBox.Text)) {
                res = false;
                MessageBox.Show("Invalid source UUID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (dstComboBox.Text != "savegame.unencrypted" && Utilities.CheckUUID(dstComboBox.Text)) {
                res = false;
                MessageBox.Show("Invalid destination UUID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!Directory.Exists(Path.Combine(DoomEternal.SavePath, srcComboBox.Text))) {
                res = false;
                MessageBox.Show("Source directory doesn't exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!Directory.Exists(Path.Combine(DoomEternal.SavePath, dstComboBox.Text)))
                Directory.CreateDirectory(Path.Combine(DoomEternal.SavePath, dstComboBox.Text));
            if (res)
                DialogResult = DialogResult.OK;
        }

        private void transferCancelBtn_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }

        private void srcComboBox_SelectedValueChanged(object sender, EventArgs e) {
            dstComboBox.Items.Clear();
            dstComboBox.Items.AddRange(uids);
            dstComboBox.Items.Add("savegame.unencrypted");
            dstComboBox.Items.Remove(((ComboBox)sender).Text);
            if (dstComboBox.Items.Count > 0)
                dstComboBox.SelectedIndex = 0;
        }

        private void dstComboBox_SelectedValueChanged(object sender, EventArgs e) {

        }
    }
}
