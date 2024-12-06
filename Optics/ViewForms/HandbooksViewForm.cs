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
    public partial class HandbooksViewForm : Form
    {
        public HandbooksViewForm()
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

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Validation.IsRussianLetter(e.KeyChar) || char.IsWhiteSpace(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Validation.IsRussianLetter(e.KeyChar) || char.IsWhiteSpace(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void HandbooksViewForm_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;

            dataGridView1.ClearSelection();
            dataGridView2.ClearSelection();

            FillDataProductCategoryGridView();
            FillDataRoleGridView();
        }

        private void FillDataProductCategoryGridView()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.ClearSelection();

            using (MySqlConnection connection = new MySqlConnection(Connection.conn))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand($@"SELECT * FROM productcategory", connection))
                {
                    command.ExecuteNonQuery();

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        table.Columns[1].ColumnName = "Наименование";
                        dataGridView1.DataSource = table;

                        dataGridView1.Columns[0].Visible = false;

                        dataGridView1.Rows[0].Cells[0].Selected = false;

                        DataGridViewButtonColumn buttonColumn2 = new DataGridViewButtonColumn();
                        dataGridView1.Columns.Add(buttonColumn2);
                        buttonColumn2.UseColumnTextForButtonValue = true;
                        buttonColumn2.Text = "Удалить";

                        foreach (DataGridViewColumn column in dataGridView1.Columns)
                        {
                            column.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                    }
                }
            }
        }

        private void FillDataRoleGridView()
        {
            dataGridView2.Columns.Clear();

            using (MySqlConnection connection = new MySqlConnection(Connection.conn))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand($@"SELECT * FROM role", connection))
                {
                    command.ExecuteNonQuery();

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        table.Columns[1].ColumnName = "Наименование";
                        dataGridView2.DataSource = table;

                        dataGridView2.Columns[0].Visible = false;

                        dataGridView2.Rows[0].Cells[0].Selected = false;

                        DataGridViewButtonColumn buttonColumn2 = new DataGridViewButtonColumn();
                        dataGridView2.Columns.Add(buttonColumn2);
                        buttonColumn2.UseColumnTextForButtonValue = true;
                        buttonColumn2.Text = "Удалить";

                        foreach (DataGridViewColumn column in dataGridView2.Columns)
                        {
                            column.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                    }
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView2.ClearSelection();
            dataGridView1.ClearSelection();
            FillDataRoleGridView();
            FillDataProductCategoryGridView();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string productcategory = textBox2.Text;
                List<string> name = new List<string>();
                try
                {
                    MySqlConnection connection = new MySqlConnection(Connection.conn);
                    connection.Open();
                    MySqlCommand command = new MySqlCommand($@"SELECT lower(ProductCategoryName) FROM productcategory;", connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        name.Add(reader.GetString(0));
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (!name.Contains(productcategory.ToLower()))
                {
                    Data.InsertUpdateDeleteData($@"INSERT INTO productcategory (ProductCategoryName) 
                    VALUES ('{productcategory}')");
                    MessageBox.Show("Категория товара успешно добавлена!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    textBox2.Clear();
                    button2.Enabled = false;
                    FillDataProductCategoryGridView();
                }
                else
                {
                    MessageBox.Show("Категория товара с таким наименованием уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            button2.Enabled = true;
        }

        public int categoryId; 
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex != -1)
                {
                    int r = e.RowIndex, c = e.ColumnIndex;
                    categoryId = Convert.ToInt32(dataGridView1.Rows[r].Cells[0].Value);

                    switch (c)
                    {
                        case 1:
                            MySqlConnection connection = new MySqlConnection(Connection.conn);
                            connection.Open();
                            MySqlCommand command = new MySqlCommand($"SELECT * FROM productcategory WHERE ProductCategoryID = '{categoryId}'", connection);
                            MySqlDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                textBox2.Text = reader[1].ToString();
                            }
                            connection.Close();
                            button3.Enabled = true;
                            break;
                        case 2:
                            DialogResult result = MessageBox.Show("Вы действительно хотите удалить запись?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                try
                                {
                                    bool res = Data.InsertUpdateDeleteData($"DELETE FROM productcategory WHERE ProductCategoryID = '{categoryId}'");

                                    if (res == true)
                                    {
                                        Data.InsertUpdateDeleteData($"DELETE FROM productcategory WHERE ProductCategoryID = '{categoryId}'");
                                        MessageBox.Show("Категория товара успешно удалена!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        FillDataProductCategoryGridView();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Категория товара не может быть удалена, так как она используется в других таблицах.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string productcategory = textBox2.Text;
                List<string> name = new List<string>();
                try
                {
                    MySqlConnection connection = new MySqlConnection(Connection.conn);
                    connection.Open();
                    MySqlCommand command = new MySqlCommand($@"SELECT lower(ProductCategoryName) 
                    FROM productcategory WHERE ProductCategoryName = '{productcategory}' AND ProductCategoryID <> '{categoryId}';", connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        name.Add(reader.GetString(0));
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (!name.Contains(productcategory.ToLower()))
                {
                    Data.InsertUpdateDeleteData($@"UPDATE productcategory SET ProductCategoryName = '{productcategory}'
                    WHERE ProductCategoryID = '{categoryId}'");
                    MessageBox.Show("Категория товара успешно отредактирована!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    textBox2.Clear();
                    FillDataProductCategoryGridView();
                    button2.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Категория товара с таким наименованием уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button5.Enabled = true;
        }

        public int roleId;
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex != -1)
                {
                    int r = e.RowIndex, c = e.ColumnIndex;
                    roleId = Convert.ToInt32(dataGridView2.Rows[r].Cells[0].Value);

                    switch (c)
                    {
                        case 1:
                            MySqlConnection connection = new MySqlConnection(Connection.conn);
                            connection.Open();
                            MySqlCommand command = new MySqlCommand($"SELECT * FROM role WHERE RoleID = '{roleId}'", connection);
                            MySqlDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                textBox1.Text = reader[1].ToString();
                            }
                            connection.Close();
                            button4.Enabled = true;
                            break;
                        case 2:
                            DialogResult result = MessageBox.Show("Вы действительно хотите удалить запись?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                try
                                {
                                    bool res = Data.InsertUpdateDeleteData($"DELETE FROM role WHERE RoleID = '{roleId}'");

                                    if (res == true)
                                    {
                                        Data.InsertUpdateDeleteData($"DELETE FROM role WHERE RoleID = '{roleId}'");
                                        MessageBox.Show("Роль успешно удалена!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        FillDataRoleGridView();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Роль не может быть удалена, так как она используется в других таблицах.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                int r = e.RowIndex;
                dataGridView2.Rows[r].Selected = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string role = textBox1.Text;
                List<string> name = new List<string>();
                try
                {
                    MySqlConnection connection = new MySqlConnection(Connection.conn);
                    connection.Open();
                    MySqlCommand command = new MySqlCommand($@"SELECT lower(RoleName) FROM role;", connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        name.Add(reader.GetString(0));
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (!name.Contains(role.ToLower()))
                {
                    Data.InsertUpdateDeleteData($@"INSERT INTO role (RoleName) 
                    VALUES ('{role}')");
                    MessageBox.Show("Роль успешно добавлена!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    textBox1.Clear();
                    button5.Enabled = false;
                    FillDataRoleGridView();
                }
                else
                {
                    MessageBox.Show("Роль с таким наименованием уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string role = textBox1.Text;
                List<string> name = new List<string>();
                try
                {
                    MySqlConnection connection = new MySqlConnection(Connection.conn);
                    connection.Open();
                    MySqlCommand command = new MySqlCommand($@"SELECT lower(RoleName) 
                    FROM role WHERE RoleName = '{role}' AND RoleID <> '{roleId}';", connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        name.Add(reader.GetString(0));
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (!name.Contains(role.ToLower()))
                {
                    Data.InsertUpdateDeleteData($@"UPDATE role SET RoleName = '{role}'
                    WHERE RoleID = '{roleId}'");
                    MessageBox.Show("Роль успешно отредактирована!", "Сообщение пользователю", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    textBox1.Clear();
                    FillDataRoleGridView();
                    button4.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Роль с таким наименованием уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
