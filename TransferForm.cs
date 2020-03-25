using System;
using System.Windows.Forms;

using System.IO;

namespace DOOMSaveManager
{
    public partial class TransferForm : Form
    {
        public DoomEternalSave SrcSave;
        public DoomEternalSave DstSave;

        private string[] uids;

        public TransferForm() {
            InitializeComponent();
        }

        private void TransferForm_Load(object sender, EventArgs e) {
            DoomEternal.EnumerateSaves();
            uids = DoomEternal.Saves.GetIdentifiers();

            if (!Directory.Exists(Path.Combine(DoomEternal.BnetSavePath, "savegame.unencrypted")))
                srcComboBox.Items.Remove("savegame.unencrypted");
            srcComboBox.Items.AddRange(uids);

            dstComboBox.Items.AddRange(uids);
            //dstComboBox.Items.Add("savegame.unencrypted");

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

            if(SrcSave.Platform == DoomEternalSavePlatform.BethesdaNet) {
                if (!Directory.Exists(Path.Combine(DoomEternal.BnetSavePath, SrcSave.Identifier))) {
                    MessageBox.Show("Source directory doesn't exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            } else if(SrcSave.Platform == DoomEternalSavePlatform.Steam) {
                if (!Directory.Exists(Utilities.GetSavePathForId64(ulong.Parse(SrcSave.Identifier)))) {
                    MessageBox.Show("Source directory doesn't exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (DstSave.Platform == DoomEternalSavePlatform.BethesdaNet) {
                if (!Directory.Exists(Path.Combine(DoomEternal.BnetSavePath, DstSave.Identifier))) {
                    MessageBox.Show("Destination directory doesn't exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            } else if (DstSave.Platform == DoomEternalSavePlatform.Steam) {
                if (!Directory.Exists(Utilities.GetSavePathForId64(ulong.Parse(DstSave.Identifier)))) {
                    MessageBox.Show("Destination directory doesn't exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            //if(dstComboBox.Text == "savegame.unencrypted") {
            //    if (!Directory.Exists(Path.Combine(DoomEternal.BnetSavePath, dstComboBox.Text)))
            //        Directory.CreateDirectory(Path.Combine(DoomEternal.BnetSavePath, dstComboBox.Text));
            //}

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
