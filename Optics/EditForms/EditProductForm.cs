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
using MySql.Data.MySqlClient;

namespace Optics
{
    public partial class EditProductForm : Form
    {
        public EditProductForm()
        {
            InitializeComponent();
        }

        public string fileName;
        public string fullPath;
        public string oldPhoto;
        private void EditProductForm_Load(object sender, EventArgs e)
        {
            button3.Enabled = false;
        }
        public EditProductForm(string id)
        {
            InitializeComponent();
            
            textBox2.ScrollBars = ScrollBars.Vertical;
            textBox7.ScrollBars = ScrollBars.Vertical;

            MySqlConnection connection = new MySqlConnection(Connection.conn);
            connection.Open();
            MySqlCommand command = new MySqlCommand("SELECT * FROM supplier", connection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                comboBox3.Items.Add(reader.GetValue(1).ToString());
            }
            connection.Close();

            MySqlConnection connection1 = new MySqlConnection(Connection.conn);
            connection1.Open();
            MySqlCommand command1 = new MySqlCommand("SELECT * FROM productcategory", connection1);
            MySqlDataReader reader1 = command1.ExecuteReader();
            while (reader1.Read())
            {
                comboBox1.Items.Add(reader1.GetValue(1).ToString());
            }
            connection1.Close();

            MySqlConnection connection2 = new MySqlConnection(Connection.conn);
            connection2.Open();
            MySqlCommand command2 = new MySqlCommand($@"SELECT ProductArticleNumber, ProductName, ProductUnit, ProductCost, 
            ProductManufacturer, ProductSupplier, ProductDiscountAmount, ProductQuantityInStock, ProductDescription, 
            ProductCategory, ProductPhoto, supplier.SupplierName, productcategory.ProductCategoryName
            FROM product 
            INNER JOIN supplier ON supplier.SupplierID = product.ProductSupplier
            INNER JOIN productcategory ON productcategory.ProductCategoryID = product.ProductCategory
            WHERE ProductArticleNumber = '{id}'", connection2);
            MySqlDataReader reader2 = command2.ExecuteReader();
            while (reader2.Read())
            {
                textBox1.Text = reader2[0].ToString();
                textBox2.Text = reader2[1].ToString();
                comboBox2.SelectedItem = reader2[2].ToString();
                textBox3.Text = reader2[3].ToString();
                textBox4.Text = reader2[4].ToString();
                textBox5.Text = reader2[6].ToString();
                textBox6.Text = reader2[7].ToString();
                textBox7.Text = reader2[8].ToString();
                comboBox3.SelectedItem = reader2[11].ToString();
                comboBox1.SelectedItem = reader2[12].ToString();
                oldPhoto = reader2[10].ToString();

                pictureBox1.ImageLocation = $@"./product/{oldPhoto}";
                if (oldPhoto == String.Empty)
                {
                    pictureBox1.ImageLocation = $@"./product/picture.png";
                }
            }
            connection2.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Validation.IsValidArticle(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void textBox3_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!Validation.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ProductsViewForm productsViewForm = new ProductsViewForm();
            this.Visible = false;
            productsViewForm.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button3.Enabled = true;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png";
                openFileDialog.Title = "Выберите изображение";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileInfo fileInfo = new FileInfo(openFileDialog.FileName);

                    if ((fileInfo.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                         fileInfo.Extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                         fileInfo.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase)) &&
                        fileInfo.Length <= 2 * 1024 * 1024)
                    {
                        pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                        fileName = fileInfo.Name;
                        fullPath = openFileDialog.FileName;
                    }
                    else
                    {
                        MessageBox.Show("Выберите файл JPG или PNG размером не более 2 Мб.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button3.Enabled = true;
            pictureBox1.ImageLocation = $@"./product/picture.png";
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "" && textBox1.Text != "" && textBox3.Text != "" && textBox4.Text != "" &&
                textBox6.Text != "" && textBox5.Text != "" && textBox7.Text != "" &&
                comboBox1.SelectedIndex != -1 && comboBox2.SelectedIndex != -1 && comboBox3.SelectedIndex != -1)
            {
                UpdateProduct();
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateProduct() {
            try
            {
                string article = textBox1.Text;
                string name = textBox2.Text;
                int categoryId = Data.GetID($"SELECT ProductCategoryID FROM productcategory WHERE ProductCategoryName = '{comboBox1.SelectedItem}'");
                string unit = comboBox2.SelectedItem.ToString();
                double cost = Convert.ToDouble(textBox3.Text);
                string manufacturer = textBox4.Text;
                int supplierId = Data.GetID($"SELECT SupplierID FROM supplier WHERE SupplierName = '{comboBox3.SelectedItem}'");
                int amount = Convert.ToInt32(textBox5.Text);
                int count = Convert.ToInt32(textBox6.Text);
                string description = textBox7.Text;
                string photoName = fileName;

                if (photoName != null)
                {
                    string dest = @"./product/" + photoName;
                    File.Copy(fullPath, dest, true);
                }
                else
                {
                    photoName = "";
                }

                bool res = Data.InsertUpdateDeleteData($@"UPDATE product
                SET ProductName = '{name}', ProductUnit = '{unit}', ProductCost = '{cost}',
                ProductManufacturer = '{manufacturer}', ProductSupplier = '{supplierId}', 
                ProductDiscountAmount = '{amount}',
                ProductQuantityInStock = '{count}', ProductDescription = '{description}', ProductCategory = '{categoryId}', 
                ProductPhoto = '{photoName}'
                WHERE ProductArticleNumber = '{article}'");

                MySqlConnection connection = new MySqlConnection(Connection.conn);
                connection.Open();
                if (res == true)
                {
                    MessageBox.Show("Товар успешно изменен!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    button3.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Товар с таким артикулом уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                connection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
