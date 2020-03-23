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
            srcUidComboBox.Items.AddRange(DoomEternal.GetUserIDs());
            if (srcUidComboBox.Items.Count == 0) {
                DialogResult = DialogResult.Abort;
            } else if(srcUidComboBox.Items.Count > 0) {
                srcUidComboBox.SelectedIndex = 0;
            }
        }

        private void transferOkBtn_Click(object sender, EventArgs e) {
            bool res = true;
            if (!Utilities.CheckUUID(srcUidComboBox.Text)) {
                res = false;
                MessageBox.Show("Invalid source UUID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!Utilities.CheckUUID(dstUidBox.Text)) {
                res = false;
                MessageBox.Show("Invalid destination UUID!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!Directory.Exists(Path.Combine(DoomEternal.SavePath, srcUidComboBox.Text))) {
                res = false;
                MessageBox.Show("Source UUID directory doesn't exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //if (!Directory.Exists(Path.Combine(DoomEternal.SavePath, dstUidBox.Text))) {
            //    Directory.CreateDirectory(Path.Combine(DoomEternal.SavePath, dstUidBox.Text));
            //}
            if (res) {
                DialogResult = DialogResult.OK;
            }
        }

        private void transferCancelBtn_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }
    }
}
