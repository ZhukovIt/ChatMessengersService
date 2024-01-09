namespace SiMed.ChatMessengers.Umnico.GUI
{
    partial class CMUmnicoLocalOptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.errors = new System.Windows.Forms.ErrorProvider(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.cmbManagers = new System.Windows.Forms.ComboBox();
            this.bCancel = new System.Windows.Forms.Button();
            this.bOK = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errors)).BeginInit();
            this.SuspendLayout();
            // 
            // errors
            // 
            this.errors.ContainerControl = this;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Оператор Umnico";
            // 
            // cmbManagers
            // 
            this.cmbManagers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbManagers.FormattingEnabled = true;
            this.cmbManagers.Location = new System.Drawing.Point(122, 17);
            this.cmbManagers.Name = "cmbManagers";
            this.cmbManagers.Size = new System.Drawing.Size(285, 21);
            this.cmbManagers.TabIndex = 1;
            this.cmbManagers.SelectedIndexChanged += new System.EventHandler(this.cmbManagers_SelectedIndexChanged);
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bCancel.Location = new System.Drawing.Point(224, 75);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(88, 28);
            this.bCancel.TabIndex = 18;
            this.bCancel.Text = "Отмена";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bOK
            // 
            this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bOK.Location = new System.Drawing.Point(143, 75);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 28);
            this.bOK.TabIndex = 17;
            this.bOK.Text = "Ок";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(265, 44);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(142, 23);
            this.btnCopy.TabIndex = 19;
            this.btnCopy.Text = "Скопировать в буфер";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // CMUmnicoLocalOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 115);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.cmbManagers);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "CMUmnicoLocalOptionsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Локальные настройки системы месенджеров";
            ((System.ComponentModel.ISupportInitialize)(this.errors)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ErrorProvider errors;
        private System.Windows.Forms.ComboBox cmbManagers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Button btnCopy;
    }
}