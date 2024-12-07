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
    public partial class AuthorizationForm : Form
    {
        public AuthorizationForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы действительно хотите выйти?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    Authorization();
                    textBox1.Clear();
                    textBox2.Clear();
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка авторизации! Такого пользователя не существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Clear();
                    textBox2.Clear();
                }
            }
        }
        string GetHashPass(string password)
        {
            using (var sh = SHA256.Create())
            {
                var shbyte = sh.ComputeHash(Encoding.UTF8.GetBytes(password));
                password = BitConverter.ToString(shbyte).Replace("-", "").ToLower();
            }

            return password;
        }

        private void Authorization()
        {
            string login = textBox1.Text;
            string password = GetHashPass(textBox2.Text);

            MySqlConnection connection = new MySqlConnection(Connection.conn);
            connection.Open();
            MySqlCommand command = new MySqlCommand($"SELECT * FROM user WHERE UserLogin = '{login}'", connection);

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            adapter.Fill(table);

            Data.userId = Convert.ToInt32(table.Rows[0].ItemArray.GetValue(0));
            Data.surname = table.Rows[0].ItemArray.GetValue(1).ToString();
            Data.name = table.Rows[0].ItemArray.GetValue(2).ToString();
            Data.patronymic = table.Rows[0].ItemArray.GetValue(3).ToString();
            Data.role = table.Rows[0].ItemArray.GetValue(6).ToString();
            string passwordBd = table.Rows[0].ItemArray.GetValue(5).ToString();

            if (password == passwordBd)
            {
                MessageBox.Show("Успешная авторизация!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);
                switch (Data.role)
                {
                    case "1":
                        AdminForm adminForm = new AdminForm();
                        this.Visible = false;
                        adminForm.ShowDialog();
                        this.Close();
                        break;

                    case "2":
                        ManagerForm managerForm = new ManagerForm();
                        this.Visible = false;
                        managerForm.ShowDialog();
                        this.Close();
                        break;
                }
            }
            else
            {
                MessageBox.Show("Ошибка авторизации! Неверные логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connection.Close();
        }

        private void AuthorizationForm_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox2.PasswordChar = default;
            }
            else
            {
                textBox2.PasswordChar = '*';
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Validation.IsValidPassword(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Validation.IsValidLogin(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            IdleTimeForm idleTimeForm = new IdleTimeForm();
            this.Visible = false;
            idleTimeForm.ShowDialog();
            this.Close();
        }
    }
}
