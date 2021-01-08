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
using System.IO;

namespace Potezny_projekt
{
    public partial class BazaDanych : Form
    {
        public BazaDanych(Menu menu_pr, FormOverlay fov, RichTextBox rtb)
        {
            menu = menu_pr;
            formov = fov;
            text = rtb;
            InitializeComponent();
            Fill();
        }

        private string gcode_string="";
        private List<int> listview_id_global;
        private List<int> listview_id_obrazu_global;

        private List<int> listview_id_global2;
        private List<int> listview_id_obrazu_global2;

        private Image[] obrazki;
        //public int flaga=0;
        private Menu menu;
        private FormOverlay formov;
        private RichTextBox text;
        private void Fill()
        {
            listBox1.Items.Clear();
            SqlConnection con = new SqlConnection("Data Source=DESKTOP-3GCP9O2\\BAZAWARIATA;Initial Catalog=projekt_laser;Persist Security Info=True;User ID=sa;Password=okon");
            string query = "select * from LASER";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<int> listview_id = new List<int>();
                    while (reader.Read())
                    {
                        string Id;
                        Id = reader["ID_KONFIGURACJI"].ToString();
                        listBox1.Items.Add(Id + "\t\t    " + reader["MOC_LASERA"].ToString() + "\t\t" + 
                            reader["PREDKOSC"].ToString() + "\t        " + reader["LICZBA_PRZEJSC"].ToString());
                        listview_id.Add(Int32.Parse(Id));
                    }
                    listview_id_global = listview_id;
                    con.Close();

                }
                catch (SqlException a)
                {
                    MessageBox.Show(a.Message.ToString(), "Ups, coś poszło nie tak");
                }
            }
        }

        private void Fill2()
        {
            listBox2.Items.Clear();
            SqlConnection con = new SqlConnection("Data Source=DESKTOP-3GCP9O2\\BAZAWARIATA;Initial Catalog=projekt_laser;Persist Security Info=True;User ID=sa;Password=okon");
            string query = "select GCODE.ID_GCODU, GCODE.Opis from LASER join GCODE ON LASER.ID_KONFIGURACJI=GCODE.ID_KONFIGURACJI where GCODE.ID_KONFIGURACJI=" + listview_id_global[listBox1.SelectedIndex];
            using (SqlCommand cmd = new SqlCommand(query, con))
            {

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<int> listview_id = new List<int>();
                    while (reader.Read())
                    {
                        string Id;
                        Id = reader["ID_GCODU"].ToString();
                        listBox2.Items.Add(Id + "\t\t    " + reader["Opis"].ToString());
                        listview_id.Add(Int32.Parse(Id));
                    }
                    listview_id_global2 = listview_id;
                    con.Close();
                    textBox4.Text = listview_id_global[listBox1.SelectedIndex].ToString();

                }
                catch (SqlException a)
                {
                    MessageBox.Show(a.Message.ToString(), "Ups, coś poszło nie tak");
                }
            }
        }


        private void Ustaw_listview()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.Columns.Add("Do przesłania", 200);
            listView1.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize);
            
        }

        private void Ustaw_listview2()
        {
            listView2.Clear();
            listView2.View = View.Details;
            listView2.Columns.Add("Obrazy w bazie", 200);
            listView2.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize);

        }

        private void Obrazy_Load(object sender, EventArgs e)
        {
            Ustaw_listview();
            Ustaw_listview2();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            Ustaw_listview();


            Image[] images = new Image[openFileDialog1.FileNames.Length];
            ImageList imgs = new ImageList();

            imgs.ImageSize = new Size(192, 108);

            try
            {
                int i = 0;
                foreach (String file in openFileDialog1.FileNames)
                {
                    imgs.Images.Add(Image.FromFile(file));
                    images[i] = Image.FromFile(file);
                    i++;
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }

            listView1.SmallImageList = imgs;

            {
                int i = 0;
                foreach (String file in openFileDialog1.FileNames)
                {
                    listView1.Items.Add("", i);
                    i++;
                }
            }
            obrazki = images;
            //flaga = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                listView2.Clear();
                listBox2.Items.Clear();
                SqlConnection con = new SqlConnection("Data Source=DESKTOP-3GCP9O2\\BAZAWARIATA;Initial Catalog=projekt_laser;Persist Security Info=True;User ID=sa;Password=okon");
                string query = "select GCODE.ID_GCODU, GCODE.Opis from LASER join GCODE ON LASER.ID_KONFIGURACJI=GCODE.ID_KONFIGURACJI where GCODE.ID_KONFIGURACJI=" + listview_id_global[listBox1.SelectedIndex];
                using (SqlCommand cmd = new SqlCommand(query, con))
                {

                    try
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        List<int> listview_id = new List<int>();
                        while (reader.Read())
                        {
                            string Id;
                            Id = reader["ID_GCODU"].ToString();
                            listBox2.Items.Add(Id + "\t\t    " + reader["Opis"].ToString());
                            listview_id.Add(Int32.Parse(Id));
                        }
                        listview_id_global2 = listview_id;
                        con.Close();
                        textBox4.Text = listview_id_global[listBox1.SelectedIndex].ToString();

                    }
                    catch (SqlException a)
                    {
                        MessageBox.Show(a.Message.ToString(), "Ups, coś poszło nie tak");
                    }
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {

            if (listBox1.SelectedIndex >= 0)
            {
                if (listBox2.SelectedIndex >= 0)
                {
                    if (obrazki != null)
                    {
                        foreach (Bitmap bitmapa in obrazki)
                        {
                            SqlConnection con = new SqlConnection("Data Source=DESKTOP-3GCP9O2\\BAZAWARIATA;Initial Catalog=projekt_laser;Persist Security Info=True;User ID=sa;Password=okon");
                            string query = "insert into OBRAZY(OBRAZ, ID_GCODU, MATERIAL) values (@par1, @par2, @par3)";

                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                MemoryStream stream = new MemoryStream();
                                bitmapa.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                                byte[] content = stream.ToArray();
                                cmd.Parameters.AddWithValue("@par1", content);
                                cmd.Parameters.AddWithValue("@par2", listview_id_global2[listBox2.SelectedIndex]);
                                cmd.Parameters.AddWithValue("@par3", textBox6.Text);

                                try
                                {
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    con.Close();
                                }
                                catch (SqlException a)
                                {
                                    MessageBox.Show(a.Message.ToString(), "Ups, coś poszło nie tak");
                                }
                            }
                        }
                        listBox2_SelectedIndexChanged(listView1, EventArgs.Empty);
                        Ustaw_listview();
                        MessageBox.Show("Proces zakończony pomyślnie");
                    }
                    else
                    {
                        MessageBox.Show("Dodaj obrazy");
                    }
                }
                else
                {
                    MessageBox.Show("Wybierz Gcode");
                }
            }
            else
            {
                MessageBox.Show("Wybierz konfiguracje"); ;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count!=0)
            {
                SqlConnection con = new SqlConnection("Data Source=DESKTOP-3GCP9O2\\BAZAWARIATA;Initial Catalog=projekt_laser;Persist Security Info=True;User ID=sa;Password=okon");
                string query = "delete OBRAZY WHERE ID_OBRAZU=" + listview_id_obrazu_global[listView2.SelectedIndices[0]];

                for(int i=1; i< listView2.SelectedIndices.Count; ++i)
                {
                    query = query + " OR ID_OBRAZU=" + listview_id_obrazu_global[listView2.SelectedIndices[i]];
                }

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        //MessageBox.Show("Zajebiście elo");
                        con.Close();
                        listBox2_SelectedIndexChanged(listBox1, EventArgs.Empty);
                    }
                    catch (SqlException a)
                    {
                        MessageBox.Show(a.Message.ToString(), "Ups, coś poszło nie tak");
                    }
                }
            }
            else
            {
                MessageBox.Show("Zaznacz obrazy do usunięcia");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Ustaw_listview();
            obrazki = null;
        }

        private void Obrazy_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            menu.Enabled = true;
            formov.TopMost = true;
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=DESKTOP-3GCP9O2\\BAZAWARIATA;Initial Catalog=projekt_laser;Persist Security Info=True;User ID=sa;Password=okon");
            string query = "insert into LASER(MOC_LASERA, PREDKOSC, LICZBA_PRZEJSC) values (@par1, @par2, @par3)";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@par1", textBox1.Text);
                cmd.Parameters.AddWithValue("@par2", textBox2.Text);
                cmd.Parameters.AddWithValue("@par3", textBox3.Text);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    //MessageBox.Show("Zajebiście elo");
                    con.Close();
                    Fill();
                }
                catch (SqlException a)
                {
                    MessageBox.Show(a.Message.ToString(), "Ups, coś poszło nie tak");
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {

                foreach (int i in listview_id_global2)
                {
                    SqlConnection con = new SqlConnection("Data Source=DESKTOP-3GCP9O2\\BAZAWARIATA;Initial Catalog=projekt_laser;Persist Security Info=True;User ID=sa;Password=okon");
                    string query = "delete OBRAZY WHERE ID_GCODU=" + i;

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                        catch (SqlException a)
                        {
                            MessageBox.Show(a.Message.ToString(), "Ups, coś poszło nie tak");
                        }
                    }
                }

                SqlConnection con2 = new SqlConnection("Data Source=DESKTOP-3GCP9O2\\BAZAWARIATA;Initial Catalog=projekt_laser;Persist Security Info=True;User ID=sa;Password=okon");
                string query2 = "delete GCODE WHERE ID_KONFIGURACJI=" + listview_id_global[listBox1.SelectedIndex];

                using (SqlCommand cmd = new SqlCommand(query2, con2))
                {
                    try
                    {
                        con2.Open();
                        cmd.ExecuteNonQuery();
                        con2.Close();
                        Fill2();
                        listBox2.SelectedIndex = -1;
                        listBox2_SelectedIndexChanged(listBox2, EventArgs.Empty);
                    }
                    catch (SqlException a)
                    {
                        MessageBox.Show(a.Message.ToString(), "Ups, coś poszło nie tak");
                    }
                }



                SqlConnection con3 = new SqlConnection("Data Source=DESKTOP-3GCP9O2\\BAZAWARIATA;Initial Catalog=projekt_laser;Persist Security Info=True;User ID=sa;Password=okon");
                string query3 = "delete LASER WHERE ID_KONFIGURACJI=" + listview_id_global[listBox1.SelectedIndex];

                using (SqlCommand cmd = new SqlCommand(query3, con3))
                {
                    try
                    {
                        con3.Open();
                        cmd.ExecuteNonQuery();
                        con3.Close();
                        Fill();
                        listBox1.SelectedIndex = -1;
                        listBox1_SelectedIndexChanged(listBox1, EventArgs.Empty);
                    }
                    catch (SqlException a)
                    {
                        MessageBox.Show(a.Message.ToString(), "Ups, coś poszło nie tak");
                    }
                }

            }
            else 
            {
                MessageBox.Show("Wybierz konfiguracje");
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            Bitmap bitmapa;
            if (listBox1.SelectedIndex >= 0)
            {
                //if (obrazki != null)
                //{

                    string query = "SELECT [OBRAZ] FROM [OBRAZY] WHERE [ID_OBRAZU]=" + listview_id_obrazu_global[listView2.SelectedIndices[0]];
                    SqlConnection con = new SqlConnection("Data Source=DESKTOP-3GCP9O2\\BAZAWARIATA;Initial Catalog=projekt_laser;Persist Security Info=True;User ID=sa;Password=okon");


                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {

                        try
                        {
                            con.Open();
                            SqlDataReader reader = cmd.ExecuteReader();

                            //List<int> listview_id_obrazu = new List<int>();
                            while (reader.Read())
                            {
                                //int ID_OBRAZU = (int)reader["ID_OBRAZU"];
                                //listview_id_obrazu.Add(ID_OBRAZU);
                                byte[] content = (byte[])reader["OBRAZ"];
                                MemoryStream stream = new MemoryStream(content);
                                //imgs.Images.Add(Image.FromStream(stream));
                                //listview_id_obrazu_global = listview_id_obrazu;
                                bitmapa = new Bitmap(stream);
                                Show_image show_image_form = new Show_image(bitmapa);
                                show_image_form.Show();

                            }

                            
                            
                            //MessageBox.Show("Proces zakończony pomyślnie");
                            con.Close();
                        }
                        catch (SqlException a)
                        {
                            MessageBox.Show(a.Message.ToString(), "Ups, coś poszło nie tak");
                        }
                    }
                   
            }
            else
            {
                MessageBox.Show("Wybierz konfiguracje");
            }

            //MessageBox.Show("jet git");
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex >= 0)
            {
                listView2.Clear();
                listView2.View = View.Details;
                listView2.Columns.Add("Obrazy w bazie ID=" + listview_id_global[listBox1.SelectedIndex], 200);
                listView2.Columns.Add("Materiał", 200);
                //listView2.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize);

                ImageList imgs = new ImageList();
                imgs.ImageSize = new Size(192, 108);

                List<string> materialsy = new List<string>();


                SqlConnection con = new SqlConnection("Data Source=DESKTOP-3GCP9O2\\BAZAWARIATA;Initial Catalog=projekt_laser;Persist Security Info=True;User ID=sa;Password=okon");
                string query = "select ID_OBRAZU, OBRAZ, MATERIAL from OBRAZY where ID_GCODU=" + listview_id_global2[listBox2.SelectedIndex];

                //SqlConnection con = new SqlConnection("Data Source=DESKTOP-3GCP9O2\\BAZAWARIATA;Initial Catalog=projekt_laser;Persist Security Info=True;User ID=sa;Password=okon");
                //string query = "select OBRAZY.ID_OBRAZU, OBRAZY.OBRAZ, OBRAZY.MATERIAL from OBRAZY join GCODE on OBRAZY.ID_GCODU=GCODE.ID_GCODU where GCODE.ID_KONFIGURACJI=" + listview_id_global[listBox1.SelectedIndex];

                using (SqlCommand cmd = new SqlCommand(query, con))
                {

                    try
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        List<int> listview_id_obrazu = new List<int>();
                        while (reader.Read())
                        {
                            int ID_OBRAZU = (int)reader["ID_OBRAZU"];
                            listview_id_obrazu.Add(ID_OBRAZU);
                            byte[] content = (byte[])reader["OBRAZ"];
                            MemoryStream stream = new MemoryStream(content);
                            imgs.Images.Add(Image.FromStream(stream));
                            string m = (string)reader["MATERIAL"];
                            materialsy.Add(m);
                        }
                        listview_id_obrazu_global = listview_id_obrazu;
                        con.Close();
                    }
                    catch (SqlException a)
                    {
                        MessageBox.Show(a.Message.ToString(), "Ups, coś poszło nie tak");
                    }
                }

                listView2.SmallImageList = imgs;

                
                    int i = 0;
                    foreach (Image file in imgs.Images)
                    {
                    string[] arr = { "", materialsy[i] };
                    ListViewItem itm = new ListViewItem(arr, i);
                        listView2.Items.Add(itm);
                        i++;
                    }

                //foreach (Image file in imgs.Images)
                //{
                //    listView2.Items.Add
                //    i++;
                //}


            }
            else
            { listView2.Clear(); }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            gcode_string = File.ReadAllText(openFileDialog2.FileName);
            button8.ForeColor = Color.Green;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=DESKTOP-3GCP9O2\\BAZAWARIATA;Initial Catalog=projekt_laser;Persist Security Info=True;User ID=sa;Password=okon");
            string query = "insert into GCODE(G_CODE, ID_KONFIGURACJI, Opis) values (@par1, @par2, @par3)";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@par1", gcode_string);
                cmd.Parameters.AddWithValue("@par2", textBox4.Text);
                cmd.Parameters.AddWithValue("@par3", textBox5.Text);


                try
                {
                    int select = listBox2.SelectedIndex;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    //MessageBox.Show("Zajebiście elo");
                    con.Close();
                    Fill2();
                    button8.ForeColor = Color.Black;
                    gcode_string = "";
                    if (select == -1) select = 0;
                    listBox2.SelectedIndex = select;
                }
                catch (SqlException a)
                {
                    MessageBox.Show(a.Message.ToString(), "Ups, coś poszło nie tak");
                }

            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex >= 0)
            {
                SqlConnection con = new SqlConnection("Data Source=DESKTOP-3GCP9O2\\BAZAWARIATA;Initial Catalog=projekt_laser;Persist Security Info=True;User ID=sa;Password=okon");
                string query = "delete OBRAZY WHERE ID_GCODU=" + listview_id_global2[listBox2.SelectedIndex];

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (SqlException a)
                    {
                        MessageBox.Show(a.Message.ToString(), "Ups, coś poszło nie tak");
                    }
                }

                con = new SqlConnection("Data Source=DESKTOP-3GCP9O2\\BAZAWARIATA;Initial Catalog=projekt_laser;Persist Security Info=True;User ID=sa;Password=okon");
                query = "delete GCODE WHERE ID_GCODU=" + listview_id_global2[listBox2.SelectedIndex];

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        Fill2();
                        listBox2.SelectedIndex = -1;
                        listBox2_SelectedIndexChanged(listBox1, EventArgs.Empty);
                    }
                    catch (SqlException a)
                    {
                        MessageBox.Show(a.Message.ToString(), "Ups, coś poszło nie tak");
                    }
                }
            }
            else
            {
                MessageBox.Show("Wybierz gcode");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {

            if (listBox2.SelectedIndex >= 0)
            {
                SqlConnection con = new SqlConnection("Data Source=DESKTOP-3GCP9O2\\BAZAWARIATA;Initial Catalog=projekt_laser;Persist Security Info=True;User ID=sa;Password=okon");
                string query = "select G_CODE from GCODE where ID_GCODU=" + listview_id_global2[listBox2.SelectedIndex];

                using (SqlCommand cmd = new SqlCommand(query, con))
                {

                    try
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            string m = (string)reader["G_CODE"];
                            menu.gcode_string = m;
                            text.Text = m;
                            this.Close();
                        }
                        con.Close();
                        
                    }
                    catch (SqlException a)
                    {
                        MessageBox.Show(a.Message.ToString(), "Ups, coś poszło nie tak");
                    }
                }
            }
            else
            {
                MessageBox.Show("Wybierz Gcode");
            }
        }
    }
}
