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
using Word = Microsoft.Office.Interop.Word;

namespace Optics
{
    public partial class AddOrderForm : Form
    {
        private readonly string fileName = Directory.GetCurrentDirectory() + @"\doc\cheque.docx";
        public AddOrderForm()
        {
            InitializeComponent();
        }

        Dictionary<string, int> bucket;
        
        private void AddOrderForm_Load(object sender, EventArgs e)
        {
            label10.Text = DateTime.Now.ToString("yyyy-MM-dd");
            MySqlConnection connection = new MySqlConnection(Connection.conn);
            connection.Open();
            MySqlCommand command = new MySqlCommand("SELECT * FROM client", connection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader.GetValue(1).ToString() + " " + reader.GetValue(2).ToString() + " " + reader.GetValue(3).ToString() + " " + reader.GetValue(4).ToString());
            }
            connection.Close();

            textBox1.Text = Data.surname + " " + Data.name + " " + Data.patronymic;
            this.bucket = Data.MyBucket;
            string where = " ProductArticleNumber IN ('" + string.Join("', '", bucket.Keys.ToArray()) + "') ";
            FillDataGridView(where);
            CalculateSums();
        }

        /// <summary>
        /// Заполнение datagridview данными
        /// </summary>
        /// <param name="where">Строка с sql запросом</param>
        void FillDataGridView(string where = "")
        {
            dataGridView1.ClearSelection();
            string cmdStr = @"SELECT ProductArticleNumber, ProductName, ProductCost, ProductDiscountAmount FROM product WHERE" + where;

            MySqlConnection con = new MySqlConnection(Connection.conn);
            con.Open();
            MySqlCommand cmd = new MySqlCommand(cmdStr, con);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable table = new DataTable();
            da.Fill(table);

            table.Columns["ProductArticleNumber"].ColumnName = "Артикул";
            table.Columns["ProductName"].ColumnName = "Наименование";
            table.Columns["ProductCost"].ColumnName = "Цена";
            table.Columns["ProductDiscountAmount"].ColumnName = "Скидка";

            dataGridView1.DataSource = table;

            dataGridView1.Columns.Add("Count", "Количество");
            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.Rows[0].Cells[0].Selected = false;

            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                string ProductArticleNumber = r.Cells["Артикул"].Value.ToString();
                r.Cells["Count"].Value = bucket[ProductArticleNumber];
            }
            con.Close();
        }

        double totalWithDiscount = 0;
        /// <summary>
        /// Метод для нахождения суммы заказа со скидкой и без
        /// </summary>
        private void CalculateSums()
        {
            double totalWithoutDiscount = 0;
            int allCount = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Цена"].Value != null && row.Cells["Скидка"].Value != null)
                {
                    double price = Convert.ToDouble(row.Cells["Цена"].Value);
                    double discount = Convert.ToDouble(row.Cells["Скидка"].Value);
                    int count = Convert.ToInt32(row.Cells["Count"].Value);

                    // Считаем общую сумму без скидки
                    totalWithoutDiscount += price * count;

                    // Считаем сумму со скидкой
                    double discountedPrice = price - (price * discount / 100);
                    totalWithDiscount += discountedPrice * count;

                    // общее количество товара
                    allCount += count;
                }
            }

            label6.Text = totalWithoutDiscount.ToString() + " руб.";
            label7.Text = totalWithDiscount.ToString() + " руб."; 
            label9.Text = allCount.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                int r = e.RowIndex;
                dataGridView1.Rows[r].Selected = true;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    string orderDate = label10.Text;
                    double orderSum = totalWithDiscount;
                    int userId = Data.userId;

                    string client = comboBox1.SelectedItem.ToString();
                    string[] clients = client.Split();
                    string phone = $@"{clients[3]} {clients[4]} {clients[5]}";
                    string cmdClient = $@"SELECT ClientId FROM client WHERE ClientPhone = '{phone}';";

                    // добавление заказа order
                    MySqlConnection con = new MySqlConnection(Connection.conn);
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(cmdClient, con);

                    //order
                    int clientID = (int)cmd.ExecuteScalar();//id пункта выдачи
                    string cmdOrder = string.Format(@"INSERT INTO `order`(OrderDate, ProductUser, OrderClient, OrderAmount) 
                                                VALUES('{0}', '{1}', '{2}', '{3}');", orderDate, userId, clientID, orderSum);
                    // узнаем ID последней добавленной записи
                    string cmdLastId = "SELECT last_insert_id();";
                    cmd.CommandText = cmdOrder + cmdLastId;
                    string OrderID = cmd.ExecuteScalar().ToString();

                    // добавление товаров в заказ orderproduct
                    string cmdOrderProduct = @"INSERT INTO orderproduct VALUES ";
                    string productArticleNumber, orderProductCount;

                    foreach (var item in bucket)
                    {
                        productArticleNumber = item.Key.ToString();
                        orderProductCount = item.Value.ToString();
                        cmdOrderProduct += string.Format("({0},'{1}',{2}),", OrderID, productArticleNumber, orderProductCount);
                    }

                    cmd.CommandText = cmdOrderProduct.Substring(0, cmdOrderProduct.Length - 1);
                    cmd.ExecuteNonQuery();
                    con.Close();

                    // обновляем количество товара на складе
                    MySqlConnection connection = new MySqlConnection(Connection.conn);
                    connection.Open();
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string productArticle = row.Cells["Артикул"].Value.ToString();
                        int count = Convert.ToInt32(row.Cells["Count"].Value);

                        int countInStock = GetProductQuantityInStock(connection, productArticle);

                        if (countInStock >= count)
                        {
                            Data.InsertUpdateDeleteData($@"UPDATE product SET 
                                                    ProductQuantityInStock = '{countInStock - count} '
                                                    WHERE ProductArticleNumber = '{productArticle}'");
                        }
                        else
                        {
                            MessageBox.Show($"Недостаточно товара для {productArticle}! Текущий запас: {countInStock}.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    connection.Close();

                }
                catch (Exception ex)
                {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            MessageBox.Show("Заказ сформирован!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // создание чека
                DialogResult result = MessageBox.Show("Создать чек?", "Сообщение пользователю", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        var word = new Word.Application();
                        word.Visible = false;

                        try
                        {
                            var wordDocument = word.Documents.Add(fileName);

                            string client = comboBox1.SelectedItem.ToString();
                            string[] clients = client.Split();
                            string fio = $@"{clients[0]} {clients[1]} {clients[2]}";

                            ReplaceWordStub("{date}", DateTime.Now.ToString(), wordDocument);
                            ReplaceWordStub("{manager}", textBox1.Text, wordDocument);
                            ReplaceWordStub("{client}", fio, wordDocument);
                            ReplaceWordStub("{count}", label9.Text, wordDocument);
                            ReplaceWordStub("{sum}", label7.Text, wordDocument);

                            // получаем таблицу из документа
                            Word.Table table = wordDocument.Tables[1];

                            // определяем начальную строку для вставки данных
                            int rowIndex = 2; // пропускаем первую строку

                            int rowCount = dataGridView1.Rows.Count;
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                // получаем данные из ячеек
                                string product = row.Cells["Наименование"].Value.ToString();
                                string cost = row.Cells["Цена"].Value.ToString();
                                string amount = row.Cells["Скидка"].Value.ToString();
                                string countProduct = row.Cells["Count"].Value.ToString();

                                // вставляем данные в таблицу
                                table.Cell(rowIndex, 1).Range.Text = product;
                                table.Cell(rowIndex, 2).Range.Text = cost;
                                table.Cell(rowIndex, 3).Range.Text = amount;
                                table.Cell(rowIndex, 4).Range.Text = countProduct;

                                // добавление новой строки таблицы
                                if (table.Rows.Count - 1 < rowCount)
                                {
                                    table.Rows.Add();
                                }

                                rowIndex++;
                            }

                            word.Visible = true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                Data.MyBucket.Clear();

                AddOrderProductForm addOrderProductForm = new AddOrderProductForm();
                this.Visible = false;
                addOrderProductForm.ShowDialog();
                this.Close();
            }
        }

        /// <summary>
        /// Получаем количество товара на складе
        /// </summary>
        /// <param name="productArticleNumber">Артикул товара</param>
        /// <returns></returns>
        private int GetProductQuantityInStock(MySqlConnection connection, string productArticleNumber)
        {
            MySqlCommand command = new MySqlCommand($@"SELECT ProductQuantityInStock FROM product 
                                                   WHERE ProductArticleNumber = '{productArticleNumber}'", connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }
        private void ReplaceWordStub(string stubToReplace, string text, Word.Document wordDocument)
        {
            var range = wordDocument.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: stubToReplace, ReplaceWith: text);
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Validation.IsRussianLetter(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
