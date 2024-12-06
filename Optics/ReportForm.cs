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
using Word = Microsoft.Office.Interop.Word;
using MySql.Data.MySqlClient;

namespace Optics
{
    public partial class ReportForm : Form
    {
        private readonly string fileName = Directory.GetCurrentDirectory() + @"\doc\otchet.docx";
        public ReportForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ManagerForm managerForm = new ManagerForm();
            this.Visible = false;
            managerForm.ShowDialog();
            this.Close();
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var word = new Word.Application();
                word.Visible = false;

                try
                {
                    var wordDocument = word.Documents.Add(fileName);

                    ReplaceWordStub("{date}", DateTime.Now.ToString(), wordDocument);

                    // получаем таблицу из документа
                    Word.Table table = wordDocument.Tables[1];

                    // определяем начальную строку для вставки данных
                    int rowIndex = 2; // пропускаем первую строку

                    // получаем количество записей из таблицы product
                    MySqlConnection connection1 = new MySqlConnection(Connection.conn);
                    connection1.Open();
                    MySqlCommand command1 = new MySqlCommand("SELECT COUNT(*) FROM product", connection1);
                    int res = Convert.ToInt32(command1.ExecuteScalar());
                    connection1.Close();

                    // создаем отчет
                    MySqlConnection connection = new MySqlConnection(Connection.conn);
                    connection.Open();
                    MySqlCommand command = new MySqlCommand($@"SELECT ProductArticleNumber, ProductName, ProductSupplier,
                    ProductQuantityInStock, supplier.SupplierName AS 'SupplierName' FROM product
                    INNER JOIN supplier ON supplier.SupplierID = product.ProductSupplier", connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string article = reader[0].ToString();
                        string product = reader[1].ToString();
                        string count = reader[3].ToString();
                        string supplier = reader[4].ToString();

                        table.Cell(rowIndex, 1).Range.Text = article;
                        table.Cell(rowIndex, 2).Range.Text = product;
                        table.Cell(rowIndex, 3).Range.Text = supplier;
                        table.Cell(rowIndex, 4).Range.Text = count;

                        // добавление новой строки таблицы
                        if (table.Rows.Count - 1 < res)
                        {
                            table.Rows.Add();
                        }

                        rowIndex++;
                    }
                    connection.Close();

                word.Visible = true;
            }
                catch (Exception ex)
                {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            comboBox1.SelectedIndex = -1;
            button1.Enabled = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }
        private void ReplaceWordStub(string stubToReplace, string text, Word.Document wordDocument)
        {
            var range = wordDocument.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: stubToReplace, ReplaceWith: text);
        }
    }
}
