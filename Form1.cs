using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Software_shell_for_testing_students
{
    public partial class Frm_Authorization : Form
    {
        private static readonly string database_path = System.IO.Path.Combine(Application.StartupPath, "Database_Main.mdf");
        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={database_path};Integrated Security=True;Connect Timeout=30";
        public Frm_Authorization()
        {
            InitializeComponent();
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
            comboBox1.DataSource = comboBox1Items;
            comboBox2.DataSource = null;
            comboBox2.DataSource = uniqueGroups.ToList();
        }
        Thread thread;
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("15493264861407903145620973640218");
        private void button3_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel1.Visible = true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible=false;
            panel4.Visible=true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible=false;
            panel3.Visible=true;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != string.Empty && textBox2.Text != string.Empty)
            {
                if (textBox1.Text == "admin" && textBox2.Text == "admin")
                {
                    this.Close();
                    thread = new Thread(open_Teacher);
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }
                else
                {
                    string login = textBox1.Text;
                    string pass = "";
                    try
                    {
                        pass = Show_Pass(File.ReadAllText($"{login}.txt"));
                        if (textBox2.Text == pass)
                        {
                            this.Close();
                            thread = new Thread(open_Teacher);
                            thread.SetApartmentState(ApartmentState.STA);
                            thread.Start();
                        }
                        else { MessageBox.Show("Неправильный пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                    catch { MessageBox.Show("Неправильный логин", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
            else { MessageBox.Show("Вы не заполнили логин или пароль", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
        public void open_Teacher() { Application.Run(new Frm_Teacher()); }
        public static string Show_Pass(string pass)
        {
            byte[] fullCipher = Convert.FromBase64String(pass);
            byte[] iv = new byte[16];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream msDecrypt = new MemoryStream(fullCipher))
                {
                    msDecrypt.Seek(iv.Length, SeekOrigin.Begin);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox4.Text != string.Empty)
            {
                string name = textBox4.Text;
                if (Regex.IsMatch(name, @"^[а-яА-ЯёЁ\s]+$"))
                {
                    if (comboBox1.Text != string.Empty && comboBox2.Text != string.Empty)
                    {
                        string year = comboBox1.Text;
                        string group = comboBox2.Text;
                        this.Close();
                        thread = new Thread (()=> open_Student(name, year, group));
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                    }
                    else { MessageBox.Show("Вы не выбрали год или группу", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                }
                else { MessageBox.Show("Имя не может содержать любые иные символы, кроме киррилицы", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            else { MessageBox.Show("Вы не заполнили имя", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
        public void open_Student(string name, string year, string group) 
        { 
            Application.Run(new Frm_Student(name, year, group)); 
        }
    }
}
