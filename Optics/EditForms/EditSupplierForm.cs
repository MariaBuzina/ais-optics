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
    public partial class EditSupplierForm : Form
    {
        public EditSupplierForm()
        {
            InitializeComponent();
        }

        private void EditSupplierForm_Load(object sender, EventArgs e)
        {
            button3.Enabled = false;
        }
        public int supplierID;
        public EditSupplierForm(int id)
        {
            InitializeComponent();
            try
            {
                MySqlConnection connection = new MySqlConnection(Connection.conn);
                connection.Open();
                MySqlCommand command = new MySqlCommand($@"SELECT * FROM supplier WHERE SupplierId = '{id}'", connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    supplierID = Convert.ToInt32(reader[0]);
                    textBox1.Text = reader[1].ToString();
                    textBox2.Text = reader[2].ToString();
                    maskedTextBox1.Text = reader[3].ToString(); 
                    textBox3.Text = reader[4].ToString();

                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SuppliersViewForm supplierViewForm = new SuppliersViewForm();
            this.Visible = false;
            supplierViewForm.ShowDialog();
            this.Close();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Validation.IsValidFIO(e.KeyChar) || char.IsWhiteSpace(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Validation.IsRussianLetter(e.KeyChar) || Validation.IsEnglishLetter(e.KeyChar) || Validation.IsDigit(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Validation.IsValidAddress(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            button3.Enabled = true;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
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
                    MySqlCommand cmd = new MySqlCommand($@"SELECT COUNT(*) FROM supplier
                    WHERE SupplierPhone = '{phone}' AND SupplierId <> '{supplierID}'", connection);

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

                try
                {
                    if (IsPhoneExists(phone))
                    {
                        MessageBox.Show("Поставщик с таким номером телефона уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Data.InsertUpdateDeleteData($@"UPDATE supplier SET SupplierName = '{name}', SupplierContactPerson = '{fio}', 
                    SupplierPhone = '{phone}', SupplierAddress = '{address}' WHERE SupplierId = '{supplierID}')");

                    MessageBox.Show("Поставщик успешно изменен!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
