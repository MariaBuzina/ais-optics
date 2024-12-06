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
    public partial class OrdersViewForm : Form
    {
        public OrdersViewForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ManagerForm managerForm = new ManagerForm();
            this.Visible = false;
            managerForm.ShowDialog();
            this.Close();
        }

        private void OrderViewForm_Load(object sender, EventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dataGridView1.ClearSelection();

            MySqlConnection connection = new MySqlConnection(Connection.conn);
            connection.Open();
            MySqlCommand command = new MySqlCommand(@"SELECT OrderID AS 'Номер заказа', 
            OrderDate AS 'Дата заказа', ProductUser, OrderClient, OrderAmount AS 'Сумма заказа', 
            user.UserSurname AS 'Менеджер', client.ClientSurname AS 'Клиент' from `order`
            INNER JOIN user ON user.UserID = `order`.ProductUser
            INNER JOIN client ON client.ClientID = `order`.OrderClient", connection);

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            adapter.Fill(table);

            dataGridView1.DataSource = table;
            dataGridView1.Rows[0].Cells[0].Selected = false;

            dataGridView1.Columns["ProductUser"].Visible = false;
            dataGridView1.Columns["OrderClient"].Visible = false;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            connection.Close();
        }
        int currentRowIndex;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                int r = e.RowIndex;
                dataGridView1.Rows[r].Selected = true;
            }
        }
        void contextmenu_click(object sender, EventArgs e)
        {
            int orderId = Convert.ToInt32(dataGridView1.Rows[currentRowIndex].Cells["Номер заказа"].Value);
            dataGridView1.Rows[currentRowIndex].Selected = false;

            ViewOrderProduct viewOrderProduct = new ViewOrderProduct(orderId);
            viewOrderProduct.ShowDialog();
        }

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
    }
}
