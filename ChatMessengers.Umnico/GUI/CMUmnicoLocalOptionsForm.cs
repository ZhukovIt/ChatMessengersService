using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SiMed.ChatMessengers.Umnico.GUI
{
    public partial class CMUmnicoLocalOptionsForm : Form
    {
        public CMUmnicoLocalOptions localOptions;
        AuthorizationInfo auth;
        CMUmnicoGlobalOptions globalOptions;
        public CMUmnicoLocalOptionsForm(string _LocalOptions, CMUmnicoGlobalOptions _globalOptions)
        {
            InitializeComponent();
            localOptions = new CMUmnicoLocalOptions();
            localOptions = (CMUmnicoLocalOptions)localOptions.Unpack(_LocalOptions);
            if (localOptions == null)
                localOptions = new CMUmnicoLocalOptions();
            globalOptions = _globalOptions;
            auth = _globalOptions==null ? null : Requests.PostAuthorization(globalOptions);          
            if (auth != null)
            {
                List<Manager> listManagers = Requests.GetManagers(globalOptions, auth);
                if (listManagers.Count==0)
                {
                    ApplicationLog.SaveExceptionToLog(new Exception("Список менеджеров пуст"), CMUmnico.m_appType);
                }
                else if (String.IsNullOrEmpty(listManagers[0].login) || String.IsNullOrEmpty(listManagers[0].name))
                {
                    ApplicationLog.SaveExceptionToLog(new Exception("Данные списка менеджеров не корректны"), CMUmnico.m_appType);
                }
                cmbManagers.DataSource = listManagers;
                cmbManagers.DisplayMember = "name";
                cmbManagers.ValueMember = "id";
                ////cmbManagers.Items.AddRange(listManagers.ToArray());
                //foreach (Manager man in listManagers)
                //{
                //    cmbManagers.Items.Add(man);
                //}
                if (localOptions.m_Manager!=null)
                {
                    cmbManagers.SelectedItem = cmbManagers.Items.Cast<Manager>().FirstOrDefault(x => x.id == localOptions.m_Manager.id);
                }
                else
                {
                    cmbManagers.SelectedItem = null;
                }
                
            }
            else
            {

            }

        }
        private bool ApplySettings()
        {
            errors.Clear();
            if (cmbManagers.SelectedValue == null)
            {
                errors.SetError(cmbManagers, "Необходимо выбрать оператора");
                return false;
            }
            localOptions.m_Manager = (Manager)cmbManagers.SelectedItem;
            return true;
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            if (ApplySettings())
                DialogResult = DialogResult.OK;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cmbManagers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbManagers.SelectedIndex < 0)
            {
                btnCopy.Enabled = false;
            }
            else
            {
                btnCopy.Enabled = true;
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                CMUmnicoLocalOptions _localOptions = new CMUmnicoLocalOptions();
                _localOptions.m_Manager = (Manager)cmbManagers.SelectedItem;
                string txtXml = _localOptions.Pack();
                Clipboard.SetText(txtXml);
            }
            catch (Exception ex)
            {
                errors.SetError(btnCopy, "Не удалось скопировать настройки");
            }
        }
    }
}
