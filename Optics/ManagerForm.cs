using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Optics
{
    public partial class ManagerForm : Form
    {
        public ManagerForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AuthorizationForm authorizationForm = new AuthorizationForm();
            this.Visible = false;
            authorizationForm.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы действительно хотите выйти?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        private void ManagerForm_Load(object sender, EventArgs e)
        {
            label2.Text = $"Добро пожаловать, {Data.surname} {Data.name} {Data.patronymic}!";
            label3.Text = DateTime.Now.ToString("dd.MM.yyyy");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClientsViewForm clientsViewForm = new ClientsViewForm();
            this.Visible = false;
            clientsViewForm.ShowDialog();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AddOrderProductForm addOrderProductForm = new AddOrderProductForm();
            this.Visible = false;
            addOrderProductForm.ShowDialog();
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SuppliersViewForm supplierViewForm = new SuppliersViewForm();
            this.Visible = false;
            supplierViewForm.ShowDialog();
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OrdersViewForm orderViewForm = new OrdersViewForm();
            this.Visible = false;
            orderViewForm.ShowDialog();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ReportForm reportForm = new ReportForm();
            this.Visible = false;
            reportForm.ShowDialog();
            this.Close();
        }
    }
}
