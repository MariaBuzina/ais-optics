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
    public partial class ViewOrderProduct : Form
    {
        public ViewOrderProduct()
        {
            InitializeComponent();
        }
        public ViewOrderProduct(int orderID)
        {
            InitializeComponent();
            dataGridView1.ClearSelection();

            MySqlConnection connection = new MySqlConnection(Connection.conn);
            connection.Open();
            MySqlCommand command = new MySqlCommand($@"SELECT OrderID, OrderDate, ProductUser, 
            OrderClient, OrderAmount, user.UserSurname, user.UserName, user.UserPatronymic,
            client.ClientSurname, client.ClientName, client.ClientPatronymic FROM `order`
            INNER JOIN user ON user.UserID = `order`.ProductUser
            INNER JOIN client ON client.ClientID = `order`.OrderClient
            WHERE OrderID = {orderID}", connection);

            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                label8.Text = reader[0].ToString();
                DateTime dateTime = reader.GetDateTime(1);
                label7.Text = dateTime.ToString("dd.MM.yyyy");
                label5.Text = reader[4].ToString() + " руб.";
                textBox1.Text = $"{reader[5]} {reader[6]} {reader[7]}";
                textBox2.Text = $"{reader[8]} {reader[9]} {reader[10]}";
            }
            connection.Close();

            MySqlConnection connection1 = new MySqlConnection(Connection.conn);
            connection1.Open();
            MySqlCommand command1 = new MySqlCommand($@"SELECT OrderID, orderproduct.ProductArticleNumber, 
            OrderCount, product.ProductName AS 'ProductName', product.ProductCost AS 'ProductCost' 
            FROM orderproduct
            INNER JOIN product ON product.ProductArticleNumber = orderproduct.ProductArticleNumber
            WHERE OrderID = {orderID}", connection1);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command1);
            DataTable table = new DataTable();
            adapter.Fill(table);

            table.Columns["ProductArticleNumber"].ColumnName = "Артикул";
            table.Columns["ProductName"].ColumnName = "Наименование";
            table.Columns["ProductCost"].ColumnName = "Цена";
            table.Columns["OrderCount"].ColumnName = "Количество";

            dataGridView1.DataSource = table;

            dataGridView1.Columns["OrderID"].Visible = false;

            dataGridView1.Rows[0].Cells[0].Selected = false;

            connection1.Clone();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ViewOrderProduct_Load(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                int r = e.RowIndex;
                dataGridView1.Rows[r].Selected = true;
            }
        }
    }
}
