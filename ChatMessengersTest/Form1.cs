using SiMed.ChatMessengers.Umnico;
using SiMed.Clinic.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppealMessengersTest
{
    public partial class Form1 : Form
    {
        //CMUmnicoGlobalOptions globalOptions;
        CMUmnico m_CMUmnico;
        AttachmentForMessage m_Attacment;
        string localOptions;
        string globalOptions;
        public Form1()
        {
            InitializeComponent();
            //globalOptions = new CMUmnicoGlobalOptions();
            m_CMUmnico = new CMUmnico((string message) => { MessageBox.Show(message); });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(m_CMUmnico.CheckConnection())
            {
                MessageBox.Show("Успешно!", "Проверка соединения", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка соединения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (m_CMUmnico.Init(ClinicChatAggregatorApplicationType.Program))
            {
                MessageBox.Show("Успешно!", "Проверка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (m_CMUmnico.UpdateTokens())
            {
                MessageBox.Show("Успешно!", "Проверка обновления токенов", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка обновления токенов", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<SourceMessage> tmp = m_CMUmnico.GetSourceMessages().ToList();
            if (tmp!=null)
            {
                foreach (SourceMessage value in tmp)
                {
                    listView1.Items.Add(value.Type+"|||"+value.Id);
                }
                MessageBox.Show("Успешно!", "Проверка получения мессенджеров", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка получения мессенджеров", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            List<Appeal> tmp = m_CMUmnico.GetAppealsNew();
            if (tmp != null)
            {
                foreach (Appeal value in tmp)
                {
                    listView2.Items.Add(value.customer.login+"|||"+ value.customer.name + "|||" + value.id);
                }
                MessageBox.Show("Успешно!", "Проверка получения новых чатов", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка получения новых чатов", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            List<Appeal> tmp = m_CMUmnico.GetAppealsActive();
            if (tmp != null)
            {
                foreach (Appeal value in tmp)
                {
                    listView2.Items.Add(value.customer.login + "|||" + value.customer.name + "|||" + value.id);
                }
                MessageBox.Show("Успешно!", "Проверка получения активных чатов", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка получения активных чатов", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            List<Appeal> tmp = m_CMUmnico.GetAppealsArchive();
            if (tmp != null)
            {
                foreach (Appeal value in tmp)
                {
                    listView2.Items.Add(value.customer.login + "|||" + value.customer.name + "|||" + value.id);
                }
                MessageBox.Show("Успешно!", "Проверка получения архивных чатов", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка получения архивных чатов", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            List<Appeal> tmp = m_CMUmnico.GetAppealsAll();
            if (tmp != null)
            {
                foreach (Appeal value in tmp)
                {
                    listView2.Items.Add(value.customer.login + "|||" + value.customer.name + "|||"+value.id);
                }
                MessageBox.Show("Успешно!", "Проверка получения всех чатов", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка получения всех чатов", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            Appeal value = m_CMUmnico.GetAppealOne(Convert.ToInt32(textBox1.Text));
            if (value != null)
            {
                listView2.Items.Add(value.id.ToString());
                MessageBox.Show("Успешно!", "Проверка получения одного чата", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка получения одного чата", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            List<long> ids = new List<long>();
            foreach (string id in textBox2.Text.Split(','))
            {
                ids.Add(Convert.ToInt32(id));
            }
            List<Appeal> tmp = m_CMUmnico.GetAppealsById(ids);
            if (tmp != null)
            {
                foreach (Appeal value in tmp)
                {
                    listView2.Items.Add(value.customer.login + "|||" + value.customer.name + "|||" + value.id);
                }
                MessageBox.Show("Успешно!", "Проверка получения чатов по ids", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка получения чатов по ids", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            listView3.Items.Clear();
            List<SiMed.Clinic.DataModel.Manager> tmp = m_CMUmnico.GetManagers().ToList();
            if (tmp != null)
            {
                foreach (SiMed.Clinic.DataModel.Manager value in tmp)
                {
                    listView3.Items.Add(value.Name + "|||" + value.Login + "|||"+ value.Id);
                }
                MessageBox.Show("Успешно!", "Проверка получения списка операторов", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка получения списка операторов", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (m_CMUmnico.ReadMessageForAppeal(Convert.ToInt32(textBox3.Text)))
            {
                MessageBox.Show("Успешно!", "Проверка прочтения сообщений в чате", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка прочтения сообщений в чате", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (m_CMUmnico.AcceptAppealByIdManager(Convert.ToInt32(textBox4.Text)))
            {
                MessageBox.Show("Успешно!", "Проверка закрепления чата сотруднику", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка закрепления чата сотруднику", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            
            using (StreamReader reader = new StreamReader(@"C:\Users\Simplex\Desktop\Globaloptions.txt"))
            {
                globalOptions = reader.ReadToEnd();
            }
            using (StreamReader reader = new StreamReader(@"C:\Users\Simplex\Desktop\Localoptions.txt"))
            {
                localOptions = reader.ReadToEnd();
            }
            if(m_CMUmnico.SetOptions(globalOptions, localOptions))
            {
                MessageBox.Show("Успешно!", "Загрузка настроек", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Загрузка настроек", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            List<KeyValuePair<string, object>> tmp = new List<KeyValuePair<string, object>>();
            string[] str = textBox9.Text.Split(',');
            object[] obj = textBox10.Text.Split(',');
            for (int i=0; i < str.Length && str.Length==obj.Length; i++)
            {
                tmp.Add(new KeyValuePair<string, object>(str[i], obj[i]));
            }
            
            if (m_CMUmnico.ChangeAppeal(tmp,Convert.ToInt32(textBox8.Text)))
            {
                MessageBox.Show("Успешно!", "Изменение чата", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Изменение чата", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            listView4.Items.Clear();
            List<Chat> tmp = m_CMUmnico.GetChatsByIdAppeal(Convert.ToInt32(textBox6.Text));
            if (tmp != null)
            {
                foreach (Chat value in tmp)
                {
                    listView4.Items.Add(value.realId + "|||" + value.saId + "|||" + value.type + "|||" + value.identifier);
                }
                MessageBox.Show("Успешно!", "Проверка получения списка каналов", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка получения списка каналов", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            listView5.Items.Clear();
            var tmp = m_CMUmnico.CheckNewMessages();
            if (tmp != null)
            {
                //foreach (SiMed.ChatMessengers.Umnico.Message value in tmp.messages)
                //{
                //    listView5.Items.Add(value.sender.login + "|||" + value.incoming + "|||" + value.text);
                //}
                MessageBox.Show("Успешно!", "Проверка получения списка сообщений", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка получения списка сообщений", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            listView5.Items.Clear();
            openFileDialog1.ShowDialog();
            if (m_CMUmnico.SendMessage(textBox13.Text, textBox14.Text,"48", openFileDialog1.FileName, MessengersType.Telegram) !=null)
            {
                MessageBox.Show("Успешно!", "Проверка отправки сообщения", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка отправки сообщения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            listView5.Items.Clear();
            openFileDialog1.ShowDialog();

            m_CMUmnico.FirstSendMessage(textBox16.Text, textBox17.Text, textBox15.Text, "123123", "");
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            string filePath = openFileDialog1.FileName;
            m_Attacment = m_CMUmnico.UnloadFileForMessage(Convert.ToInt32(textBox18.Text), checkBox1.Checked, filePath,CMUmnicoEnums.MessengerTypes.telegram);
            if (m_Attacment != null && m_Attacment.media != null)
            {
                MessageBox.Show("Успешно!", "Проверка загрузки файла", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка загрузки файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button21_Click(object sender, EventArgs e)
        {
            listView6.Items.Clear();
            List<WebHook> tmp = m_CMUmnico.GetWebHooks();
            if (tmp != null)
            {
                foreach (WebHook value in tmp)
                {
                    listView6.Items.Add(value.id + "|||" + value.url);
                }
                MessageBox.Show("Успешно!", "Проверка получения списка хуков", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка получения списка хуков", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (m_CMUmnico.ShowLocalOptions(ref localOptions))
            {
                MessageBox.Show("Успешно!", "Просмотр настроек локальных", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Просмотр настроек локальных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (m_CMUmnico.ShowSystemOptions(ref globalOptions))
            {
                MessageBox.Show("Успешно!", "Просмотр настроек глобальных", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Просмотр настроек глобальных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(@"C:\Users\Simplex\Desktop\Localoptions.txt", false))
            {
                writer.WriteLine(localOptions);
            }
            using (StreamWriter writer = new StreamWriter(@"C:\Users\Simplex\Desktop\Globaloptions.txt", false))
            {
                writer.WriteLine(globalOptions);
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            listView3.Items.Clear();
            var tmp = m_CMUmnico.GetCustomer(textBox5.Text);
            if (tmp != null)
            {
                listView3.Items.Add(tmp.Id + "|||" + tmp.Login+ "|||" + tmp.Name);
                MessageBox.Show("Успешно!", "Проверка получения клиента", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Провалено!", "Проверка получения клианта", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
