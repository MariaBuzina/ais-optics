﻿using System;
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
    public partial class ClientsViewForm : Form
    {
        public ClientsViewForm()
        {
            InitializeComponent();
        }

        private void ClientsViewForm_Load(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            FillDataGridView();
        }
        public string conn = Connection.conn;
        private void FillDataGridView()
        {
            dataGridView1.Columns.Clear();

            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand($@"SELECT ClientId, ClientSurname, ClientName, ClientPatronymic, ClientPhone, ClientBirthday FROM client", connection))
                {
                    command.ExecuteNonQuery();

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        table.Columns[1].ColumnName = "Фамилия";
                        table.Columns[2].ColumnName = "Имя";
                        table.Columns[3].ColumnName = "Отчество";
                        table.Columns[4].ColumnName = "Телефон";
                        table.Columns[5].ColumnName = "Дата рождения";
                        dataGridView1.DataSource = table;

                        dataGridView1.Columns[0].Visible = false;

                        dataGridView1.Rows[0].Cells[0].Selected = false;

                        DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                        imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                        dataGridView1.Columns.Add(imageColumn);
                        dataGridView1.AllowUserToAddRows = false;

                        DataGridViewImageColumn imageColumn1 = new DataGridViewImageColumn();
                        imageColumn1.ImageLayout = DataGridViewImageCellLayout.Zoom;
                        dataGridView1.Columns.Add(imageColumn1);
                        dataGridView1.AllowUserToAddRows = false;

                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            row.Cells[6].Value = Image.FromFile(@"./photo/edit.png");
                            row.Cells[7].Value = Image.FromFile(@"./photo/delete.png");
                        }

                        dataGridView1.Columns["Телефон"].Width = 190;
                        foreach (DataGridViewColumn column in dataGridView1.Columns)
                        {
                            column.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                    }
                }
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

        private void button3_Click(object sender, EventArgs e)
        {
            AddClientForm addClientForm = new AddClientForm();
            this.Visible = false;
            addClientForm.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ManagerForm managerForm = new ManagerForm();
            this.Visible = false;
            managerForm.ShowDialog();
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
                        case 6:
                            EditClientForm editClientForm = new EditClientForm(id);
                            this.Visible = false;
                            editClientForm.ShowDialog();
                            this.Close();
                            break;
                        case 7:
                            DialogResult result = MessageBox.Show("Вы действительно хотите удалить запись?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                try
                                {
                                    bool res = Data.InsertUpdateDeleteData($"DELETE FROM supplier WHERE SupplierID = '{id}'");
                                    if (res == true)
                                    {
                                        Data.InsertUpdateDeleteData($"DELETE FROM client WHERE ClientID = '{id}'");
                                        MessageBox.Show("Клиент успешно удален!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        FillDataGridView();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Клиент не может быть удален, так как он используется в других таблицах.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null)
            {
                string val = e.Value.ToString();

                switch (dataGridView1.Columns[e.ColumnIndex].Name)
                {
                    case "Фамилия":
                        e.Value = val[0];
                        break;

                    case "Телефон":
                        e.Value = val.Substring(0, 13) + "**" + "-" + "**";
                        break;
                }
            }
        }
    }
}
