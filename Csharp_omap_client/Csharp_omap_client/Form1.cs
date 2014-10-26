using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Csharp_omap_client
{
    public partial class Fclient : Form
    {
        public Fclient()
        {
            InitializeComponent();
            timer1.Interval = 2000;
            timer1.Stop();
            
            
        }
        string constr;
        bool running = false;
        SqlDataAdapter myda1;
        DataSet myds1;
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            constr =string.Format( "server={0},{1};database=DataRecover;Uid={2};Pwd={3}",
                IPbox.Text.ToString(),
                Portbox.Text.ToString(),
                userbox.Text.ToString(),
                passwdbox.Text.ToString()
            );


            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("Select Name FROM SysObjects Where XType='U' ORDER BY Name  ", con);
                try
                {
                    //3.打开连接
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    comboBox1.Items.Clear();
                    while (dr.Read())
                    {
                        comboBox1.Items.Add(dr.GetValue(0).ToString());
                        PortX.Items.Add(dr.GetValue(0).ToString());
                    }
                    con.Close();
                    comboBox1.SelectedIndex = 1;
                    PortX.SelectedIndex = 0;
                    running = true;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally 
                {
                    using (StreamWriter sw = new StreamWriter(Application.StartupPath + @"\ClientConf", false, Encoding.UTF8))
                    {
                        sw.WriteLine(":");
                        sw.WriteLine(IPbox.Text.ToString());
                        sw.WriteLine(Portbox.Text.ToString());
                        sw.WriteLine(userbox.Text.ToString());
                        sw.WriteLine(passwdbox.Text.ToString());
                        sw.WriteLine(dbnamebox.Text.ToString());
                        sw.WriteLine(";");
                               
                    }
                }
            }
        }
        /// <summary>
        /// 查询原始数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DB_test_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {

                string sqlstr1 = string.Format("select top 15 * from {0} order by [RID] desc ",comboBox1.SelectedItem.ToString());

                SqlDataAdapter myda1 = new SqlDataAdapter(sqlstr1, con);

                DataSet myds1 = new DataSet();

                try
                {
                    con.Open();
                    myda1.Fill(myds1, comboBox1.SelectedItem.ToString());
                    dataGridView1.DataSource = myds1.Tables[0];
                    con.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void IPbox_ModifiedChanged(object sender, EventArgs e)
        {
            Portbox.Enabled = true;
            button4.Enabled = false;
        }

        private void Portboox_ModifiedChanged(object sender, EventArgs e)
        {
            userbox.Enabled = true;
            dbnamebox.Enabled = true;
        }

        private void userbox_ModifiedChanged(object sender, EventArgs e)
        {
            passwdbox.Enabled = true;
            button4.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            flashchart();
        }

        private void flashchart()
        {
            chart1.Series[0].Points.Clear();
            using (SqlConnection con = new SqlConnection(constr))
            {
                
                string sqlstr1 = string.Format("select top 100 * from {0} order by [RID] desc ", PortX.SelectedItem.ToString());
                myda1 = new SqlDataAdapter(sqlstr1, con);
                myds1 = new DataSet();
                myds1.Clear();
                try
                {
                    con.Open();
                    myda1.Fill(myds1, this.PortX.SelectedItem.ToString());
                    con.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                for (int i = 0; i < myds1.Tables[0].Rows.Count; i++)
                    this.chart1.Series[0].Points.AddY(myds1.Tables[0].Rows[i][1]);
                this.chart1.Series[0].ChartType = SeriesChartType.Line;
            }
        }

        private void PortX_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox s1 = (ComboBox)sender;
            
            if(running)
                flashchart();
           
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox1.CheckState == CheckState.Checked)
            {
                if (running)
                {
                    timer1.Interval = Convert.ToInt32(timerbox.Text.ToString());
                    timer1.Start();
                }
                else
                    MessageBox.Show("Set time !");
            }
            else
            {
                timer1.Stop();
            }
        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {
            IPbox.Enabled = true;
            Portbox.Enabled = false;
            userbox.Enabled = false;
            passwdbox.Enabled = false;
            dbnamebox.Enabled = false;
            using (StreamReader sr = new StreamReader(@".\ClientConf", Encoding.UTF8))
            {
                while (!sr.EndOfStream)
                {
                    if (sr.ReadLine().ToString().Equals(":"))
                    {
                        IPbox.Text = sr.ReadLine().ToString();
                        Portbox.Text = sr.ReadLine().ToString();
                        userbox.Text = sr.ReadLine().ToString();
                        passwdbox.Text = sr.ReadLine().ToString();
                        dbnamebox.Text = sr.ReadLine().ToString();
                    }
                    else
                        continue;
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(@".\ClientConf", Encoding.UTF8))
            {
                while (!sr.EndOfStream)
                {
                    if (sr.ReadLine().ToString().Equals(":"))
                    {
                        IPbox.Text = sr.ReadLine().ToString();
                        Portbox.Text = sr.ReadLine().ToString();
                        userbox.Text = sr.ReadLine().ToString();
                        passwdbox.Text = sr.ReadLine().ToString();
                        dbnamebox.Text = sr.ReadLine().ToString();
                    }
                    else
                        continue;
                }
            }
        }

        
    }
}
