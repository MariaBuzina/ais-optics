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
    public partial class AddOrderProductForm : Form
    {
        public AddOrderProductForm()
        {
            InitializeComponent();
        }

        public void FillProductCategory()
        {
            MySqlConnection connection = new MySqlConnection(Connection.conn);
            connection.Open();
            MySqlCommand command = new MySqlCommand("SELECT * FROM productcategory", connection);
            MySqlDataReader reader = command.ExecuteReader();

            comboBox2.Items.Clear();
            comboBox2.Items.Add("Все типы");

            while (reader.Read())
            {
                comboBox2.Items.Add(reader[1].ToString());
            }

            comboBox2.SelectedIndex = 0;
            connection.Close();
        }

        public void FillSort()
        {
            comboBox3.Items.Clear();
            comboBox3.Items.Add("Наименование");
            comboBox3.Items.Add("Цена");
            comboBox3.Items.Add("Количество");

            comboBox1.SelectedIndex = 0;
        }
        public void FillFiltr()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Сортировать по");
            comboBox1.Items.Add("По возрастанию");
            comboBox1.Items.Add("По убыванию");

            comboBox1.SelectedIndex = 0;
        }
        private void AddOrderProductForm_Load(object sender, EventArgs e)
        {
            FillDataGridView();
            FillProductCategory();
            FillFiltr();
            FillSort();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ManagerForm managerForm = new ManagerForm();
            this.Visible = false;
            managerForm.ShowDialog();
            this.Close();
        }

        int allPages = 1;
        int limitPages = 20;
        int currentPage = 1;
        int allRecords = 0;

        /// <summary>
        /// Заполнение данными datagridview
        /// </summary>
        /// <param name="cmd">SQl запрос</param>
        public void FillDataGridView(string cmd = "")
        {
            dataGridView1.Columns.Clear();

            string com = @"SELECT ProductArticleNumber, ProductName, ProductUnit, ProductCost, ProductManufacturer, ProductSupplier, 
            ProductDiscountAmount, ProductQuantityInStock, ProductDescription, ProductCategory, ProductPhoto, 
            productcategory.ProductCategoryName AS 'ProductCategoryName' FROM product
            INNER JOIN productcategory ON product.ProductSupplier = productcategory.ProductCategoryID";

            if (cmd != String.Empty)
            {
                com += $" AND (product.ProductName LIKE '%{cmd}%')";
            }

            if (comboBox2.SelectedIndex != 0 && comboBox2.SelectedIndex != -1)
            {
                com += $" WHERE ProductCategoryName = '{comboBox2.SelectedItem}'";
            }

            if (comboBox1.SelectedIndex != 0 && comboBox1.SelectedIndex != -1 && comboBox3.SelectedIndex == 0)
            {
                com += " ORDER BY product.ProductName";
                com += comboBox1.SelectedItem.ToString() == "По возрастанию" ? $" ASC" : $" DESC";
            }
            else if (comboBox1.SelectedIndex != 0 && comboBox1.SelectedIndex != -1 && comboBox3.SelectedIndex == 1)
            {
                com += " ORDER BY product.ProductCost";
                com += comboBox1.SelectedItem.ToString() == "По возрастанию" ? $" ASC" : $" DESC";
            }
            else if (comboBox1.SelectedIndex != 0 && comboBox1.SelectedIndex != -1 && comboBox3.SelectedIndex == 2)
            {
                com += " ORDER BY product.ProductQuantityInStock";
                com += comboBox1.SelectedItem.ToString() == "По возрастанию" ? $" ASC" : $" DESC";
            }

            MySqlConnection connection = new MySqlConnection(Connection.conn);
            connection.Open();

            MySqlCommand mySqlCommand = new MySqlCommand("SELECT COUNT(*) FROM product", connection);
            allRecords = Convert.ToInt32(mySqlCommand.ExecuteScalar());
            allPages = (int)Math.Ceiling(allRecords / (double)limitPages);

            int offset = (currentPage - 1) * limitPages;
            com += $" LIMIT {limitPages} OFFSET {offset}";

            MySqlCommand command = new MySqlCommand(com, connection);
            command.ExecuteNonQuery();

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            adapter.Fill(table);

            table.Columns["ProductName"].ColumnName = "Наименование";
            table.Columns["ProductCategoryName"].ColumnName = "Категория";
            table.Columns["ProductManufacturer"].ColumnName = "Производитель";
            table.Columns["ProductCost"].ColumnName = "Цена";
            table.Columns["ProductQuantityInStock"].ColumnName = "Количество";

            dataGridView1.DataSource = table;

            dataGridView1.Columns["ProductArticleNumber"].Visible = false;
            dataGridView1.Columns["ProductUnit"].Visible = false;
            dataGridView1.Columns["ProductSupplier"].Visible = false;
            dataGridView1.Columns["ProductDescription"].Visible = false;
            dataGridView1.Columns["ProductCategory"].Visible = false;
            dataGridView1.Columns["ProductPhoto"].Visible = false;
            dataGridView1.Columns["ProductDiscountAmount"].Visible = false;

            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
            imageColumn.Name = "Фото";
            imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;

            dataGridView1.Columns.Add(imageColumn);
            dataGridView1.AllowUserToAddRows = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string name = row.Cells["ProductPhoto"].Value.ToString();

                if (name == "")
                {
                    name = "picture.png";
                }
                row.Cells["Фото"].Value = Image.FromFile(@"./product/" + name);
            }

            DataGridViewButtonColumn buttonColumn1 = new DataGridViewButtonColumn();
            dataGridView1.Columns.Add(buttonColumn1);
            buttonColumn1.UseColumnTextForButtonValue = true;
            buttonColumn1.Text = "Добавить";

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            connection.Close();
            UpdatePagination();
        }

        private void UpdatePagination()
        {
            label4.Text = $"{dataGridView1.Rows.Count}/{allRecords}";
            pictureBox2.Visible = currentPage < allPages;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                int r = e.RowIndex;
                dataGridView1.Rows[r].Selected = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            FillDataGridView(textBox1.Text);
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Скидка")
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal discount))
                {
                    if (discount >= 15)
                    {
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(223, 240, 254);
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                    }
                    else
                    {
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataGridView(textBox1.Text);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataGridView(textBox1.Text);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataGridView(textBox1.Text);
        }

        Dictionary<string, int> bucket = new Dictionary<string, int>();

        /// <summary>
        /// Добавление товара в заказ
        /// </summary>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns[""].Index)
            {
                string ProductArticleNumber = dataGridView1.Rows[e.RowIndex].Cells["ProductArticleNumber"].Value.ToString();
                int count = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Количество"].Value);

                if (count > 0)
                {
                    count--;
                    dataGridView1.Rows[e.RowIndex].Cells["Количество"].Value = count;

                    try
                    {
                        bucket.Add(ProductArticleNumber, 1);
                    }
                    catch (ArgumentException)
                    {
                        bucket[ProductArticleNumber] = bucket[ProductArticleNumber] + 1;
                    }

                    button3.Visible = true;
                    button3.Text = $"Оформить заказ ({bucket.Count})";
                }
                else
                {
                    MessageBox.Show("Недостаточное количество товара!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Data.MyBucket = bucket;

            AddOrderForm addOrderForm = new AddOrderForm();
            this.Hide();
            addOrderForm.ShowDialog();
            this.Show();
            bucket = Data.MyBucket;
        }

        int currentRowIndex;
        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                m.MenuItems.Add(new MenuItem("Показать польностью", contextmenu_click)); //Имя пункта меню, с указание обработкича события(нажатие на меню)

                //получение идекса выбранной строки по координатам мыши
                this.currentRowIndex = dataGridView1.HitTest(e.X, e.Y).RowIndex;
                dataGridView1.Rows[currentRowIndex].Selected = true;

                m.Show(dataGridView1, new Point(e.X, e.Y));
            }
        }
        void contextmenu_click(object sender, EventArgs e)
        {
            string ProductArticleNumber = dataGridView1.Rows[currentRowIndex].Cells["ProductArticleNumber"].Value.ToString();
            dataGridView1.Rows[currentRowIndex].Selected = false;

            ViewProductForm viewProductForm = new ViewProductForm(ProductArticleNumber);
            viewProductForm.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (currentPage < allPages)
            {
                currentPage++;
                FillDataGridView();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                FillDataGridView();
            }
        }
    }
}
