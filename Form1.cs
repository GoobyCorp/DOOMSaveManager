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
            DoomEternal.EnumerateSaves();

            if(!Directory.Exists(DoomEternal.BnetSavePath) && !Directory.Exists(DoomEternal.BnetSavePathUnencrypted) && !Directory.Exists(DoomEternal.SteamSavePath)) {
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
                                if (suf.SelectedSave.Identifier == "savegame.unencrypted") {
                                    Utilities.Unarchive(ofd.FileName, DoomEternal.BnetSavePathUnencrypted);
                                } else {
                                    if (suf.SelectedSave.Platform == DoomEternalSavePlatform.BethesdaNet) {
                                        Directory.CreateDirectory("tmp");
                                        Utilities.Unarchive(ofd.FileName, "tmp");
                                        DoomEternal.BnetBulkEncrypt("tmp", suf.SelectedSave.Identifier);
                                        Directory.Delete("tmp", true);
                                    } else if (suf.SelectedSave.Platform == DoomEternalSavePlatform.Steam) {
                                        Directory.CreateDirectory("tmp");
                                        Utilities.Unarchive(ofd.FileName, "tmp");
                                        DoomEternal.SteamBulkEncrypt("tmp", suf.SelectedSave.Identifier);
                                        Directory.Delete("tmp", true);
                                    }
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
                                if (suf.SelectedSave.Identifier == "savegame.unencrypted") {
                                    Utilities.Archive(sfd.FileName, DoomEternal.BnetSavePathUnencrypted);
                                } else {
                                    if(suf.SelectedSave.Platform == DoomEternalSavePlatform.BethesdaNet) {
                                        Directory.CreateDirectory("tmp");
                                        DoomEternal.BnetBulkDecrypt(suf.SelectedSave.Identifier, "tmp");
                                        Utilities.Archive(sfd.FileName, "tmp");
                                        Directory.Delete("tmp", true);
                                    } else if(suf.SelectedSave.Platform == DoomEternalSavePlatform.Steam) {
                                        Directory.CreateDirectory("tmp");
                                        DoomEternal.SteamBulkDecrypt(suf.SelectedSave.Identifier, "tmp");
                                        Utilities.Archive(sfd.FileName, "tmp");
                                        Directory.Delete("tmp", true);
                                    }
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
                        // messy :'(
                        if (tf.SrcSave.Identifier == "savegame.unencrypted") {
                            if (tf.DstSave.Platform == DoomEternalSavePlatform.BethesdaNet)
                                DoomEternal.BnetBulkEncrypt(DoomEternal.BnetSavePathUnencrypted, tf.DstSave.Identifier);
                            else if (tf.DstSave.Platform == DoomEternalSavePlatform.Steam)
                                DoomEternal.SteamBulkEncrypt(DoomEternal.BnetSavePathUnencrypted, tf.DstSave.Identifier);
                        } else if (tf.DstSave.Identifier == "savegame.unencrypted") {
                            if (tf.SrcSave.Platform == DoomEternalSavePlatform.BethesdaNet)
                                DoomEternal.BnetBulkDecrypt(tf.SrcSave.Identifier, DoomEternal.BnetSavePathUnencrypted);
                            else if (tf.SrcSave.Platform == DoomEternalSavePlatform.Steam)
                                DoomEternal.SteamBulkDecrypt(tf.SrcSave.Identifier, DoomEternal.BnetSavePathUnencrypted);
                        } else {
                            if (tf.SrcSave.Platform == DoomEternalSavePlatform.BethesdaNet && tf.DstSave.Platform == DoomEternalSavePlatform.BethesdaNet)  // bnet to bnet
                                DoomEternal.BnetBulkTransfer(tf.SrcSave.Identifier, tf.DstSave.Identifier);
                            else if (tf.SrcSave.Platform == DoomEternalSavePlatform.BethesdaNet && tf.DstSave.Platform == DoomEternalSavePlatform.Steam)  // bnet to steam
                                DoomEternal.BnetToSteamTransfer(tf.SrcSave.Identifier, tf.DstSave.Identifier);
                            else if (tf.SrcSave.Platform == DoomEternalSavePlatform.Steam && tf.DstSave.Platform == DoomEternalSavePlatform.BethesdaNet)  // steam to bnet
                                DoomEternal.SteamToBnetTransfer(tf.SrcSave.Identifier, tf.DstSave.Identifier);
                            else if (tf.SrcSave.Platform == DoomEternalSavePlatform.Steam && tf.DstSave.Platform == DoomEternalSavePlatform.Steam)  // steam to steam
                                DoomEternal.SteamBulkTransfer(tf.SrcSave.Identifier, tf.DstSave.Identifier);
                        }
                        MessageBox.Show("Transfer success!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    break;
                    }
            }
        }
    }
}
