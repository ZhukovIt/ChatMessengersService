using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace SiMed.ChatMessengers.Umnico.GUI
{
    public partial class CMUmnicoGlobalOptionsForm : Form
    {
        public CMUmnicoGlobalOptions globalOptions;
        AuthorizationInfo auth;
        WebHook webHook;
        public CMUmnicoGlobalOptionsForm(string _GlobalOptions)
        {
            InitializeComponent();
            ServicePointManager.Expect100Continue = true;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 |
                SecurityProtocolType.Tls |
                (SecurityProtocolType)768 |
                (SecurityProtocolType)3072;

            globalOptions = new CMUmnicoGlobalOptions();
            globalOptions = (CMUmnicoGlobalOptions)globalOptions.Unpack(_GlobalOptions);
            if (globalOptions == null)
                globalOptions = new CMUmnicoGlobalOptions();

            tbLogin.Text = globalOptions.LOGIN;
            tbPassword.Text = globalOptions.PASSWORD;
            tbURL.Text = globalOptions.MAIN_URL;
            tbClinicGUID.Text = globalOptions.GUID;

            auth = String.IsNullOrEmpty(_GlobalOptions) ? null: Requests.PostAuthorization(globalOptions);
            if (auth != null)
            {
                List<WebHook> listWebHook = Requests.GetWebHooks(globalOptions, auth);
                if (listWebHook.Count>0)
                {
                    webHook = listWebHook[0];
                    Uri uri = new Uri(webHook.url);
                    tbWebHookURL.Text = uri.Scheme +"://" +uri.Host+":"+ uri.Port;
                    tbIDClinic.Text = Uri.UnescapeDataString(uri.Segments[uri.Segments.Length - 1].ToString());
                    tbIDClinic.ReadOnly = true;
                    tbWebHookURL.ReadOnly = true;
                    btnChangeHook.Text = "Удалить";
                }
            }
            toolTip.SetToolTip(tbWebHookURL, "Пример: http://test.web.ru:81");
            toolTip.SetToolTip(tbURL, "Пример: http://test.web.ru");
        }
        private bool ApplySettings()
        {
            errors.Clear();
            if (tbURL.Text == "")
            {
                errors.SetError(tbURL, "Необходимо ввести URL");
                return false;
            }
            globalOptions.MAIN_URL = tbURL.Text;
            if (tbLogin.Text == "")
            {
                errors.SetError(tbLogin, "Необходимо ввести Логин от уетной записи Umnico");
                return false;
            }
            globalOptions.LOGIN = tbLogin.Text;
            if (tbPassword.Text == "")
            {
                errors.SetError(tbPassword, "Необходимо ввести Пароль от учетной записи Umnico");
                return false;
            }
            globalOptions.PASSWORD = tbPassword.Text;
            //if (tbWebHookURL.Text == "")
            //{
            //    errors.SetError(btnChangeHook, "Необходимо ввести адрес Веб-хука Веб-сервера");
            //    return false;
            //}
            //if (tbIDClinic.Text == "")
            //{
            //    errors.SetError(btnChangeHook, "Необходимо ввести id клиники");
            //    return false;
            //}
            globalOptions.GUID = tbClinicGUID.Text;
            if (tbClinicGUID.Text == "")
            {
                errors.SetError(tbClinicGUID, "Необходимо ввести GUID клиники");
                return false;
            }
            return true;
        }
        //Отмена
        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        //Сохранение
        private void bOK_Click(object sender, EventArgs e)
        {
            if (ApplySettings())
                DialogResult = DialogResult.OK;
        }

        private void btnCheckConnectionUmnico_Click(object sender, EventArgs e)
        {
            if (!ApplySettings())
                return;

            //string message;
            if (Requests.CheckConnection(globalOptions))
                MessageBox.Show("Успешно!", "Проверка соединения", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            else
                MessageBox.Show("Ошибка: ", "Проверка соединения", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void btnChangeHook_Click(object sender, EventArgs e)
        {
            if (!ApplySettings())
                return;
            if (auth == null)
            {
                auth = Requests.PostAuthorization(globalOptions);
            }
            if (auth == null)
            {
                MessageBox.Show("Ошибка авторизации при настройке веб-хука!\r\nПроверьте подключение к сети/umnico.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                if (btnChangeHook.Text == "Добавить")
                {
                    if (!Requests.AddClinicForServer(tbWebHookURL.Text,tbClinicGUID.Text,tbIDClinic.Text,tbLogin.Text))
                    {
                        return;
                    }
                    string URL = tbWebHookURL.Text + "/api/ChatMessengersWebHooks/InfoEvent/" + tbIDClinic.Text;
                    webHook = Requests.AddWebHook(globalOptions, auth, URL);
                    if (webHook==null)
                    {
                        return;
                    }
                    tbWebHookURL.ReadOnly = true;
                    tbIDClinic.ReadOnly = true;
                    btnChangeHook.Text = "Удалить";
                }
                else
                {
                    if (Requests.DeleteWebHook(globalOptions, auth, webHook.id))
                    {
                        btnChangeHook.Text = "Добавить";
                        tbWebHookURL.Text = "";
                        tbIDClinic.Text = "";
                        webHook = null;
                        tbWebHookURL.ReadOnly = false;
                        tbIDClinic.ReadOnly = false;
                    }
                }

            }
        }

        private void btnCheckConnectionSiMed_Click(object sender, EventArgs e)
        {
            if (!ApplySettings())
                return;

            string URL = tbWebHookURL.Text + "/HomeApi/ChatMessengersWebHooks/CheckConnection";
            if (Requests.CheckConnectionSiMed(URL))
                MessageBox.Show("Успешно!", "Проверка соединения", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            else
                MessageBox.Show("Ошибка: ", "Проверка соединения", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
    }
}
