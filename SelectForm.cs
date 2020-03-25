using System;
using System.Windows.Forms;

using System.IO;

namespace DOOMSaveManager
{
    public partial class SelectForm : Form
    {
        public DoomEternalSave SelectedSave;

        public SelectForm(string title = "") {
            InitializeComponent();
            if(!String.IsNullOrEmpty(title))
                this.Text = title;
        }

        private void SelectForm_Load(object sender, EventArgs e) {
            selectComboBox.Items.AddRange(DoomEternal.Saves.GetIdentifiers());
            if (selectComboBox.Items.Count > 0) {
                selectComboBox.SelectedIndex = 0;
            }
            if (!Directory.Exists(Path.Combine(DoomEternal.BnetSavePath, "savegame.unencrypted"))) {
                selectComboBox.Items.Remove("savegame.unencrypted");
            }
        }

        private void selectOkBtn_Click(object sender, EventArgs e) {
            if(String.IsNullOrEmpty(selectComboBox.Text)) {
                MessageBox.Show("No item has been selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(!DoomEternal.Saves.SaveExists(selectComboBox.Text, out SelectedSave)) {
                MessageBox.Show("That item doesn't exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DialogResult = DialogResult.OK;
        }

        private void selectCancelBtn_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }
    }
}
