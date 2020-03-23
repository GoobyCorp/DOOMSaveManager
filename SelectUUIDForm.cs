using System;
using System.Windows.Forms;

namespace DOOMSaveManager
{
    public partial class SelectUUIDForm : Form
    {
        public SelectUUIDForm(string title = "") {
            InitializeComponent();
            if(!String.IsNullOrEmpty(title))
                this.Text = title;
        }

        private void UUIDForm_Load(object sender, EventArgs e) {
            selectUidComboBox.Items.AddRange(DoomEternal.GetUserIDs());
            if (selectUidComboBox.Items.Count == 0) {
                DialogResult = DialogResult.Abort;
            } else if (selectUidComboBox.Items.Count > 0) {
                selectUidComboBox.SelectedIndex = 0;
            }
        }

        private void selectUidOkBtn_Click(object sender, EventArgs e) {
            bool res = true;
            if(String.IsNullOrEmpty(selectUidComboBox.Text)) {
                res = false;
                MessageBox.Show("No UUID has been selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if(res) {
                DialogResult = DialogResult.OK;
            }
        }

        private void selectUidCancelBtn_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }
    }
}
