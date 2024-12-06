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
    public partial class AddSupplierForm : Form
    {
        public AddSupplierForm()
        {
            InitializeComponent();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!((Validation.IsValidFIO(e.KeyChar)) || Char.IsWhiteSpace(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox2.Text))
            {
                var words = textBox2.Text.Split(' ');
                for (int i = 0; i < words.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(words[i]))
                    {
                        words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                    }
                }
                textBox2.Text = string.Join(" ", words);
                textBox2.SelectionStart = textBox2.Text.Length;
                textBox2.SelectionLength = 0;
                textBox2.TextChanged -= textBox2_TextChanged;
                textBox2.Text = textBox2.Text;
                textBox2.TextChanged += textBox2_TextChanged;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SuppliersViewForm supplierViewForm = new SuppliersViewForm();
            this.Visible = false;
            supplierViewForm.ShowDialog();
            this.Close();
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Validation.IsValidAddress(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "г. ул. д." || textBox3.Text == "" || maskedTextBox1.MaskFull == false)
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string name = textBox1.Text;
                string fio = textBox2.Text;
                string address = textBox3.Text;
                string phone = maskedTextBox1.Text;
                List<string> supplierPhone = new List<string>();

                try
                {
                    MySqlConnection connection = new MySqlConnection(Connection.conn);
                    connection.Open();

                    MySqlCommand command = new MySqlCommand("SELECT SupplierPhone FROM supplier", connection);
                    MySqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        supplierPhone.Add(dataReader.GetString(0));
                    }

                    if (!supplierPhone.Contains(phone))
                    {
                        Data.InsertUpdateDeleteData($@"INSERT INTO supplier (SupplierName, SupplierContactPerson, SupplierPhone, SupplierAddress)
                        VALUES ('{name}','{fio}','{phone}','{address}')");

                        MessageBox.Show("Поставщик успешно добавлен!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        textBox1.Clear();
                        textBox2.Clear();
                        textBox3.Clear();
                        maskedTextBox1.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Поставщик с таким номером телефона уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    connection.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AddSupplierForm_Load(object sender, EventArgs e)
        {
            button3.Enabled = false;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }
    }
}
