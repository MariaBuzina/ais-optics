using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Optics
{
    public partial class AddUserForm : Form
    {
        public AddUserForm()
        {
            InitializeComponent();
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
        }
        private void AddUserForm_Load(object sender, EventArgs e)
        {
            button3.Enabled = false;
            MySqlConnection connection = new MySqlConnection(Connection.conn);
            connection.Open();
            MySqlCommand command = new MySqlCommand("SELECT * FROM role", connection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader.GetValue(1));
            }
            connection.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            UsersViewForm usersViewForm = new UsersViewForm();
            this.Visible = false;
            usersViewForm.ShowDialog();
            this.Close();
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

        private void button5_Click(object sender, EventArgs e)
        {
            textBox4.Text = GenerationData.GenerationLogin();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            textBox5.Text = GenerationData.GenerationPassword();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "" || comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    string surname = textBox1.Text;
                    string name = textBox2.Text;
                    string patronymic = textBox3.Text;
                    string login = textBox4.Text;
                    string password = Data.GetHashPass(textBox5.Text.ToString());
                    int role = Data.GetID($"SELECT RoleID FROM role WHERE RoleName = '{comboBox1.SelectedItem}'");
                    List<string> loginName = new List<string>();

                    MySqlConnection connection = new MySqlConnection(Connection.conn);
                    connection.Open();

                    MySqlCommand command = new MySqlCommand("SELECT UserLogin FROM user", connection);
                    MySqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        loginName.Add(dataReader.GetString(0));
                    }

                    if (!loginName.Contains(textBox4.Text))
                    {
                        Data.InsertUpdateDeleteData($@"INSERT INTO user(UserSurname, UserName, UserPatronymic, UserLogin, UserPassword, UserRole)
                                                               VALUES ('{surname}', '{name}', '{patronymic}', '{login}', '{password}', '{role}')");
                        MessageBox.Show("Пользователь успешно добавлен!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        textBox1.Clear();
                        textBox2.Clear();
                        textBox3.Clear();
                        textBox4.Clear();
                        textBox5.Clear();
                        comboBox1.SelectedIndex = -1;
                        button3.Enabled = false;

                    }
                    else
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Validation.IsValidLogin(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Validation.IsValidPassword(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox5.PasswordChar = default;
            }
            else
            {
                textBox5.PasswordChar = '*';
            }
        }
    }
}
