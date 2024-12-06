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
    public partial class SuppliersViewForm : Form
    {
        public SuppliersViewForm()
        {
            InitializeComponent();
        }

        private void SupplierViewForm_Load(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            FillDataGridView();
        }
        private void FillDataGridView()
        {
            dataGridView1.Columns.Clear();

            using (MySqlConnection connection = new MySqlConnection(Connection.conn))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand($@"SELECT * FROM supplier", connection))
                {
                    command.ExecuteNonQuery();

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        table.Columns[1].ColumnName = "Наименование";
                        table.Columns[2].ColumnName = "Контактное лицо";
                        table.Columns[3].ColumnName = "Номер телефона";
                        table.Columns[4].ColumnName = "Адрес";
                        dataGridView1.DataSource = table;

                        dataGridView1.Columns[0].Visible = false;

                        dataGridView1.Rows[0].Cells[0].Selected = false;

                        if (Data.role == "1")
                        {
                            DataGridViewButtonColumn buttonColumn1 = new DataGridViewButtonColumn();
                            dataGridView1.Columns.Add(buttonColumn1);
                            buttonColumn1.UseColumnTextForButtonValue = true;
                            buttonColumn1.Text = "Изменить";

                            DataGridViewButtonColumn buttonColumn2 = new DataGridViewButtonColumn();
                            dataGridView1.Columns.Add(buttonColumn2);
                            buttonColumn2.UseColumnTextForButtonValue = true;
                            buttonColumn2.Text = "Удалить";

                            button3.Visible = true;
                        }
                        foreach (DataGridViewColumn column in dataGridView1.Columns)
                        {
                            column.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Data.role == "1")
            {
                AdminForm adminForm = new AdminForm();
                this.Visible = false;
                adminForm.ShowDialog();
                this.Close();
            }
            else if (Data.role == "2")
            {
                ManagerForm managerForm = new ManagerForm();
                this.Visible = false;
                managerForm.ShowDialog();
                this.Close();
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddSupplierForm addSupplierForm = new AddSupplierForm();
            this.Visible = false;
            addSupplierForm.ShowDialog();
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex != -1)
                {
                    int r = e.RowIndex, c = e.ColumnIndex;
                    int id = Convert.ToInt32(dataGridView1.Rows[r].Cells[0].Value);

                    switch (c)
                    {
                        case 5:
                            EditSupplierForm editSupplierForm = new EditSupplierForm(id);
                            this.Visible = false;
                            editSupplierForm.ShowDialog();
                            this.Close();
                            break;
                        case 6:
                            DialogResult result = MessageBox.Show("Вы действительно хотите удалить запись?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                try
                                {
                                    bool res = Data.InsertUpdateDeleteData($"DELETE FROM supplier WHERE SupplierID = '{id}'");
                                    if (res == true)
                                    {
                                        Data.InsertUpdateDeleteData($"DELETE FROM supplier WHERE SupplierID = '{id}'");
                                        MessageBox.Show("Поставщик успешно удален!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        FillDataGridView();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Поставщик не может быть удален, так как он используется в других таблицах.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
    }
}
