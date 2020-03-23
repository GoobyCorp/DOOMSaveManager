using System;
using System.Windows.Forms;

using System.IO;

namespace DOOMSaveManager
{
    public partial class SelectForm : Form
    {
        public SelectForm(string title = "") {
            InitializeComponent();
            if(!String.IsNullOrEmpty(title))
                this.Text = title;
        }

        private void SelectForm_Load(object sender, EventArgs e) {
            selectComboBox.Items.AddRange(DoomEternal.GetUserIDs());
            if (selectComboBox.Items.Count > 0) {
                selectComboBox.SelectedIndex = 0;
            }
            if (Directory.Exists(Path.Combine(DoomEternal.SavePath, "savegame.unencrypted"))) {
                selectComboBox.Items.Add("savegame.unencrypted");
            }
        }

        private void selectOkBtn_Click(object sender, EventArgs e) {
            bool res = true;
            if(String.IsNullOrEmpty(selectComboBox.Text)) {
                res = false;
                MessageBox.Show("No item has been selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if(res) {
                DialogResult = DialogResult.OK;
            }
        }

        private void selectCancelBtn_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }
    }
}
