using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Software_shell_for_testing_students
{
    public partial class Frm_Teacher : Form
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("15493264861407903145620973640218");
        private static readonly string database_path = System.IO.Path.Combine(Application.StartupPath, "Database_Main.mdf");
        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={database_path};Integrated Security=True;Connect Timeout=30";
        public Frm_Teacher()
        {
            InitializeComponent();
            Load_comboboxes();
            Load_treeview();
            Load_treeview_test();
        }
        private void Load_comboboxes()
        {
            List<string> comboBox1Items = new List<string>();
            HashSet<string> uniqueGroups = new HashSet<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Year, [Group] FROM [Groups]";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBox1Items.Add(reader[0].ToString());
                            string groupData = reader[1].ToString();
                            if (groupData.Contains("\\n"))
                            {
                                string[] groups = groupData.Split(new[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var g in groups)
                                {
                                    string trimmedGroup = g.Trim();
                                    if (!string.IsNullOrEmpty(trimmedGroup)) { uniqueGroups.Add(trimmedGroup); }
                                }
                            }
                            else
                            {
                                string[] groups = groupData.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var g in groups)
                                {
                                    string trimmedGroup = g.Trim();
                                    if (!string.IsNullOrEmpty(trimmedGroup)) { uniqueGroups.Add(trimmedGroup); }
                                }
                            }
                        }
                    }
                }
            }
            comboBox2.DataSource = null;
            comboBox2.DataSource = comboBox1Items;
            comboBox2.Text = "";
            comboBox3.DataSource = null;
            comboBox3.DataSource = uniqueGroups.ToList();
            comboBox3.Text = "";
            comboBox5.DataSource = null;
            comboBox5.DataSource = comboBox1Items;
            comboBox5.Text = "";
            comboBox4.DataSource = null;
            comboBox4.DataSource = uniqueGroups.ToList();
            comboBox4.Text = "";
        }
        private void Load_treeview()
        {
            string mainNodeName = "ГБПОУ КК \"АМТ\"";
            TreeNode mainNode = null;
            foreach (TreeNode node in treeView1.Nodes)
            {
                if (node.Text == mainNodeName)
                {
                    mainNode = node;
                    break;
                }
            }
            if (mainNode == null)
            {
                MessageBox.Show($"Главный узел '{mainNodeName}' не найден.");
                return;
            }
            mainNode.Nodes.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string queryYears = "SELECT DISTINCT Year FROM [Groups]";
                List<string> years = new List<string>();
                using (SqlCommand cmdYears = new SqlCommand(queryYears, connection))
                {
                    using (SqlDataReader readerYears = cmdYears.ExecuteReader())
                    {
                        while (readerYears.Read()) { years.Add(readerYears[0].ToString()); }
                    }
                }
                foreach (string year in years)
                {
                    TreeNode yearNode = new TreeNode(year);
                    string queryGroupsByYear = "SELECT [Group] FROM [Groups] WHERE Year = @Year";
                    using (SqlCommand cmdGroups = new SqlCommand(queryGroupsByYear, connection))
                    {
                        cmdGroups.Parameters.AddWithValue("@Year", year);
                        using (SqlDataReader readerGroups = cmdGroups.ExecuteReader())
                        {
                            while (readerGroups.Read())
                            {
                                string groupData = readerGroups["Group"].ToString();
                                groupData = groupData.Replace("\\n", "\n");
                                string[] groups;
                                if (groupData.Contains("\n")) {  groups = groupData.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries); }
                                else { groups = new[] { groupData }; }
                                foreach (var g in groups)
                                {
                                    string trimmedGroup = g.Trim();
                                    if (!string.IsNullOrEmpty(trimmedGroup))
                                    {
                                        TreeNode groupNode = new TreeNode(trimmedGroup);
                                        yearNode.Nodes.Add(groupNode);
                                    }
                                }
                            }
                        }
                    }
                    mainNode.Nodes.Add(yearNode);
                }
                treeView1.Refresh();
            }
        }
        private void Load_treeview_test()
        {
            string query_select = "SELECT * FROM Tests";
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query_select, connection);
                adapter.Fill(table);
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    TreeNode targetNode = null;
                    string yeargroup = Convert.ToString(table.Rows[i][1]);
                    string year = yeargroup.Length >= 2 ? yeargroup.Substring(0, 2) : yeargroup;
                    string group = yeargroup.Length > 2 ? yeargroup.Substring(2) : "";
                    foreach (TreeNode node in treeView1.Nodes)
                    {
                        targetNode = FindNodeByText(node, year);
                        if (targetNode != null)
                            break;
                    }
                    if (targetNode != null)
                    {
                        TreeNode groupNode = null;
                        foreach (TreeNode child in targetNode.Nodes)
                        {
                            if (child.Text == group)
                            {
                                groupNode = child;
                                break;
                            }
                        }
                        if (groupNode != null)
                        {
                            groupNode.Nodes.Add(new TreeNode(Convert.ToString(table.Rows[i][2])));
                            TreeNode test_tree = FindNodeByText(groupNode, Convert.ToString(table.Rows[i][2]));
                            test_tree.Tag = "Test";
                        }
                    }
                }
            }
        }
        TreeNode FindNodeByText(TreeNode currentNode, string text)
        {
            if (currentNode.Text == text)
                return currentNode;

            foreach (TreeNode child in currentNode.Nodes)
            {
                var result = FindNodeByText(child, text);
                if (result != null)
                    return result;
            }
            return null;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabPage1);
        }
        private void button8_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabPage2);
        }
        private void button9_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabPage3);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox7.Text != string.Empty && textBox8.Text != string.Empty)
            {
                string login = textBox8.Text;
                string pass = Hide_Pass(textBox7.Text);
                File.WriteAllText($"{login}.txt", pass);
                MessageBox.Show("Аккаунт успешно создан", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox4.Text = string.Empty;
                textBox3.Text = string.Empty;
            }
            else { MessageBox.Show("Вы не заполнили логин или пороль","Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
        public static string Hide_Pass(string pass)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.GenerateIV();
                byte[] iv = aes.IV;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, iv);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(iv, 0, iv.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) { swEncrypt.Write(pass); }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox6.Text != string.Empty)
            {
                string file_path = Path.Combine(Directory.GetCurrentDirectory(), comboBox3.Text);
                try
                {
                    if (File.Exists(file_path))
                    {
                        File.Delete(file_path);
                        MessageBox.Show("Аккаунт успешно удалён", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else { MessageBox.Show("Файл не найден: " + file_path, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
                catch (Exception ex) { MessageBox.Show("Ошибка при удалении аккаунта: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            else { MessageBox.Show("Вы не выбрали аккаунт для удаления", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
        private void button12_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Tag != null && treeView1.SelectedNode.Tag.ToString() == "Test")
                {
                    Frm_Test frm_Test = new Frm_Test(treeView1.SelectedNode.Text, "Open_test");
                    frm_Test.ShowDialog();
                }
                else { MessageBox.Show("Вы не выбрали тест", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            }
            else { MessageBox.Show("Вы не выбрали тест", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox2.Text != string.Empty && comboBox3.Text != string.Empty && textBox2.Text != string.Empty)
            {
                TreeNode targetNode = null;
                foreach (TreeNode node in treeView1.Nodes)
                {
                    targetNode = FindNodeByText(node, comboBox2.Text);
                    if (targetNode != null) { break; }
                }
                if (targetNode != null)
                {
                    TreeNode groupNode = null;
                    foreach (TreeNode child in targetNode.Nodes)
                    {
                        if (child.Text == comboBox3.Text)
                        {
                            groupNode = child;
                            break;
                        }
                    }
                    if (groupNode != null)
                    {
                        groupNode.Nodes.Add(new TreeNode(textBox2.Text));
                        string query = "INSERT INTO Tests ([Group], Name) VALUES (@Group, @Name)";
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@Group", comboBox2.Text + comboBox3.Text);
                                cmd.Parameters.AddWithValue("@Name", textBox2.Text);
                                try
                                {
                                    conn.Open();
                                    cmd.ExecuteNonQuery();
                                }
                                catch (Exception ex) { MessageBox.Show("Ошибка при сохранении данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                            }
                        }
                        Frm_Test frm_Test = new Frm_Test(textBox2.Text, "New_test");
                        frm_Test.ShowDialog();
                    }
                    else { MessageBox.Show("Такой группы нет в списке", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
                else { MessageBox.Show("Такого года нет в списке", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            else { MessageBox.Show("Вы не выбрали год, группу или не заполнили название теста", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty && textBox9.Text != string.Empty)
            {
                string mainNodeName = "ГБПОУ КК \"АМТ\"";
                TreeNode targetNode = null;
                foreach (TreeNode node in treeView1.Nodes)
                {
                    targetNode = FindNodeByText(node, textBox1.Text);
                    if (targetNode != null) { break; }
                }
                if (targetNode == null)
                {
                    TreeNode mainNode = null;
                    foreach (TreeNode node in treeView1.Nodes)
                    {
                        if (node.Text == mainNodeName)
                        {
                            mainNode = node;
                            mainNode.Nodes.Add(new TreeNode(textBox1.Text));
                            break;
                        }
                    }
                }
                foreach (TreeNode node in treeView1.Nodes)
                {
                    targetNode = FindNodeByText(node, textBox1.Text);
                    if (targetNode != null) { break; }
                }
                if (targetNode != null) { targetNode.Nodes.Add(new TreeNode(textBox9.Text)); }
                else { MessageBox.Show("Такая группа уже есть в списке", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string checkYearQuery = "SELECT [Group] FROM [Groups] WHERE Year = @Year";
                    string existingGroups = null;
                    using (SqlCommand cmdCheck = new SqlCommand(checkYearQuery, connection))
                    {
                        cmdCheck.Parameters.AddWithValue("@Year", textBox1.Text);
                        object result = cmdCheck.ExecuteScalar();
                        if (result != null && result != DBNull.Value) { existingGroups = result.ToString(); }
                    }
                    if (existingGroups != null)
                    {
                        string newGroupsStr = existingGroups + "\\n" + textBox9.Text;
                        string updateQuery = "UPDATE [Groups] SET [Group] = @Groups WHERE Year = @Year";
                        using (SqlCommand cmdUpdate = new SqlCommand(updateQuery, connection))
                        {
                            cmdUpdate.Parameters.AddWithValue("@Groups", newGroupsStr);
                            cmdUpdate.Parameters.AddWithValue("@Year", textBox1.Text);
                            cmdUpdate.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO [Groups] (Year, [Group]) VALUES (@Year, @Group)";
                        using (SqlCommand cmdInsert = new SqlCommand(insertQuery, connection))
                        {
                            cmdInsert.Parameters.AddWithValue("@Year", textBox1.Text);
                            cmdInsert.Parameters.AddWithValue("@Group", textBox9.Text);
                            cmdInsert.ExecuteNonQuery();
                        }
                    }
                }
                Load_comboboxes();
            }
            else { MessageBox.Show("Вы не заполнили поля года или группы", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
        private void Frm_Teacher_Load(object sender, EventArgs e)
        {
            this.resultsTableAdapter.Fill(this.database_MainDataSet.Results);
        }
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            try
            {
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.txt");
                foreach (string file in files) { comboBox3.Items.Add(Path.GetFileName(file)); }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка при загрузке файлов: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                string tableName = e.Node.Text;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = $"SELECT * FROM [{tableName}]";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch { }
        }
        private void button10_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Tag != null && treeView1.SelectedNode.Tag.ToString() == "Test")
                {
                    Frm_Test frm_Test = new Frm_Test(treeView1.SelectedNode.Text, "Print_test");
                    frm_Test.ShowDialog();
                }
                else { MessageBox.Show("Вы не выбрали тест", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            }
            else { MessageBox.Show("Вы не выбрали тест", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM Results WHERE 1=1";
            var conditions = new List<string>();
            var parameters = new List<SqlParameter>();
            if (!string.IsNullOrWhiteSpace(comboBox5.Text))
            {
                conditions.Add("Year LIKE @ComboBox5");
                parameters.Add(new SqlParameter("@ComboBox5", "%" + comboBox5.Text + "%"));
            }
            if (!string.IsNullOrWhiteSpace(comboBox4.Text))
            {
                conditions.Add("[Group] LIKE @ComboBox4");
                parameters.Add(new SqlParameter("@ComboBox4", "%" + comboBox4.Text + "%"));
            }
            if (!string.IsNullOrWhiteSpace(textBox3.Text))
            {
                conditions.Add("Name_test LIKE @TextBox3");
                parameters.Add(new SqlParameter("@TextBox3", "%" + textBox3.Text + "%"));
            }
            if (!string.IsNullOrWhiteSpace(textBox4.Text))
            {
                conditions.Add("Name_student LIKE @TextBox4");
                parameters.Add(new SqlParameter("@TextBox4", "%" + textBox4.Text + "%"));
            }
            if (!string.IsNullOrWhiteSpace(textBox6.Text))
            {
                conditions.Add("Accuracy LIKE @TextBox6");
                parameters.Add(new SqlParameter("@TextBox6", "%" + textBox6.Text + "%"));
            }
            if (checkBox1.Checked)
            {
                string selectedDate = dateTimePicker1.Value.Date.ToString("dd/MM/yyyy");
                conditions.Add("CONVERT(date, Date) = @SelectedDate");
                parameters.Add(new SqlParameter("@SelectedDate", selectedDate));
            }
            if (conditions.Count > 0) { query += " AND " + string.Join(" AND ", conditions); }
            if (conditions.Count != 0)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                        try
                        {
                            conn.Open();
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dataGridView2.DataSource = dt;
                            var columnHeaders = new Dictionary<string, string>
                            {
                                { "Year", "Год" },
                                { "Group", "Группа" },
                                { "Name_test", "Название теста" },
                                { "Name_student", "ФИО студента" },
                                { "Accuracy", "Правильность" },
                                { "Date", "Дата" },
                            };
                            foreach (var kvp in columnHeaders)
                            {
                                if (dataGridView2.Columns.Contains(kvp.Key)) { dataGridView2.Columns[kvp.Key].HeaderText = kvp.Value; }
                            }
                        }
                        catch (Exception ex) { MessageBox.Show("Ошибка при выполнении поиска: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                }
            }
            else { MessageBox.Show("Вы не выбрали значения для поиска", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog()
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Сохранить как Excel"
                })
                {
                    if (sfd.ShowDialog() != DialogResult.OK) { return; }
                    using (var workbook = new ClosedXML.Excel.XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Результаты");
                        for (int i = 0; i < dataGridView2.Columns.Count; i++) { worksheet.Cell(1, i + 1).Value = dataGridView2.Columns[i].HeaderText; }
                        for (int i = 0; i < dataGridView2.Rows.Count; i++)
                        {
                            if (!dataGridView2.Rows[i].IsNewRow)
                            {
                                for (int j = 0; j < dataGridView2.Columns.Count; j++)
                                {
                                    var value = dataGridView2.Rows[i].Cells[j].Value;
                                    worksheet.Cell(i + 2, j + 1).Value = value != null ? value.ToString() : "";
                                }
                            }
                        }
                        workbook.SaveAs(sfd.FileName);
                    }
                    MessageBox.Show($"Данные успешно сохранены в файл {sfd.FileName}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка при сохранении файла: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
