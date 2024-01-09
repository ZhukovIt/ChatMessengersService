namespace SiMed.ChatMessengers.Umnico.GUI
{
    partial class CMUmnicoGlobalOptionsForm
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
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbLogin = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.btnCheckConnectionUmnico = new System.Windows.Forms.Button();
            this.Label3 = new System.Windows.Forms.Label();
            this.tbURL = new System.Windows.Forms.TextBox();
            this.errors = new System.Windows.Forms.ErrorProvider(this.components);
            this.bCancel = new System.Windows.Forms.Button();
            this.bOK = new System.Windows.Forms.Button();
            this.SettingsPanel = new System.Windows.Forms.Panel();
            this.tbIDClinic = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnChangeHook = new System.Windows.Forms.Button();
            this.tbWebHookURL = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCheckConnectionSiMed = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tbClinicGUID = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.errors)).BeginInit();
            this.SettingsPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbPassword
            // 
            this.tbPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbPassword.Location = new System.Drawing.Point(111, 39);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(376, 20);
            this.tbPassword.TabIndex = 7;
            // 
            // tbLogin
            // 
            this.tbLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbLogin.Location = new System.Drawing.Point(111, 13);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.Size = new System.Drawing.Size(376, 20);
            this.tbLogin.TabIndex = 6;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(3, 39);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(84, 13);
            this.Label2.TabIndex = 5;
            this.Label2.Text = "Пароль Umnico";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(3, 13);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(77, 13);
            this.Label1.TabIndex = 4;
            this.Label1.Text = "Логин Umnico";
            // 
            // btnCheckConnectionUmnico
            // 
            this.btnCheckConnectionUmnico.Location = new System.Drawing.Point(42, 0);
            this.btnCheckConnectionUmnico.Name = "btnCheckConnectionUmnico";
            this.btnCheckConnectionUmnico.Size = new System.Drawing.Size(186, 28);
            this.btnCheckConnectionUmnico.TabIndex = 12;
            this.btnCheckConnectionUmnico.Text = "Проверка связи с Umnico";
            this.btnCheckConnectionUmnico.UseVisualStyleBackColor = true;
            this.btnCheckConnectionUmnico.Click += new System.EventHandler(this.btnCheckConnectionUmnico_Click);
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(3, 65);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(83, 13);
            this.Label3.TabIndex = 13;
            this.Label3.Text = "Сервер Umnico";
            // 
            // tbURL
            // 
            this.tbURL.Location = new System.Drawing.Point(111, 65);
            this.tbURL.Name = "tbURL";
            this.tbURL.Size = new System.Drawing.Size(376, 20);
            this.tbURL.TabIndex = 14;
            this.tbURL.Text = "https://api.umnico.com/v1.3";
            // 
            // errors
            // 
            this.errors.ContainerControl = this;
            // 
            // bCancel
            // 
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bCancel.Location = new System.Drawing.Point(262, 216);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(88, 28);
            this.bCancel.TabIndex = 16;
            this.bCancel.Text = "Отмена";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bOK
            // 
            this.bOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bOK.Location = new System.Drawing.Point(181, 216);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 28);
            this.bOK.TabIndex = 15;
            this.bOK.Text = "Ок";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // SettingsPanel
            // 
            this.SettingsPanel.Controls.Add(this.tbClinicGUID);
            this.SettingsPanel.Controls.Add(this.label6);
            this.SettingsPanel.Controls.Add(this.tbIDClinic);
            this.SettingsPanel.Controls.Add(this.label5);
            this.SettingsPanel.Controls.Add(this.btnChangeHook);
            this.SettingsPanel.Controls.Add(this.tbWebHookURL);
            this.SettingsPanel.Controls.Add(this.label4);
            this.SettingsPanel.Controls.Add(this.Label1);
            this.SettingsPanel.Controls.Add(this.Label2);
            this.SettingsPanel.Controls.Add(this.tbLogin);
            this.SettingsPanel.Controls.Add(this.tbURL);
            this.SettingsPanel.Controls.Add(this.tbPassword);
            this.SettingsPanel.Controls.Add(this.Label3);
            this.SettingsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.SettingsPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SettingsPanel.Location = new System.Drawing.Point(0, 0);
            this.SettingsPanel.Name = "SettingsPanel";
            this.SettingsPanel.Size = new System.Drawing.Size(514, 177);
            this.SettingsPanel.TabIndex = 17;
            // 
            // tbIDClinic
            // 
            this.tbIDClinic.Location = new System.Drawing.Point(111, 117);
            this.tbIDClinic.Name = "tbIDClinic";
            this.tbIDClinic.Size = new System.Drawing.Size(295, 20);
            this.tbIDClinic.TabIndex = 19;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "ID Клиники";
            // 
            // btnChangeHook
            // 
            this.btnChangeHook.Location = new System.Drawing.Point(412, 89);
            this.btnChangeHook.Name = "btnChangeHook";
            this.btnChangeHook.Size = new System.Drawing.Size(75, 48);
            this.btnChangeHook.TabIndex = 17;
            this.btnChangeHook.Text = "Добавить";
            this.btnChangeHook.UseVisualStyleBackColor = true;
            this.btnChangeHook.Click += new System.EventHandler(this.btnChangeHook_Click);
            // 
            // tbWebHookURL
            // 
            this.tbWebHookURL.Location = new System.Drawing.Point(111, 91);
            this.tbWebHookURL.Name = "tbWebHookURL";
            this.tbWebHookURL.Size = new System.Drawing.Size(295, 20);
            this.tbWebHookURL.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Сервер Клиники";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnCheckConnectionSiMed);
            this.panel2.Controls.Add(this.btnCheckConnectionUmnico);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.panel2.Location = new System.Drawing.Point(0, 177);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(514, 28);
            this.panel2.TabIndex = 18;
            // 
            // btnCheckConnectionSiMed
            // 
            this.btnCheckConnectionSiMed.Location = new System.Drawing.Point(288, 0);
            this.btnCheckConnectionSiMed.Name = "btnCheckConnectionSiMed";
            this.btnCheckConnectionSiMed.Size = new System.Drawing.Size(186, 28);
            this.btnCheckConnectionSiMed.TabIndex = 13;
            this.btnCheckConnectionSiMed.Text = "Проверка связи с СиМед";
            this.btnCheckConnectionSiMed.UseVisualStyleBackColor = true;
            this.btnCheckConnectionSiMed.Click += new System.EventHandler(this.btnCheckConnectionSiMed_Click);
            // 
            // tbClinicGUID
            // 
            this.tbClinicGUID.Location = new System.Drawing.Point(111, 143);
            this.tbClinicGUID.Name = "tbClinicGUID";
            this.tbClinicGUID.Size = new System.Drawing.Size(376, 20);
            this.tbClinicGUID.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 143);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "GUID Клиники";
            // 
            // CMUmnicoGlobalOptionsForm
            // 
            this.AcceptButton = this.bOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(514, 255);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.SettingsPanel);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "CMUmnicoGlobalOptionsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Глобальные настройки системы месенджеров";
            ((System.ComponentModel.ISupportInitialize)(this.errors)).EndInit();
            this.SettingsPanel.ResumeLayout(false);
            this.SettingsPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbLogin;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.Button btnCheckConnectionUmnico;
        private System.Windows.Forms.Label Label3;
        private System.Windows.Forms.TextBox tbURL;
        private System.Windows.Forms.ErrorProvider errors;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel SettingsPanel;
        private System.Windows.Forms.TextBox tbWebHookURL;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnChangeHook;
        private System.Windows.Forms.Button btnCheckConnectionSiMed;
        private System.Windows.Forms.TextBox tbIDClinic;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TextBox tbClinicGUID;
        private System.Windows.Forms.Label label6;
    }
}