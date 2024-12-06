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
    public partial class EditUserForm : Form
    {
        public EditUserForm()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            UsersViewForm usersViewForm = new UsersViewForm();
            this.Visible = false;
            usersViewForm.ShowDialog();
            this.Close();
        }

        private void EditUserForm_Load(object sender, EventArgs e)
        {
            button3.Enabled = false;
        }

        public int userID;
        public string oldPwd;
        public EditUserForm(int id)
        {
            InitializeComponent();

            MySqlConnection connection1 = new MySqlConnection(Connection.conn);
            connection1.Open();
            MySqlCommand command1 = new MySqlCommand("SELECT * FROM role", connection1);
            MySqlDataReader reader1 = command1.ExecuteReader();
            while (reader1.Read())
            {
                comboBox1.Items.Add(reader1.GetValue(1));
            }
            connection1.Close();

            MySqlConnection connection = new MySqlConnection(Connection.conn);
            connection.Open();
            MySqlCommand command = new MySqlCommand($@"SELECT UserID, UserSurname, UserName, UserPatronymic, UserLogin, 
            UserPassword, UserRole, role.RoleName FROM user 
            INNER JOIN role ON role.RoleID = user.UserRole
            WHERE UserID = '{id}';", connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                userID = Convert.ToInt32(reader[0]);
                textBox1.Text = reader[1].ToString();
                textBox2.Text = reader[2].ToString();
                textBox3.Text = reader[3].ToString();
                textBox4.Text = reader[4].ToString();
                oldPwd = reader[5].ToString();
                comboBox1.SelectedItem = reader[7].ToString();
            }
            connection.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox4.Text = GenerationData.GenerationLogin();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
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
            button3.Enabled = true;
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
            button3.Enabled = true;
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

        private void button1_Click(object sender, EventArgs e)
        {
            textBox5.Text = GenerationData.GenerationPassword();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }
        private bool IsLoginExists(string login)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Connection.conn))
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand($@"SELECT COUNT(*) FROM user
                    WHERE UserLogin = '{login}' AND UserID <> '{userID}'", connection);

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
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    int role = Data.GetID($"SELECT RoleID FROM role WHERE RoleName = '{comboBox1.SelectedItem}'");
                    string newPwd = Data.GetHashPass(textBox5.Text);

                    string sqlWithNewPwd = $@"UPDATE user SET UserSurname = '{textBox1.Text}', UserName = '{textBox2.Text}',
                    UserPatronymic = '{textBox3.Text}', UserLogin = '{textBox4.Text}', UserPassword = '{newPwd}', 
                    UserRole = '{role}' WHERE UserID = '{userID}'";
                    string sqlWithOldPwd = $@"UPDATE user SET UserSurname = '{textBox1.Text}', UserName = '{textBox2.Text}',
                    UserPatronymic = '{textBox3.Text}', UserLogin = '{textBox4.Text}', UserPassword = '{oldPwd}', 
                    UserRole = '{role}' WHERE UserID = '{userID}'";

                    if (IsLoginExists(textBox4.Text))
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (oldPwd != newPwd)
                    {
                        if (textBox5.Text == "")
                        {
                            Data.InsertUpdateDeleteData(sqlWithOldPwd);
                        }
                        else
                        {
                            Data.InsertUpdateDeleteData(sqlWithNewPwd);
                        }

                        MessageBox.Show("Пользователь успешно изменен!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                        button3.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show("Пароль не может совпадать с предыдущим!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
