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
    public partial class AdminForm : Form
    {
        public AdminForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            DialogResult dialogResult = MessageBox.Show("Вы действительно хотите выйти?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AuthorizationForm authorizationForm = new AuthorizationForm();
            this.Visible = false;
            authorizationForm.ShowDialog();
            this.Close();
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            label2.Text = $"Добро пожаловать, {Data.surname} {Data.name} {Data.patronymic}!";
            label3.Text = DateTime.Now.ToString("dd.MM.yyyy");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UsersViewForm usersViewForm = new UsersViewForm();
            this.Visible = false;
            usersViewForm.ShowDialog();
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SuppliersViewForm supplierViewForm = new SuppliersViewForm();
            this.Visible = false;
            supplierViewForm.ShowDialog();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            HandbooksViewForm handbooksViewForm = new HandbooksViewForm();
            this.Visible = false;
            handbooksViewForm.ShowDialog();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ProductsViewForm productsViewForm = new ProductsViewForm();
            this.Visible = false;
            productsViewForm.ShowDialog();
            this.Close();
        }
    }
}
