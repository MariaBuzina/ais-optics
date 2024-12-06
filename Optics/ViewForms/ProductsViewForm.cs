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
    public partial class ProductsViewForm : Form
    {
        public ProductsViewForm()
        {
            InitializeComponent();
        }

        private void ProductsViewForm_Load(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.ClearSelection();
                FillDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void FillDataGridView()
        {
            dataGridView1.Columns.Clear();

            MySqlConnection connection = new MySqlConnection(Connection.conn);
            connection.Open();
            MySqlCommand command = new MySqlCommand(@"SELECT ProductArticleNumber, ProductName, ProductUnit, ProductCost, ProductManufacturer, ProductSupplier, 
            ProductDiscountAmount, ProductQuantityInStock, ProductDescription, ProductCategory, ProductPhoto, 
            productcategory.ProductCategoryName AS 'ProductCategoryName', supplier.SupplierName AS 'SupplierName' FROM product
            INNER JOIN productcategory ON product.ProductSupplier = productcategory.ProductCategoryID
            INNER JOIN supplier ON product.ProductSupplier = supplier.SupplierID
            ", connection);
            command.ExecuteNonQuery();

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            adapter.Fill(table);

            table.Columns["ProductName"].ColumnName = "Наименование";
            table.Columns["ProductCategoryName"].ColumnName = "Категория";
            table.Columns["ProductManufacturer"].ColumnName = "Производитель";
            table.Columns["SupplierName"].ColumnName = "Поставщик";
            table.Columns["ProductCost"].ColumnName = "Цена";
            table.Columns["ProductDiscountAmount"].ColumnName = "Скидка";

            dataGridView1.DataSource = table;

            dataGridView1.Columns["ProductArticleNumber"].Visible = false;
            dataGridView1.Columns["ProductUnit"].Visible = false;
            dataGridView1.Columns["ProductQuantityInStock"].Visible = false;
            dataGridView1.Columns["ProductDescription"].Visible = false;
            dataGridView1.Columns["ProductCategory"].Visible = false;
            dataGridView1.Columns["ProductSupplier"].Visible = false;
            dataGridView1.Columns["ProductPhoto"].Visible = false;

            dataGridView1.Rows[0].Cells[0].Selected = false;

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
            buttonColumn1.Text = "Изменить";

            DataGridViewButtonColumn buttonColumn2 = new DataGridViewButtonColumn();
            dataGridView1.Columns.Add(buttonColumn2);
            buttonColumn2.UseColumnTextForButtonValue = true;
            buttonColumn2.Text = "Удалить";

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            connection.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddProductForm addProductForm = new AddProductForm();
            this.Visible = false;
            addProductForm.ShowDialog();
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex != -1)
                {
                    int r = e.RowIndex, c = e.ColumnIndex;
                    string id = dataGridView1.Rows[r].Cells[0].Value.ToString();

                    switch (c)
                    {
                        case 14:
                            EditProductForm editProductForm = new EditProductForm(id);
                            this.Visible = false;
                            editProductForm.ShowDialog();
                            this.Close();
                            break;
                        case 15:
                            DialogResult result = MessageBox.Show("Вы действительно хотите удалить запись?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                try
                                {
                                    bool res = Data.InsertUpdateDeleteData($"DELETE FROM product WHERE ProductArticleNumber = '{id}'");
                                    if (res == true)
                                    {
                                        Data.InsertUpdateDeleteData($"DELETE FROM product WHERE ProductArticleNumber = '{id}'");
                                        MessageBox.Show("Товар успешно удален!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        FillDataGridView();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Товар не может быть удален, так как он используется в других таблицах.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
}

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                int r = e.RowIndex;
                dataGridView1.Rows[r].Selected = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AdminForm adminForm = new AdminForm();
            this.Visible = false;
            adminForm.ShowDialog();
            this.Close();
        }
    }
}
