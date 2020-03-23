namespace DOOMSaveManager
{
    partial class SelectUUIDForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.selectUidOkBtn = new System.Windows.Forms.Button();
            this.selectUidCancelBtn = new System.Windows.Forms.Button();
            this.selectUidComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // selectUidOkBtn
            // 
            this.selectUidOkBtn.Location = new System.Drawing.Point(271, 48);
            this.selectUidOkBtn.Name = "selectUidOkBtn";
            this.selectUidOkBtn.Size = new System.Drawing.Size(75, 23);
            this.selectUidOkBtn.TabIndex = 0;
            this.selectUidOkBtn.Text = "OK";
            this.selectUidOkBtn.UseVisualStyleBackColor = true;
            this.selectUidOkBtn.Click += new System.EventHandler(this.selectUidOkBtn_Click);
            // 
            // selectUidCancelBtn
            // 
            this.selectUidCancelBtn.Location = new System.Drawing.Point(12, 48);
            this.selectUidCancelBtn.Name = "selectUidCancelBtn";
            this.selectUidCancelBtn.Size = new System.Drawing.Size(75, 23);
            this.selectUidCancelBtn.TabIndex = 1;
            this.selectUidCancelBtn.Text = "Cancel";
            this.selectUidCancelBtn.UseVisualStyleBackColor = true;
            this.selectUidCancelBtn.Click += new System.EventHandler(this.selectUidCancelBtn_Click);
            // 
            // selectUidComboBox
            // 
            this.selectUidComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectUidComboBox.FormattingEnabled = true;
            this.selectUidComboBox.Location = new System.Drawing.Point(56, 12);
            this.selectUidComboBox.Name = "selectUidComboBox";
            this.selectUidComboBox.Size = new System.Drawing.Size(254, 21);
            this.selectUidComboBox.TabIndex = 2;
            // 
            // UUIDForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 83);
            this.ControlBox = false;
            this.Controls.Add(this.selectUidComboBox);
            this.Controls.Add(this.selectUidCancelBtn);
            this.Controls.Add(this.selectUidOkBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "UUIDForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select UUID";
            this.Load += new System.EventHandler(this.UUIDForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button selectUidOkBtn;
        private System.Windows.Forms.Button selectUidCancelBtn;
        public System.Windows.Forms.ComboBox selectUidComboBox;
    }
}