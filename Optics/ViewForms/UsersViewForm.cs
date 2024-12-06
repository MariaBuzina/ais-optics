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
    public partial class UsersViewForm : Form
    {
        public UsersViewForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AdminForm adminForm = new AdminForm();
            this.Visible = false;
            adminForm.ShowDialog();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddUserForm addUserForm = new AddUserForm();
            this.Visible = false;
            addUserForm.ShowDialog();
            this.Close();
        }

        private void UsersViewForm_Load(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            FillDataGridView();
        }

        public void FillDataGridView()
        {
            dataGridView1.Columns.Clear();

            MySqlConnection connection = new MySqlConnection(Connection.conn);
            connection.Open();
            MySqlCommand command = new MySqlCommand(@"SELECT UserID, UserSurname, UserName, UserPatronymic, UserLogin,
            UserPassword, UserRole, role.RoleName AS 'RoleName' FROM user
            INNER JOIN role ON user.UserRole = role.RoleID", connection);
            command.ExecuteNonQuery();

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            adapter.Fill(table);

            table.Columns[1].ColumnName = "Фамилия";
            table.Columns[2].ColumnName = "Имя";
            table.Columns[3].ColumnName = "Отчество";
            table.Columns[4].ColumnName = "Логин";
            table.Columns["RoleName"].ColumnName = "Роль";
            dataGridView1.DataSource = table;

            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[6].Visible = false;

            dataGridView1.Rows[0].Cells[0].Selected = false;

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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                int r = e.RowIndex;
                dataGridView1.Rows[r].Selected = true;
            }
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
                        case 8:
                            EditUserForm editUserForm = new EditUserForm(id);
                            this.Visible = false;
                            editUserForm.ShowDialog();
                            this.Close();
                            break;
                        case 9:
                            DialogResult result = MessageBox.Show("Вы действительно хотите удалить запись?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                try
                                {
                                    bool res = Data.InsertUpdateDeleteData($"DELETE FROM user WHERE UserID = '{id}'");

                                    if (res == true)
                                    {
                                        Data.InsertUpdateDeleteData($"DELETE FROM user WHERE UserID = '{id}'");
                                        MessageBox.Show("Пользователь успешно удален!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        FillDataGridView();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Пользователь не может быть удален, так как он используется в других таблицах.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
