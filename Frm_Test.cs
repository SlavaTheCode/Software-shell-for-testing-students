using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Software_shell_for_testing_students
{
    public partial class Frm_Test : Form
    {
        private static readonly string database_path = System.IO.Path.Combine(Application.StartupPath, "Database_Main.mdf");
        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={database_path};Integrated Security=True;Connect Timeout=30";
        public Frm_Test(string name_test, string type_test)
        {
            InitializeComponent();
            switch (type_test)
            {
                case "New_test":
                    {
                        label1.Text = name_test;
                        label1.Left = (this.ClientSize.Width - label1.Width) / 2;
                        New_question();
                        break;
                    }
                case "Open_test":
                    {
                        label1.Text = name_test;
                        label1.Left = (this.ClientSize.Width - label1.Width) / 2;
                        Open_test();
                        button1.Text = "Распечатать";
                        break;
                    }
                case "Pass_test":
                    {
                        button1.Text = "Отправить";
                        string[] splited_info = name_test.Split('|');
                        string year = splited_info[0].Length >= 2 ? splited_info[0].Substring(0, 2) : splited_info[0];
                        string group = splited_info[0].Length > 2 ? splited_info[0].Substring(2) : "";
                        string st_name = splited_info[1];
                        string st_test = splited_info[2];
                        label1.Text = st_test;
                        label2.Visible = true;
                        label2.Text = "Студент - " + st_name;
                        label2.Location = new System.Drawing.Point((this.Width - label2.Size.Width - 28), label2.Location.Y);
                        label3.Visible = true;
                        label3.Text = "Группа - " + year +group;
                        Pass_test();
                        break;
                    }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("При подтверждении выхода тест не сохранится, поэтому убедитесь, что вы его уже сохранили, если это необходимо", "Вы уверены, что хотите выйти?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes) { Close(); }
            if (result == DialogResult.No) { }
        }
        private void New_question()
        {
            Panel pnl_newquestion = new Panel();
            pnl_newquestion.BorderStyle = BorderStyle.None;
            pnl_newquestion.Size = new Size(527, 120);
            pnl_newquestion.Location = new System.Drawing.Point(14, 13);

            System.Windows.Forms.Label lbl_question = new System.Windows.Forms.Label();
            lbl_question.Text = "Добавить вопрос";
            lbl_question.Location = new System.Drawing.Point(3, 4);
            lbl_question.AutoSize = true;

            Button btn1_question = new Button();
            btn1_question.Text = "С одним ответом";
            btn1_question.Size = new Size(137, 23);
            btn1_question.Location = new System.Drawing.Point(104, 20);
            btn1_question.Visible = false;

            Button btn2_question = new Button();
            btn2_question.Text = "С вариантами ответов";
            btn2_question.Size = new Size(137, 23);
            btn2_question.Location = new System.Drawing.Point(104, 72);
            btn2_question.Visible = false;
            btn2_question.Click += (s, args) =>
            {
                Question_complex(pnl_newquestion);
                btn1_question.Visible = false;
                btn2_question.Visible = false;
            };

            btn1_question.Click += (s, args) =>
            {
                Question_simple(pnl_newquestion);
                btn1_question.Visible = false;
                btn2_question.Visible = false;
            };

            PictureBox pctrBx_question = new PictureBox();
            pctrBx_question.Size = new Size(75, 75);
            pctrBx_question.Location = new System.Drawing.Point(14, 20);
            pctrBx_question.SizeMode = PictureBoxSizeMode.StretchImage;
            pctrBx_question.Image = Software_shell_for_testing_students.Properties.Resources._149145;
            pctrBx_question.Click += (s, args) =>
            {
                if (btn1_question.Visible == false) { btn1_question.Visible = true; btn2_question.Visible = true; }
                else { btn1_question.Visible = false; btn2_question.Visible = false; }
            };

            pnl_newquestion.Controls.Add(lbl_question);
            pnl_newquestion.Controls.Add(pctrBx_question);
            pnl_newquestion.Controls.Add(btn1_question);
            pnl_newquestion.Controls.Add(btn2_question);

            panel_Main.Controls.Add(pnl_newquestion);
        }
        private void Question_simple(Panel pnl_newquestion)
        {
            Panel pnl_question = new Panel();
            pnl_question.BorderStyle = BorderStyle.None;
            pnl_question.Size = new Size(520, 160);
            pnl_question.Location = new System.Drawing.Point(14, 13 + this.Controls.OfType<System.Windows.Forms.Control>().Count(c => c is Panel) * 160);

            System.Windows.Forms.Label lbl1_question = new System.Windows.Forms.Label();
            lbl1_question.Text = "Напишите вопрос:";
            lbl1_question.Location = new System.Drawing.Point(3, 4);
            lbl1_question.AutoSize = true;

            TextBox txtBx1_question = new TextBox();
            txtBx1_question.Size = new Size(510, 80);
            txtBx1_question.Location = new System.Drawing.Point(3, 20);
            txtBx1_question.Multiline = true;
            txtBx1_question.AutoSize = true;

            System.Windows.Forms.Label lbl2_question = new System.Windows.Forms.Label();
            lbl2_question.Text = "Напишите ответ:";
            lbl2_question.Location = new System.Drawing.Point(3, 100);
            lbl2_question.AutoSize = true;

            TextBox txtBx2_question = new TextBox();
            txtBx2_question.Size = new Size(400, 45);
            txtBx2_question.Location = new System.Drawing.Point(3, 115);
            txtBx2_question.Multiline = true;
            txtBx2_question.AutoSize = true;

            pnl_question.Controls.Add(lbl1_question);
            pnl_question.Controls.Add(txtBx1_question);
            pnl_question.Controls.Add(lbl2_question);
            pnl_question.Controls.Add(txtBx2_question);

            panel_Main.Controls.Add(pnl_question);
            int temp = panel_Main.Controls.GetChildIndex(pnl_newquestion);
            panel_Main.Controls.SetChildIndex(pnl_newquestion, panel_Main.Controls.GetChildIndex(pnl_question));
            panel_Main.Controls.SetChildIndex(pnl_question, temp);
        }
        private void Question_complex(Panel pnl_newquestion)
        {
            Panel pnl_question_optional = new Panel();
            pnl_question_optional.BorderStyle = BorderStyle.None;
            pnl_question_optional.Size = new Size(520, 140);
            pnl_question_optional.Location = new System.Drawing.Point(14, 13 + this.Controls.OfType<System.Windows.Forms.Control>().Count(c => c is Panel) * 160);

            System.Windows.Forms.Label lbl1_question = new System.Windows.Forms.Label();
            lbl1_question.Text = "Напишите вопрос:";
            lbl1_question.Location = new System.Drawing.Point(3, 4);
            lbl1_question.AutoSize = true;

            TextBox txtBx1_question = new TextBox();
            txtBx1_question.Size = new Size(510, 80);
            txtBx1_question.Location = new System.Drawing.Point(3, 20);
            txtBx1_question.Multiline = true;
            txtBx1_question.AutoSize = true;

            Button btn1_question = new Button();
            btn1_question.Text = "Добавить вариант ответа";
            btn1_question.Size = new Size(150, 23);
            btn1_question.Location = new System.Drawing.Point(3, 100);
            btn1_question.Click += (s, args) =>
            {
                Question_complex_add(pnl_question_optional, btn1_question);
                btn1_question.Location = new System.Drawing.Point(3, 40 + btn1_question.Location.Y);
                pnl_question_optional.Size = new Size(pnl_question_optional.Size.Width, pnl_question_optional.Size.Height + 40);
                pnl_newquestion.Location = new System.Drawing.Point(pnl_newquestion.Location.X, pnl_newquestion.Location.Y + 40);
            };

            pnl_question_optional.Controls.Add(lbl1_question);
            pnl_question_optional.Controls.Add(txtBx1_question);
            pnl_question_optional.Controls.Add(btn1_question);

            panel_Main.Controls.Add(pnl_question_optional);
            int temp = panel_Main.Controls.GetChildIndex(pnl_newquestion);
            panel_Main.Controls.SetChildIndex(pnl_newquestion, panel_Main.Controls.GetChildIndex(pnl_question_optional));
            panel_Main.Controls.SetChildIndex(pnl_question_optional, temp);
        }
        private void Question_complex_add(Panel panel, Button button)
        {
            Panel pnl_answer_optional = new Panel();
            pnl_answer_optional.BorderStyle = BorderStyle.None;
            pnl_answer_optional.Size = new Size(500, 35);
            pnl_answer_optional.Location = new System.Drawing.Point(3, button.Location.Y);
            pnl_answer_optional.Tag = "pnl_answer_optional";

            System.Windows.Forms.Label lbl2_question_optional = new System.Windows.Forms.Label();
            lbl2_question_optional.Text = "Напишите вариант ответа:";
            lbl2_question_optional.Location = new System.Drawing.Point(3, 0);
            lbl2_question_optional.AutoSize = true;

            TextBox txtBx2_question_optional = new TextBox();
            txtBx2_question_optional.Size = new Size(300, 23);
            txtBx2_question_optional.Location = new System.Drawing.Point(3, 15);
            txtBx2_question_optional.AutoSize = true;

            System.Windows.Forms.CheckBox checkBox_question_optional = new System.Windows.Forms.CheckBox();
            checkBox_question_optional.Text = "Правильный ответ";
            checkBox_question_optional.Location = new System.Drawing.Point(10 + txtBx2_question_optional.Location.X + txtBx2_question_optional.Size.Width, 2 + txtBx2_question_optional.Location.Y);
            checkBox_question_optional.AutoSize = true;

            pnl_answer_optional.Controls.Add(lbl2_question_optional);
            pnl_answer_optional.Controls.Add(txtBx2_question_optional);
            pnl_answer_optional.Controls.Add(checkBox_question_optional);

            panel.Controls.Add(pnl_answer_optional);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            switch (button1.Text)
            {
                case "Сохранить":
                    {
                        try
                        {
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();

                                using (SqlCommand cmd = new SqlCommand("CreateTable", connection))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@TableName", label1.Text);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            SaveQuestions();
                            MessageBox.Show("Тест успешно сохранён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex) { MessageBox.Show("У нас произошли неполадки! " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        break;
                    }
                case "Распечатать":
                    {
                        try { Print_test(); }
                        catch (Exception ex) { MessageBox.Show("У нас произошли неполадки! " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        break;
                    }
                case "Отправить":
                    {
                        try { Send_results(); }
                        catch (Exception ex) { MessageBox.Show("У нас произошли неполадки! " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        break;
                    }
            }
        }
        public void SaveQuestions()
        {
            int count_questions = 1;
            foreach (System.Windows.Forms.Control control in panel_Main.Controls)
            {
                if (control is Panel questionPanel)
                {
                    var textBoxes = questionPanel.Controls.OfType<TextBox>().ToList();
                    if (textBoxes.Count == 0) { continue; }
                    var questionTextBox = textBoxes.First();
                    string questionText = Convert.ToString(count_questions) + ". " + questionTextBox?.Text.Trim() ?? "";
                    count_questions++;
                    bool isComplexQuestion = questionPanel.Controls.OfType<Button>().Any(b => b.Text.Contains("Добавить вариант ответа"));
                    string answerText = "";
                    string rightAnswer = "";
                    if (isComplexQuestion)
                    {
                        int count_answer = 1;
                        var answerPanels = questionPanel.Controls.OfType<Panel>().Where(p => p.Tag != null && p.Tag.ToString() == "pnl_answer_optional").ToList();
                        foreach (var ansPanel in answerPanels)
                        {
                            var txtAnswer = ansPanel.Controls.OfType<TextBox>().FirstOrDefault();
                            var checkBox = ansPanel.Controls.OfType<System.Windows.Forms.CheckBox>().FirstOrDefault();
                            string answerItem = txtAnswer?.Text.Trim() ?? "";
                            if (string.IsNullOrEmpty(answerItem)) { continue; }
                            string answerNumbered = $"| {count_answer}. {answerItem}";
                            if (string.IsNullOrEmpty(answerText)) { answerText = answerNumbered; }
                            else { answerText += "\n" + answerNumbered; }
                            if (checkBox.Checked) { rightAnswer = answerNumbered; }
                            count_answer++;
                        }
                        if (string.IsNullOrEmpty(rightAnswer) && answerPanels.Count > 0)
                        {
                            var firstAnsPanel = answerPanels.First();
                            var txtFirst = firstAnsPanel.Controls.OfType<TextBox>().FirstOrDefault();
                            rightAnswer = $"1. {txtFirst?.Text.Trim() ?? ""}";
                        }
                    }
                    else
                    {
                        var answerTextBox = textBoxes.Skip(1).FirstOrDefault();
                        string simpleAnswer = answerTextBox?.Text.Trim() ?? "";
                        if (string.IsNullOrEmpty(simpleAnswer)) continue;
                        answerText = simpleAnswer;
                        rightAnswer = simpleAnswer;
                    }
                    string insertSql = $"INSERT INTO [{label1.Text}] (Question, Answer, Right_answer) VALUES (@question, @answer, @right_answer)";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand cmd = new SqlCommand(insertSql, connection))
                        {
                            cmd.Parameters.AddWithValue("@question", questionText);
                            cmd.Parameters.AddWithValue("@answer", answerText);
                            cmd.Parameters.AddWithValue("@right_answer", rightAnswer);
                            try { cmd.ExecuteNonQuery(); }
                            catch (Exception ex) { MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        }
                    }
                }
            }
        }
        private void Open_test()
        {
            int rowCount = 0;
            string query_count = $"SELECT COUNT(*) FROM [{label1.Text}]";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query_count, connection);
                try
                {
                    connection.Open();
                    rowCount = (int)command.ExecuteScalar();
                }
                catch (Exception ex) { MessageBox.Show("Ошибка при получении количества строк: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            string query_select = $"SELECT * FROM [{label1.Text}]";
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query_select, connection);
                adapter.Fill(table);
                int yOffset = 13;
                for (int i = 0; i < rowCount; i++)
                {
                    Panel pnl_question = new Panel();
                    pnl_question.BorderStyle = BorderStyle.None;
                    pnl_question.Size = new Size(520, 160);
                    pnl_question.Location = new System.Drawing.Point(14, yOffset);

                    System.Windows.Forms.Label lbl1_question = new System.Windows.Forms.Label();
                    lbl1_question.Text = Convert.ToString(table.Rows[i][1]);
                    lbl1_question.Location = new System.Drawing.Point(3, 4);
                    lbl1_question.AutoSize = true;

                    System.Windows.Forms.Label lbl2_question = new System.Windows.Forms.Label();
                    string[] options = Convert.ToString(table.Rows[i][2]).Split('|');
                    for (int k = 0; k < options.Length; k++) { lbl2_question.Text += options[k] + " \n"; }
                    lbl2_question.Location = new System.Drawing.Point(3, lbl1_question.Location.Y + lbl1_question.Size.Height + 10);
                    lbl2_question.AutoSize = true;

                    pnl_question.Controls.Add(lbl1_question);
                    pnl_question.Controls.Add(lbl2_question);

                    panel_Main.Controls.Add(pnl_question);
                    yOffset += pnl_question.Height + 10;
                }
            }
        }
        private void Print_test()
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Word Files|*.docx", Title = "Сохранить как Word" })
            {
                if (sfd.ShowDialog() != DialogResult.OK) { return; }
                try
                {
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(sfd.FileName, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        Body body = mainPart.Document.AppendChild(new Body());
                        foreach (System.Windows.Forms.Control ctrl in panel_Main.Controls)
                        {
                            if (ctrl is Panel pnl)
                            {
                                foreach (System.Windows.Forms.Control c in pnl.Controls)
                                {
                                    if (c is System.Windows.Forms.Label lbl)
                                    {
                                        DocumentFormat.OpenXml.Drawing.Paragraph para = new DocumentFormat.OpenXml.Drawing.Paragraph();
                                        DocumentFormat.OpenXml.Drawing.Run run = new DocumentFormat.OpenXml.Drawing.Run();
                                        run.AppendChild(new DocumentFormat.OpenXml.Drawing.Text(lbl.Text));
                                        para.AppendChild(run);
                                        body.AppendChild(para);
                                    }
                                }
                                body.AppendChild(new DocumentFormat.OpenXml.Drawing.Paragraph(new DocumentFormat.OpenXml.Drawing.Run(new DocumentFormat.OpenXml.Drawing.Text(" "))));
                            }
                        }
                        mainPart.Document.Save();
                    }
                    MessageBox.Show("Тест успешно сохранён в Word", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }
        private void Pass_test()
        {
            string query_select = $"SELECT * FROM [{label1.Text}]";
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query_select, connection);
                adapter.Fill(table);
                for (int i = 0; i<table.Rows.Count; i++)
                {
                    if (Convert.ToString(table.Rows[i][2]) == Convert.ToString(table.Rows[i][3]))
                    {
                        Panel pnl_question = new Panel();
                        pnl_question.BorderStyle = BorderStyle.None;
                        pnl_question.Size = new Size(520, 50);
                        pnl_question.Location = new System.Drawing.Point(14, 13 + this.Controls.OfType<System.Windows.Forms.Control>().Count(c => c is Panel) * 160);

                        System.Windows.Forms.Label lbl1_question = new System.Windows.Forms.Label();
                        lbl1_question.Text = Convert.ToString(table.Rows[i][1]);
                        lbl1_question.Location = new System.Drawing.Point(3, 4);
                        lbl1_question.AutoSize = true;
                        pnl_question.Size = new Size(pnl_question.Size.Width, pnl_question.Size.Height + lbl1_question.Size.Height);

                        TextBox txtBx_question = new TextBox();
                        txtBx_question.Size = new Size(400, 45);
                        txtBx_question.Location = new System.Drawing.Point(3, lbl1_question.Location.Y + lbl1_question.Size.Height + 10);
                        txtBx_question.AutoSize = true;
                        pnl_question.Size = new Size(pnl_question.Size.Width, pnl_question.Size.Height + txtBx_question.Size.Height);

                        pnl_question.Controls.Add(lbl1_question);
                        pnl_question.Controls.Add(txtBx_question);

                        panel_Main.Controls.Add(pnl_question);
                    }
                    else 
                    {
                        string[] options = Convert.ToString(table.Rows[i][2]).Split('|');

                        Panel pnl_question = new Panel();
                        pnl_question.BorderStyle = BorderStyle.None;
                        pnl_question.Size = new Size(520, 50);
                        pnl_question.Location = new System.Drawing.Point(14, 13 + this.Controls.OfType<System.Windows.Forms.Control>().Count(c => c is Panel) * 160);

                        System.Windows.Forms.Label lbl1_question = new System.Windows.Forms.Label();
                        lbl1_question.Text = Convert.ToString(table.Rows[i][1]);
                        lbl1_question.Location = new System.Drawing.Point(3, 4);
                        lbl1_question.AutoSize = true;
                        pnl_question.Size = new Size(pnl_question.Size.Width, pnl_question.Size.Height + lbl1_question.Size.Height);

                        Panel pnl_answer_optional = new Panel();
                        pnl_answer_optional.BorderStyle = BorderStyle.None;
                        pnl_answer_optional.Size = new Size(300, 35);
                        pnl_answer_optional.Location = new System.Drawing.Point(3, lbl1_question.Location.Y + lbl1_question.Size.Height);
                        pnl_answer_optional.Tag = "pnl_answer_optional";

                        int points = 14;
                        for (int k = 1; k<options.Length; k++)
                        {
                            System.Windows.Forms.CheckBox chckBx_option = new System.Windows.Forms.CheckBox();
                            chckBx_option.Text = options[k];
                            chckBx_option.Location = new System.Drawing.Point(3, lbl1_question.Location.Y + lbl1_question.Size.Height + points);
                            chckBx_option.AutoSize = true;
                            pnl_answer_optional.Size = new Size(pnl_answer_optional.Width, pnl_answer_optional.Height + chckBx_option.Size.Height);
                            pnl_question.Size = new Size(pnl_question.Size.Width, pnl_question.Size.Height + chckBx_option.Size.Height);
                            pnl_answer_optional.Controls.Add(chckBx_option);
                            points += chckBx_option.Height;
                        }
                        pnl_question.Controls.Add(lbl1_question);
                        pnl_question.Controls.Add(pnl_answer_optional);

                        panel_Main.Controls.Add(pnl_question);
                    }
                }
            }
        }
        private void Send_results()
        {
            string query_select = $"SELECT * FROM [{label1.Text}]";
            DataTable table = new DataTable();
            int questions_count = 0;
            int right_questions_count = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query_select, connection);
                adapter.Fill(table);
                foreach (System.Windows.Forms.Control control in panel_Main.Controls)
                {
                    if (control is Panel questionPanel)
                    {
                        questions_count++;
                        bool isComplexQuestion = questionPanel.Controls.OfType<Panel>().Where(p => p.Tag != null && p.Tag.ToString() == "pnl_answer_optional").Any();
                        if(isComplexQuestion)
                        {
                            var answerPanels = questionPanel.Controls.OfType<Panel>().Where(p => p.Tag != null && p.Tag.ToString() == "pnl_answer_optional").ToList();
                            foreach (var ansPanel in answerPanels)
                            {
                                var checkBox = ansPanel.Controls.OfType<System.Windows.Forms.CheckBox>().ToList();
                                for (int i = 0; i < checkBox.Count; i++)
                                {
                                    if (checkBox[i].Checked)
                                    {
                                        string[] options = Convert.ToString(table.Rows[questions_count - 1][3]).Split('|');
                                        for (int k = 1; k < options.Length; k++)
                                        {
                                            string right_answer = options[k] + " ";
                                            string checkBox_answer = checkBox[i].Text;
                                            if (checkBox_answer == right_answer) { right_questions_count++; }
                                        }
                                    }
                                }
                            }
                        }
                        else 
                        {
                            var textBoxes = questionPanel.Controls.OfType<TextBox>().ToList();
                            var answerTextBox = textBoxes.FirstOrDefault();
                            string simpleAnswer = answerTextBox?.Text.Trim() ?? "";
                            if (simpleAnswer == Convert.ToString(table.Rows[questions_count-1][3])) { right_questions_count++; }
                        }
                    }
                }
            }
            string yeargroup = label3.Text.Replace("Группа - ", "").Trim();
            string year = yeargroup.Length >= 2 ? yeargroup.Substring(0, 2) : yeargroup;
            string group = yeargroup.Length > 2 ? yeargroup.Substring(2) : "";
            string testName = label1.Text;
            string fio = label2.Text.Replace("Студент - ", "").Trim();
            float accuracy;
            if (right_questions_count == 0) { accuracy = 0; }
            else { accuracy = questions_count / right_questions_count * 100; }
            string today = DateTime.Today.ToString("dd/MM/yyyy");
            try
            {
                string query = @"INSERT INTO Results (Year, [Group], Name_test, Name_student, Accuracy, Date) VALUES (@Year, @Group, @Name_test, @Name_student, @Accuracy, @Date)";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Year", year);
                    command.Parameters.AddWithValue("@Group", group);
                    command.Parameters.AddWithValue("@Name_test", testName);
                    command.Parameters.AddWithValue("@Name_student", fio);
                    command.Parameters.AddWithValue("@Accuracy", accuracy);
                    command.Parameters.AddWithValue("@Date", today);
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Данные успешно добавлены", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка при добавлении данных - " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}