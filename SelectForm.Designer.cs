namespace DOOMSaveManager
{
    partial class SelectForm
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
            this.selectOkBtn = new System.Windows.Forms.Button();
            this.selectCancelBtn = new System.Windows.Forms.Button();
            this.selectComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // selectOkBtn
            // 
            this.selectOkBtn.Location = new System.Drawing.Point(271, 48);
            this.selectOkBtn.Name = "selectOkBtn";
            this.selectOkBtn.Size = new System.Drawing.Size(75, 23);
            this.selectOkBtn.TabIndex = 0;
            this.selectOkBtn.Text = "OK";
            this.selectOkBtn.UseVisualStyleBackColor = true;
            this.selectOkBtn.Click += new System.EventHandler(this.selectOkBtn_Click);
            // 
            // selectCancelBtn
            // 
            this.selectCancelBtn.Location = new System.Drawing.Point(12, 48);
            this.selectCancelBtn.Name = "selectCancelBtn";
            this.selectCancelBtn.Size = new System.Drawing.Size(75, 23);
            this.selectCancelBtn.TabIndex = 1;
            this.selectCancelBtn.Text = "Cancel";
            this.selectCancelBtn.UseVisualStyleBackColor = true;
            this.selectCancelBtn.Click += new System.EventHandler(this.selectCancelBtn_Click);
            // 
            // selectComboBox
            // 
            this.selectComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectComboBox.FormattingEnabled = true;
            this.selectComboBox.Location = new System.Drawing.Point(56, 12);
            this.selectComboBox.Name = "selectComboBox";
            this.selectComboBox.Size = new System.Drawing.Size(254, 21);
            this.selectComboBox.TabIndex = 2;
            // 
            // SelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(358, 83);
            this.ControlBox = false;
            this.Controls.Add(this.selectComboBox);
            this.Controls.Add(this.selectCancelBtn);
            this.Controls.Add(this.selectOkBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "placeholder";
            this.Load += new System.EventHandler(this.SelectForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button selectOkBtn;
        private System.Windows.Forms.Button selectCancelBtn;
        public System.Windows.Forms.ComboBox selectComboBox;
    }
}