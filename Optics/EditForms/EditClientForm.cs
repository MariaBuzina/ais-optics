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
    public partial class EditClientForm : Form
    {
        public EditClientForm()
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
            button3.Enabled = true;
        }

        private void EditClientForm_Load(object sender, EventArgs e)
        {
            dateTimePicker1.MaxDate = DateTime.Now;
            dateTimePicker1.MinDate = DateTime.Now.AddYears(-120);
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            button3.Enabled = false;
        }

        public int clientID;
        public EditClientForm(int id)
        {
            InitializeComponent();
            try
            {
                MySqlConnection connection = new MySqlConnection(Connection.conn);
                connection.Open();
                MySqlCommand command = new MySqlCommand($@"SELECT * FROM client WHERE ClientId = '{id}'", connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    clientID = Convert.ToInt32(reader[0]);
                    textBox1.Text = reader[1].ToString();
                    textBox2.Text = reader[2].ToString();
                    textBox3.Text = reader[3].ToString();
                    maskedTextBox1.Text = reader[4].ToString();
                    DateTime dateTime = Convert.ToDateTime(reader[5]);
                    dateTimePicker1.Value = dateTime;
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            button3.Enabled = true;
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
            button3.Enabled = true;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            button3.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClientsViewForm clientsViewForm = new ClientsViewForm();
            this.Visible = false;
            clientsViewForm.ShowDialog();
            this.Close();
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            button3.Enabled = true;
        }

        private bool IsPhoneExists(string phone)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Connection.conn))
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand($@"SELECT COUNT(*) FROM client
                    WHERE ClientPhone = '{phone}' AND ClientId <> '{clientID}'", connection);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

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

                try
                {
                    if (IsPhoneExists(phone))
                    {
                        MessageBox.Show("Клиент с таким номером телефона уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Data.InsertUpdateDeleteData($@"UPDATE client SET ClientSurname = '{name}', ClientName = '{surname}', 
                        ClientPatronymic = '{patronymic}', ClientPhone, ClientBirthday)
                        VALUES (  '{patronymic}', '{phone}', '{dateBirth}')  WHERE ClientId = '{clientID}'");
                        MessageBox.Show("Клиент успешно изменен!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        button3.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
