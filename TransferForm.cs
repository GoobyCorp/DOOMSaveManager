using System;
using System.Windows.Forms;

using System.IO;

namespace DOOMSaveManager
{
    public partial class TransferForm : Form
    {
        public DoomEternalSavePath SrcSave;
        public DoomEternalSavePath DstSave;

        private string[] uids;

        public TransferForm() {
            InitializeComponent();
        }

        private void TransferForm_Load(object sender, EventArgs e) {
            DoomEternal.EnumerateSaves();
            uids = DoomEternal.Saves.GetIdentifiers();

            srcComboBox.Items.AddRange(uids);

            dstComboBox.Items.AddRange(uids);

            if (srcComboBox.Items.Count > 0)
                srcComboBox.SelectedIndex = 0;
            if (dstComboBox.Items.Count > 0)
                dstComboBox.SelectedIndex = 0;
        }

        private void transferOkBtn_Click(object sender, EventArgs e) {
            if (!DoomEternal.Saves.SaveExists(srcComboBox.Text, out SrcSave)) {
                MessageBox.Show("Invalid source!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!DoomEternal.Saves.SaveExists(dstComboBox.Text, out DstSave)) {
                MessageBox.Show("Invalid destination!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!SrcSave.Exists()) {
                MessageBox.Show("Source directory doesn't exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!DstSave.Exists()) {
                MessageBox.Show("Destination directory doesn't exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void transferCancelBtn_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }

        private void srcComboBox_SelectedValueChanged(object sender, EventArgs e) {
            dstComboBox.Items.Clear();
            dstComboBox.Items.AddRange(uids);
            // dstComboBox.Items.Add("savegame.unencrypted");
            dstComboBox.Items.Remove(((ComboBox)sender).Text);
            if (dstComboBox.Items.Count > 0)
                dstComboBox.SelectedIndex = 0;
        }

        private void dstComboBox_SelectedValueChanged(object sender, EventArgs e) {

        }
    }
}
