using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Optics
{
    public partial class AddClientForm : Form
    {
        public AddClientForm()
        {
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Validation.IsValidFIO(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Validation.IsValidFIO(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Метод для автоматического ввода первой заглавной буквы
        /// </summary>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                var words = textBox1.Text.Split('-');
                for (int i = 0; i < words.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(words[i]))
                    {
                        words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                    }
                }

                textBox1.Text = string.Join("-", words);
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.SelectionLength = 0;
                textBox1.TextChanged -= textBox1_TextChanged;
                textBox1.Text = textBox1.Text;
                textBox1.TextChanged += textBox1_TextChanged;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox2.Text))
            {
                var words = textBox2.Text.Split('-');
                for (int i = 0; i < words.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(words[i]))
                    {
                        words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                    }
                }

                textBox2.Text = string.Join("-", words);
                textBox2.SelectionStart = textBox2.Text.Length;
                textBox2.SelectionLength = 0;
                textBox2.TextChanged -= textBox2_TextChanged;
                textBox2.Text = textBox2.Text;
                textBox2.TextChanged += textBox2_TextChanged;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox3.Text))
            {
                var words = textBox3.Text.Split('-');
                for (int i = 0; i < words.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(words[i]))
                    {
                        words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                    }
                }

                textBox3.Text = string.Join("-", words);
                textBox3.SelectionStart = textBox3.Text.Length;
                textBox3.SelectionLength = 0;
                textBox3.TextChanged -= textBox3_TextChanged;
                textBox3.Text = textBox3.Text;
                textBox3.TextChanged += textBox3_TextChanged;
            }
        }

        private void AddClientForm_Load(object sender, EventArgs e)
        {
            dateTimePicker1.MaxDate = DateTime.Now;
            dateTimePicker1.MinDate = DateTime.Now.AddYears(-150);
            dateTimePicker1.CustomFormat = " ";
            button3.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClientsViewForm clientsViewForm = new ClientsViewForm();
            this.Visible = false;
            clientsViewForm.ShowDialog();
            this.Close();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";
        }

        /// <summary>
        /// Добавление нового клиента
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || dateTimePicker1.CustomFormat == " " || maskedTextBox1.MaskFull == false)
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string name = textBox1.Text;
                string surname = textBox2.Text;
                string patronymic = textBox3.Text;
                string phone = maskedTextBox1.Text;
                string dateBirth = dateTimePicker1.Text;
                List<string> clientPhone = new List<string>();

                try
                {
                    MySqlConnection connection = new MySqlConnection(Connection.conn);
                    connection.Open();

                    MySqlCommand command = new MySqlCommand("SELECT ClientPhone FROM client", connection);
                    MySqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        clientPhone.Add(dataReader.GetString(0));
                    }

                    if (!clientPhone.Contains(phone))
                    {
                        Data.InsertUpdateDeleteData($@"INSERT INTO client (ClientSurname, ClientName, ClientPatronymic, ClientPhone, ClientBirthday)
                        VALUES ('{name}', '{surname}', '{patronymic}', '{phone}', '{dateBirth}')");
                        MessageBox.Show("Клиент успешно добавлен!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        textBox1.Clear();
                        textBox2.Clear();
                        textBox3.Clear();
                        maskedTextBox1.Clear();
                        dateTimePicker1.CustomFormat = " ";
                        button3.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show("Клиент с таким номером телефона уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            button3.Enabled = true;
        }
    }
}
