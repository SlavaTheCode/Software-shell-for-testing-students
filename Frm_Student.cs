using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;

namespace Software_shell_for_testing_students
{
    public partial class Frm_Student : Form
    {
        private static readonly string database_path = System.IO.Path.Combine(Application.StartupPath, "Database_Main.mdf");
        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={database_path};Integrated Security=True;Connect Timeout=30";
        public Frm_Student(string student_name, string student_year, string student_group)
        {
            InitializeComponent();
            label6.Text = "Студент - " + student_name;
            label7.Text = "Группа - " + student_year + student_group;
            Load_treeview(student_year, student_group);
            Load_treeview_test();
        }
        private void Load_treeview(string st_year, string st_group)
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
                                if (groupData.Contains("\n")) { groups = groupData.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries); }
                                else { groups = new[] { groupData }; }
                                foreach (var g in groups)
                                {
                                    string trimmedGroup = g.Trim();
                                    if (!string.IsNullOrEmpty(trimmedGroup))
                                    {
                                        TreeNode groupNode = new TreeNode(trimmedGroup);
                                        if (st_group == groupNode.Text) { yearNode.Nodes.Add(groupNode); }
                                    }
                                }
                            }
                        }
                    }
                    if (st_year == yearNode.Text) { mainNode.Nodes.Add(yearNode); }
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
        Thread thread;
        private void button1_Click(object sender, EventArgs e)
        {
            if (label4.Text != "Выбранный тест - не выбран")
            {
                string yeargroup = label7.Text.Replace("Группа - ", "");
                string st_name = label6.Text.Replace("Студент - ", "");
                string st_test = label4.Text.Replace("Выбранный тест - ", "");
                this.Close();
                thread = new Thread(() => open_Test(yeargroup, st_name, st_test));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            else { MessageBox.Show("Вы не выбрали тест для прохождения", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
        private void open_Test(string yeargroup, string st_name, string st_test)
        {
            string info = yeargroup + "|" + st_name + "|" + st_test;
            Application.Run(new Frm_Test(info, "Pass_test"));
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string yeargroup = label7.Text.Replace("Группа - ", "").Trim();
            string year = yeargroup.Length >= 2 ? yeargroup.Substring(0, 2) : yeargroup;
            string group = yeargroup.Length > 2 ? yeargroup.Substring(2) : "";
            if (treeView1.SelectedNode.Text != year && treeView1.SelectedNode.Text != group && treeView1.SelectedNode.Text != "ГБПОУ КК \"АМТ\"") { label4.Text = "Выбранный тест - " + treeView1.SelectedNode.Text; }
        }
    }
}
