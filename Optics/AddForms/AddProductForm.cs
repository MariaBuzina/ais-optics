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
    public partial class AddProductForm : Form
    {
        public AddProductForm()
        {
            InitializeComponent();
        }

        private void AddProductForm_Load(object sender, EventArgs e)
        {
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

            pictureBox1.ImageLocation = $@"./product/picture.png";
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Validation.IsValidArticle(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
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

        public string fileName;
        public string fullPath;
        private void button1_Click(object sender, EventArgs e)
        {
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
            pictureBox1.ImageLocation = $@"./product/picture.png";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = GenerationData.GenerationArticle();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "" && textBox1.Text != "" && textBox3.Text != "" && textBox4.Text != "" &&
                textBox6.Text != "" && textBox5.Text != "" && textBox7.Text != "" &&
                comboBox1.SelectedIndex != -1 && comboBox2.SelectedIndex != -1 && comboBox3.SelectedIndex != -1)
            {
                AddProduct();
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void AddProduct()
        {
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
                    if (File.Exists(dest) == false)
                    {
                        File.Copy(fullPath, dest, true);
                        string[] vs = photoName.Split('.');
                        string newName = textBox1.Text.Trim() + $".{vs[1]}";
                        File.Move(dest, @"./product/" + newName);
                        photoName = newName;
                    }
                }
                else
                {
                    photoName = "";
                }

                bool res = Data.InsertUpdateDeleteData($@"INSERT INTO product
                           (ProductArticleNumber, ProductName, ProductUnit, ProductCost, ProductManufacturer, ProductSupplier,
                           ProductDiscountAmount, ProductQuantityInStock, ProductDescription, ProductCategory, ProductPhoto) VALUES
                           ('{article}', '{name}', '{unit}', '{cost}', '{manufacturer}',
                           '{supplierId}', '{amount}', '{count}', '{description}', '{categoryId}', '{photoName}')");

                MySqlConnection connection = new MySqlConnection(Connection.conn);
                connection.Open();
                if (res == true)
                {
                    MessageBox.Show("Товар успешно добавлен!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox4.Clear();
                    textBox5.Clear();
                    textBox6.Clear();
                    textBox7.Clear();
                    comboBox1.SelectedIndex = -1;
                    comboBox2.SelectedIndex = -1;
                    comboBox3.SelectedIndex = -1;
                    pictureBox1.ImageLocation = $@"./product/picture.png";
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

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
        }
    }
}
