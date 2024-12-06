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
    public partial class ViewProductForm : Form
    {
        public ViewProductForm()
        {
            InitializeComponent();
        }

        private void ViewProductForm_Load(object sender, EventArgs e)
        {

        }

        public string oldPhoto;
        public ViewProductForm(string id)
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

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
