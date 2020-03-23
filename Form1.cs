using System;
using System.Windows.Forms;

using System.IO;

namespace DOOMSaveManager
{
    public partial class Form1 : Form
    {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            bool res = false;
            // add tabs back if the games exist
            if(Directory.Exists(DoomEternal.SavePath)) {
                res = true;
            }
            if(!res) {
                MessageBox.Show("DOOM Eternal wasn't found on your computer!\r\nThe program will now exit...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void actionOkBtn_Click(object sender, EventArgs e) {
            switch(actionComboBox.Text) {
                case "Import Backup": {
                    using (var ofd = new OpenFileDialog()) {
                        ofd.Title = "Open Backup";
                        ofd.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                        ofd.Filter = "Zip Files (*.zip)|*.zip";
                        ofd.FilterIndex = 0;
                        ofd.FileName = "backup.Zip";
                        if (ofd.ShowDialog() == DialogResult.OK) {
                            var suf = new SelectUUIDForm("Select Import Destination UUID");
                            if (suf.ShowDialog() == DialogResult.OK) {
                                Directory.CreateDirectory("tmp");
                                Utilities.Unarchive(ofd.FileName, "tmp");
                                DoomEternal.BulkEncrypt("tmp", suf.selectUidComboBox.Text);
                                Directory.Delete("tmp", true);
                                MessageBox.Show("Import success!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    break;
                }
                case "Export Backup": {
                    var suf = new SelectUUIDForm("Select Export Source UUID");
                    if(suf.ShowDialog() == DialogResult.OK) {
                        using (var sfd = new SaveFileDialog()) {
                            sfd.Title = "Save Backup";
                            sfd.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                            sfd.Filter = "Zip Files (*.zip)|*.zip";
                            sfd.FilterIndex = 0;
                            sfd.FileName = "backup.zip";
                            if (sfd.ShowDialog() == DialogResult.OK) {
                                Directory.CreateDirectory("tmp");
                                DoomEternal.BulkDecrypt(suf.selectUidComboBox.Text, "tmp");
                                Utilities.Archive(sfd.FileName, "tmp");
                                Directory.Delete("tmp", true);
                                MessageBox.Show("Export success!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    break;
                }
                case "Transfer": {
                    var tf = new TransferForm();
                    if(tf.ShowDialog() == DialogResult.OK) {
                        DoomEternal.BulkTransfer(tf.srcUidComboBox.Text, tf.dstUidBox.Text);
                        MessageBox.Show("Transfer success!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    break;
                }
            }
        }
    }
}
