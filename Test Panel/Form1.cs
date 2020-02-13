using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Test_Panel
{
   
    public partial class Form1 : Form
    {
        int[,] Available = new int[6,13];
        int Day, Start_Course, End_Course;
        
        SqlConnection SQL_Con;
        SqlDataAdapter SQL_Da;
        SqlCommand SQL_command;
        DataSet DS;
        SqlConnection connection;

        DataTable TimeTable = new DataTable();

        List<string> Available_Time = new List<string>();
        List<Panel> List_of_Panel = new List<Panel>();

        string cn;
        string Degree, Year, Gender;
        string[] Row_Name = { "0", "0800~0900", "0900~1000", "1000~1100", "1100~1200", "1200~1300", "1300~1400", "1400~1500", "1500~1600", "1600~1700", "1700~1800", "1800~1900" };
        string[] Col_Name = { "0","Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
        string Selected_Course_Name;
        string Selected_Course_Time;
        public Form1()
        {
            //this.Size = new Size(1366, 720);

            for (int i = 1; i < 6; i++)
            {
                for (int j = 1; j < 13; j++)
                {
                    Available[i, j] = 0;
                }
            }
            cn = @"Data Source=(LocalDB)\MSSQLLocalDB;" +
                 "AttachDbFilename=|DataDirectory|Database1.mdf;" +
                "Integrated Security=True";
            SQL_Con = new SqlConnection(cn);
            SQL_Con.Open();
            DataColumn[] Date_And_Time = { new DataColumn("Time"), new DataColumn("Monday"), new DataColumn("Tuesday"), new DataColumn("Wednesday"), new DataColumn("Thursday"), new DataColumn("Friday") };
            InitializeComponent();
            TimeTable.Columns.AddRange(Date_And_Time);
            TimeTable.PrimaryKey = new DataColumn[] { TimeTable.Columns["Time"] };
            
            for (int i = 8, j = 0; i < 20; i++, j++)
            {
                DataRow row = TimeTable.NewRow();
                switch (i)
                {
                    case 8:
                        row["Time"] = "0" + i + "00~0" + (i + 1) + "00";
                        break;
                    case 9:
                        row["Time"] = "0" + i + "00~" + (i + 1) + "00";
                        break;
                    default:
                        row["Time"] = i + "00~" + (i + 1) + "00";
                        break;
                }
                        TimeTable.Rows.Add(row);
            }


            
            //DataRow new_row = TimeTable.Rows.Find("0800~0900");
            //new_row["Monday"] = "Linear Algebra";
            // new_row = TimeTable.Rows.Find("0900~1000");
            //new_row["Monday"] = "Linear Algebra";
            // new_row = TimeTable.Rows.Find("1000~1100");
            //new_row["Monday"] = "Linear Algebra";
            //new_row = TimeTable.Rows.Find("0800~0900");
            //new_row["Tuesday"] = "Calculus";
            //new_row = TimeTable.Rows.Find("0900~1000");
            //new_row["Tuesday"] = "Calculus";
            //new_row = TimeTable.Rows.Find("1200~1300");
            //new_row["Tuesday"] = "Programming";
            //new_row = TimeTable.Rows.Find("1300~1400");
            //new_row["Tuesday"] = "Programming";
            //Available[1, 1] = 1;
            //Available[1, 2] = 1;
            //Available[1, 3] = 1;
            //Available[2, 1] = 1;
            //Available[2, 2] = 1;
            //Available[2, 5] = 1;
            //Available[2, 6] = 1;



            List_of_Panel.Add(pnlLogin);
            List_of_Panel.Add(pnlTime);
            List_of_Panel.Add(pnlSelect);
            List_of_Panel[0].BringToFront();
            //pnlLogin.Visible = true;
            //pnlTime.Visible = false;
            //pnlSelect.Visible = false;
            pnlLogin.Visible = true;
            pnlTime.Visible = false;
            pnlSelect.Visible = false;


            tblTimeTable.MultiSelect = true;
        }


        private void tblTimeTable_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            int No_Selected_Cell = tblTimeTable.GetCellCount(DataGridViewElementStates.Selected);
            if (No_Selected_Cell > 0)
            {
                if (check_Selected(No_Selected_Cell))
                {
                    
                    List_of_Panel[2].BringToFront();
                    Day = tblTimeTable.SelectedCells[0].ColumnIndex;
                    updateAvailableTime(No_Selected_Cell);
                    pnlSelect.Visible = true;
                }
                else
                {
                    MessageBox.Show("Selected time has occupied by another course", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (cbDegree.Text != "" && cbYear.Text != "" && cbGender.Text != "")
            {
                Degree = cbDegree.SelectedItem.ToString();
                btnOk.PerformClick();
                Year = cbYear.SelectedItem.ToString();
                Gender = cbGender.SelectedItem.ToString();


                tblTimeTable.DataSource = null;
                List<Dictionary<string, string>> courses = (List<Dictionary<string, string>>)cbDegree.Tag;
                Dictionary<string, string> course = courses[cbDegree.SelectedIndex];
                MessageBox.Show(course["link"]);

                // Load list of classes
                this.Cursor = Cursors.WaitCursor;   // show an Hour-Glass cursor

                Scrapper scrapper = new Scrapper();
                List<Dictionary<string, string>> classes = scrapper.GetClassList(course["link"]);

                // Show data in dataGridView
                for (int i = 0; i < 13; i++)
                {
                    tblTimeTable.Columns.Add("", "");
                }

                foreach (Dictionary<string, string> className in classes)
                {
                    tblTimeTable.Rows.Add(new object[]
                    {
                    className["dept"],
                    className["code"],
                    className["serial"],
                    className["classNo"],
                    className["year"],
                    className["category"],
                    className["english"],
                    className["course"],
                    className["type"],
                    className["credit"],
                    className["instructor"],
                    className["schedule"],
                    className["classroom"]
                    });
                }
                this.Cursor = Cursors.Default;  // show default cursor pointer

                // Clear database
                ClearDatabase();

                // Put data into the database
                InputData(0);
                //label4.Text = tblTimeTable.Rows[0].Cells[11].Value.ToString();

                for (int i = 0; i < tblTimeTable.Rows.Count - 1; i++)
                {
                    int temp = i;
                    DataGridViewRow row = tblTimeTable.Rows[temp];
                    Selected_Course_Time = row.Cells[11].Value.ToString();
                    Selected_Course_Name = row.Cells[7].Value.ToString();
                    if (Selected_Course_Name != null && Selected_Course_Time != null)
                    {
                        string[] temp_seperate = Selected_Course_Time.Split(']');
                        if (!Int32.TryParse(temp_seperate[0].Remove(0, 1),out Day)) {
                            continue;
                        }

                        if (temp_seperate[1][1] == '~' || temp_seperate[1][2]== '~')
                        {
                            temp_seperate = temp_seperate[1].Split('~');
                            if (temp_seperate[0][0] != 'N')
                            {
                                Start_Course = int.Parse(temp_seperate[0]);
                            }
                            else
                            {
                                Start_Course = 5;
                            }
                            if (temp_seperate[1][0] != 'N')
                            {
                                End_Course = int.Parse(temp_seperate[1]);
                            }
                            else
                            {
                                End_Course = 4;
                            }
                        }
                        else
                        {
                            if(temp_seperate[1][0] == 'N')
                            {
                                Start_Course = 5;
                            }
                            else if(!Int32.TryParse(temp_seperate[1],out Start_Course))
                            {
                                continue;
                            }

                            End_Course = Start_Course;
                        }
                        for (int j = Start_Course; j <= End_Course && j>0; j++)
                        {
                            DataRow temp_row = TimeTable.Rows.Find(Row_Name[j]);
                            temp_row[Col_Name[Day]] = Selected_Course_Name;
                            Available[Day, j] = 1;
                        }
                    }
                }
                List_of_Panel[1].BringToFront();
                pnlLogin.Visible = true;
                pnlTime.Visible = true;
                pnlSelect.Visible = false;
                List_of_Panel[1].BringToFront();
                pnlTime.Visible = true;
                tblTimeTable.DataSource = TimeTable;

            }
            else
            {
                MessageBox.Show("Combobox cannot be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            int No_Selected_Cell = tblTimeTable.GetCellCount(DataGridViewElementStates.Selected);
            if (No_Selected_Cell > 0)
            {
                if (!check_Selected(No_Selected_Cell))
                {
                    if (DialogResult.Yes == MessageBox.Show("Are you sure to remove this course from timetable?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                    {
                        for (int i = 0; i < No_Selected_Cell; i++)
                        {
                            int col_Selected = tblTimeTable.SelectedCells[i].ColumnIndex;
                            int row_Selected = tblTimeTable.SelectedCells[i].RowIndex;
                            row_Selected++;
                            clearItem(col_Selected, row_Selected);
                            Available[col_Selected, row_Selected] = 0;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please selected time that was occupied", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            List_of_Panel[1].BringToFront();
            pnlSelect.Visible = false;

        }

        private void cbCourse_SelectedValueChanged(object sender, EventArgs e)
        {
        }

        private void clearItem(int col_index,int row_index)
        {
            DataRow row = TimeTable.Rows.Find(Row_Name[row_index]);
            row[Col_Name[col_index]] = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Kevin
            this.Cursor = Cursors.WaitCursor;   // show an Hour-Glass cursor

            Scrapper scrapper = new Scrapper();
            List<Dictionary<string, string>> courses = scrapper.GetCourseList();
            cbDegree.Tag = courses;
            cbCourse.Tag = courses;

            foreach (Dictionary<string, string> course in courses)
            {
                cbDegree.Items.Add(course["name"]);
                cbCourse.Items.Add(course["name"]);
            }

            this.Cursor = Cursors.Default;  // show default cursor pointer

            //end Kevin 
        }

        private void cbCourse_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cbCourse.Text == "")
            {
                MessageBox.Show("Please select a course", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                tblSelect.DataSource = null;
                List<Dictionary<string, string>> courses = (List<Dictionary<string, string>>)cbCourse.Tag;
                Dictionary<string, string> course = courses[cbCourse.SelectedIndex];
                MessageBox.Show(course["link"]);

                // Load list of classes
                this.Cursor = Cursors.WaitCursor;   // show an Hour-Glass cursor

                Scrapper scrapper = new Scrapper();
                List<Dictionary<string, string>> classes = scrapper.GetClassList(course["link"]);

                // Show data in dataGridView
                for(int i = 0; i < 13; i++)
                {
                    tblSelect.Columns.Add("", "");
                }

                foreach (Dictionary<string, string> className in classes)
                {
                    tblSelect.Rows.Add(new object[]
                    {
                    className["dept"],
                    className["code"],
                    className["serial"],
                    className["classNo"],
                    className["year"],
                    className["category"],
                    className["english"],
                    className["course"],
                    className["type"],
                    className["credit"],
                    className["instructor"],
                    className["schedule"],
                    className["classroom"]
                    });
                }
                this.Cursor = Cursors.Default;  // show default cursor pointer

               // Clear database
                ClearDatabase();

                // Put data into the database
                InputData(1);
            }
        }
        public void InputData( int j)
        {
            connection = new SqlConnection();
            connection.ConnectionString = cn;
            SQL_command = new SqlCommand();
            SQL_command.Connection = connection;
            connection.Open();
         
            if (j == 1)
            {
                for (int i = 0; i < tblSelect.Rows.Count - 1; i++)
                {
                    string cmd = @"INSERT INTO 通識課 VALUES (N'"
                        + tblSelect.Rows[i].Cells[0].Value.ToString() + "', N'"
                        + tblSelect.Rows[i].Cells[1].Value.ToString() + "', N'"
                        + tblSelect.Rows[i].Cells[2].Value.ToString() + "', N'"
                        + tblSelect.Rows[i].Cells[3].Value.ToString() + "', N'"
                        + tblSelect.Rows[i].Cells[4].Value.ToString() + "', N'"
                        + tblSelect.Rows[i].Cells[5].Value.ToString() + "', N'"
                        + tblSelect.Rows[i].Cells[6].Value.ToString() + "', N'"
                        + tblSelect.Rows[i].Cells[7].Value.ToString() + "', N'"
                        + tblSelect.Rows[i].Cells[8].Value.ToString() + "', N'"
                        + tblSelect.Rows[i].Cells[9].Value.ToString() + "', N'"
                        + tblSelect.Rows[i].Cells[10].Value.ToString() + "', N'"
                        + tblSelect.Rows[i].Cells[11].Value.ToString() + "', N'"
                        + tblSelect.Rows[i].Cells[12].Value.ToString() + "')";

                    SQL_command.CommandText = cmd;
                    SQL_command.ExecuteNonQuery();
                }
                string Generate = "SELECT * FROM 通識課  WHERE 時間 IN ({0})";
                string formated = string.Format(Generate, string.Join(",", Available_Time.ToArray()));
                SQL_Da = new SqlDataAdapter(formated, SQL_Con);
                DS = new DataSet();
                SQL_Da.Fill(DS, "Course");

                for (int i = 0; i < 13; i++)
                {
                    tblSelect.Columns.Remove("");
                }
                tblSelect.DataSource = DS.Tables["Course"];
                connection.Close();
            }
            else if (j == 0)
            {
                for (int i = 0; i < tblTimeTable.Rows.Count - 1; i++)
                {
                    string cmd = @"INSERT INTO 通識課 VALUES (N'"
                        + tblTimeTable.Rows[i].Cells[0].Value.ToString() + "', N'"
                        + tblTimeTable.Rows[i].Cells[1].Value.ToString() + "', N'"
                        + tblTimeTable.Rows[i].Cells[2].Value.ToString() + "', N'"
                        + tblTimeTable.Rows[i].Cells[3].Value.ToString() + "', N'"
                        + tblTimeTable.Rows[i].Cells[4].Value.ToString() + "', N'"
                        + tblTimeTable.Rows[i].Cells[5].Value.ToString() + "', N'"
                        + tblTimeTable.Rows[i].Cells[6].Value.ToString() + "', N'"
                        + tblTimeTable.Rows[i].Cells[7].Value.ToString() + "', N'"
                        + tblTimeTable.Rows[i].Cells[8].Value.ToString() + "', N'"
                        + tblTimeTable.Rows[i].Cells[9].Value.ToString() + "', N'"
                        + tblTimeTable.Rows[i].Cells[10].Value.ToString() + "', N'"
                        + tblTimeTable.Rows[i].Cells[11].Value.ToString() + "', N'"
                        + tblTimeTable.Rows[i].Cells[12].Value.ToString() + "')";

                    SQL_command.CommandText = cmd;
                    SQL_command.ExecuteNonQuery();
                }
                string Generate = "SELECT * FROM 通識課 WHERE 年級 ='" + Year + " ' AND 選必修 = N'必修'";
               // MessageBox.Show(Generate);
                SQL_Da = new SqlDataAdapter(Generate, SQL_Con);
                DS = new DataSet();
                SQL_Da.Fill(DS, "Course");

                for (int i = 0; i < 13; i++)
                {
                    tblTimeTable.Columns.Remove("");
                }
                tblTimeTable.DataSource = DS.Tables["Course"];
                connection.Close();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
        public void ClearDatabase()
        {
            connection = new SqlConnection();
            connection.ConnectionString = cn;
            connection.Open();
            string cmd = "DELETE FROM 通識課 ";
            SQL_Da = new SqlDataAdapter(cmd, connection);
            DataTable temp_DT = new DataTable();
            SQL_Da.Fill(temp_DT);
            //dataGridView1.DataSource = temp_DT;
            connection.Close();
        }


        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (tblSelect.GetCellCount(DataGridViewElementStates.Selected) == 0)
            {
                MessageBox.Show("Please select to course to be added", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                if(tblSelect.SelectedCells[0].Value.ToString() != string.Empty);
                {
                    int temp = tblSelect.SelectedCells[0].RowIndex;
                    DataGridViewRow row = tblSelect.Rows[temp];
                    Selected_Course_Time = row.Cells[11].Value.ToString();
                    Selected_Course_Name = row.Cells[7].Value.ToString();
                    if (Selected_Course_Name != null && Selected_Course_Time != null)
                    {
                        string[] temp_seperate = Selected_Course_Time.Split(']');
                        Day = int.Parse(temp_seperate[0].Remove(0, 1));
                        if (temp_seperate[1][1] == '~')
                        {
                            temp_seperate = temp_seperate[1].Split('~');
                            Start_Course = int.Parse(temp_seperate[0]);
                            End_Course = int.Parse(temp_seperate[1]);
                        }
                        else
                        {
                            Start_Course = int.Parse(temp_seperate[1]);
                            End_Course = Start_Course;
                        }
                        for (int i = Start_Course; i <= End_Course; i++)
                        {
                            DataRow temp_row = TimeTable.Rows.Find(Row_Name[i]);
                            temp_row[Col_Name[Day]] = Selected_Course_Name;
                            Available[Day, i] = 1;
                        }
                    }
                    List_of_Panel[1].BringToFront();
                    pnlLogin.Visible = true;
                    pnlTime.Visible = true;
                    pnlSelect.Visible = false;
                }
            }

        }

        private bool check_Selected(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (tblTimeTable.SelectedCells[i].Value.ToString()!= "")
                {
                    return false;
                }
            }
            return true;
        }

        private void updateAvailableTime(int count)
        {
            Available_Time.Clear();
            int rowIndex = tblTimeTable.SelectedCells[0].RowIndex;
            int colIndex = tblTimeTable.SelectedCells[0].ColumnIndex;
            string date = "[" + Day.ToString() + "]";
            rowIndex++;
            Available_Time.Add("'"+date+rowIndex.ToString() +"'");
            for (int i = rowIndex - 1; i > 0; i--)
            {
                if (Available[Day, i] == 0)
                {
                    Available_Time.Add("'" + date + i.ToString() +"'" );
                    Available_Time.Add("'" + date + i.ToString() + "~" + rowIndex.ToString() + "'");
                    int j = i;
                    while (j + 1 != rowIndex)
                    {
                        Available_Time.Add("'" + date + j.ToString() + "~" + (j + 1).ToString() + "'");
                        j++;
                    }

                }
                else
                {
                    break;
                }
            }
            for (int i = rowIndex + 1; i < 13; i++)
            {
                if (Available[Day, i] == 0)
                {
                    Available_Time.Add("'" + date + i.ToString()  + "'");
                    Available_Time.Add("'" + date + rowIndex.ToString() + "~" + i.ToString() + "'");
                    int j = rowIndex+1;
                    while (j != i)
                    {
                        Available_Time.Add("'" + date + j.ToString() + "~" + i.ToString() + "'");
                        j++;
                    }

                }
                else
                {
                    break;
                }
            }

        }
    }
}
