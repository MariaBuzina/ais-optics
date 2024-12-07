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
    public partial class IdleTimeForm : Form
    {
        public IdleTimeForm()
        {
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Validation.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        /// <summary>
        /// Сохранение времени бездействия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["timeout"] = textBox1.Text;
            MessageBox.Show("Время успешнео изменено!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);
            textBox1.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AuthorizationForm authorizationForm = new AuthorizationForm();
            this.Visible = false;
            authorizationForm.ShowDialog();
            this.Close();
        }
    }
}
