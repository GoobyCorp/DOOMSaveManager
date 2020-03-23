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
                            var suf = new SelectForm("Select Import Destination");
                            if (suf.ShowDialog() == DialogResult.OK) {
                                if(suf.selectComboBox.Text == "savegame.unencrypted") {
                                    // Directory.CreateDirectory(Path.Combine(DoomEternal.SavePath, "savegame.unencrypted"));
                                    Utilities.Unarchive(ofd.FileName, Path.Combine(DoomEternal.SavePath, "savegame.unencrypted"));
                                } else {
                                    Directory.CreateDirectory("tmp");
                                    Utilities.Unarchive(ofd.FileName, "tmp");
                                    DoomEternal.BulkEncrypt("tmp", suf.selectComboBox.Text);
                                    Directory.Delete("tmp", true);
                                }
                                MessageBox.Show("Import success!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    break;
                }
                case "Export Backup": {
                    var suf = new SelectForm("Select Export Source");
                    if(suf.ShowDialog() == DialogResult.OK) {
                        using (var sfd = new SaveFileDialog()) {
                            sfd.Title = "Save Backup";
                            sfd.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                            sfd.Filter = "Zip Files (*.zip)|*.zip";
                            sfd.FilterIndex = 0;
                            sfd.FileName = "backup.zip";
                            if (sfd.ShowDialog() == DialogResult.OK) {
                                if (suf.selectComboBox.Text == "savegame.unencrypted") {
                                    Utilities.Archive(sfd.FileName, Path.Combine(DoomEternal.SavePath, "savegame.unencrypted"));
                                } else {
                                    Directory.CreateDirectory("tmp");
                                    DoomEternal.BulkDecrypt(suf.selectComboBox.Text, "tmp");
                                    Utilities.Archive(sfd.FileName, "tmp");
                                    Directory.Delete("tmp", true);
                                }
                                MessageBox.Show("Export success!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    break;
                }
                case "Transfer": {
                    var tf = new TransferForm();
                    if(tf.ShowDialog() == DialogResult.OK) {
                        if(tf.srcComboBox.Text == "savegame.unencrypted") {
                            DoomEternal.BulkEncrypt(Path.Combine(DoomEternal.SavePath, "savegame.unencrypted"), tf.dstUidBox.Text);
                        } else {
                            DoomEternal.BulkTransfer(tf.srcComboBox.Text, tf.dstUidBox.Text);
                        }
                        MessageBox.Show("Transfer success!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    break;
                }
            }
        }
    }
}
