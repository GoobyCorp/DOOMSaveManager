using System;
using System.Windows.Forms;

using System.IO;

namespace DOOMSaveManager
{
    public partial class TransferForm : Form
    {
        public TransferForm() {
            InitializeComponent();
        }

        private void TransferForm_Load(object sender, EventArgs e) {
            srcComboBox.Items.AddRange(DoomEternal.GetUserIDs());
            if (srcComboBox.Items.Count == 0) {
                DialogResult = DialogResult.Abort;
            } else if(srcComboBox.Items.Count > 0) {
                srcComboBox.SelectedIndex = 0;
            }
            if (Directory.Exists(Path.Combine(DoomEternal.SavePath, "savegame.unencrypted"))) {
                srcComboBox.Items.Add("savegame.unencrypted");
            }
        }

        private void transferOkBtn_Click(object sender, EventArgs e) {
            bool res = true;
            if (srcComboBox.Text != "savegame.unencrypted" && !Utilities.CheckUUID(srcComboBox.Text)) {
                res = false;
                MessageBox.Show("Invalid source UUID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!Utilities.CheckUUID(dstUidBox.Text)) {
                res = false;
                MessageBox.Show("Invalid destination UUID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!Directory.Exists(Path.Combine(DoomEternal.SavePath, srcComboBox.Text))) {
                res = false;
                MessageBox.Show("Source directory doesn't exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (res) {
                DialogResult = DialogResult.OK;
            }
        }

        private void transferCancelBtn_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }
    }
}
