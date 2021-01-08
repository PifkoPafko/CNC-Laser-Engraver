using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO.Ports;

namespace Potezny_projekt
{
    public partial class Menu : Form
    {
        Graphics g;
        FormOverlay overlayform, overlayform_machina;
        Task taskA, taskB, taskC, taskok;
        bool task = false, taskB_bool=false;
        bool przerwij_wypalanie = false;
        bool przewin = false;
        string[] ports;
        int com;
        SerialPort serialport;
        string zwrotna;

        

        private object syncObj = new object();
        private bool paused, timeout= false;

        List<int> opcja = new List<int>();
        List<int> X_pr_list = new List<int>();
        List<int> Y_pr_list = new List<int>();
        List<int> X_list = new List<int>();
        List<int> Y_list = new List<int>();
        List<int> a_list = new List<int>();
        List<int> b_list = new List<int>();
        List<int> w_list = new List<int>();
        List<int> kat_start_list = new List<int>();
        List<int> kat_rys_list = new List<int>();
        List<int> t = new List<int>();
        List<bool> czy_rysowac = new List<bool>();
        public string gcode_string;

        bool gcodeflag;
        bool mozna_wyslac = false, do_sprawdzenia = false, status_exit = false, po_automatycznym = false;
        private int X_lasera, Y_lasera, Z_lasera, moc_lasera;
        int string_start, string_length;
        private Point poczatek_ukladu_wspolrzednych = new Point(50, 50);
        Bitmap panel;

        public Menu()
        {
            InitializeComponent();
            X_lasera = 50;
            Y_lasera = 50;
            Z_lasera = 0;
            moc_lasera = 0;
            ports = SerialPort.GetPortNames();


            try 
            {
                string port = ports[0];
                port = "" + port[port.Length - 1];

                int.TryParse(port, out com);
                comboBox1.SelectedIndex = com;
            }
            catch(System.IndexOutOfRangeException)
            {

            }
            
            
            
            overlayform = new FormOverlay(this, panel1, groupBox1, richTextBox1);

        }


        private void button1_Click(object sender, EventArgs e)
        {
            overlayform.TopMost = false;
            BazaDanych menuobrazow = new BazaDanych(this, overlayform, richTextBox1);
            menuobrazow.Show();
            this.Enabled = false;
        }


        private void Gcode()
        {
            Regex Gcode = new Regex("[ijngxyzfmsr][+-]?[0-9]*\\.?[0-9]*", RegexOptions.IgnoreCase);
            MatchCollection m;
            bool Xb = false, Yb = false, Zb = false, Fb = false, Gb = false, Ib = false, Jb = false, Nb = false, Mb = false, Sb = false, Rs = false;
            double X = 0.0, Y = 0.0, Z = 0.0, I = 0.0, J = 0.0, F = 0.0, promien = 0.0, OX = 0.0, OY = 0.0, X_pr = 0.0, Y_pr = 0.0, kat_start, kat_rys, X_l = 0.0, Y_l = 0.0, X_pr_l, Y_pr_l, R = 0.0;
            int G = 0, N = 0, M = 0, S = 0, licznik = 0;
            Int32 kat_rys_int, kat_start_int, a, b, w, h;
            int time;

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(gcode_string ?? ""));
            StreamReader reader = new StreamReader(stream);
            string line;

            

            while (true)
            {
                line = reader.ReadLine();
                if (line == null) break;
                m = Gcode.Matches(line);

                foreach (Match n in m)
                {
                    if (n.Value.StartsWith("G"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                            G = Convert.ToInt32(n.Value.Remove(0, 1));
                        //MessageBox.Show("command = " + n.Value);
                    }

                    if (n.Value.StartsWith("X"))
                    {

                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                        {
                            X = Convert.ToDouble(n.Value.Remove(0, 1).Replace('.', ','));
                            Xb = true;
                        }
                        //MessageBox.Show("X= " + n.Value);
                    }

                    if (n.Value.StartsWith("Y"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                        {
                            Y = Convert.ToDouble(n.Value.Remove(0, 1).Replace('.', ','));
                            Yb = true;
                        }
                        //MessageBox.Show("Y= " + n.Value);
                    }

                    if (n.Value.StartsWith("Z"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                        {
                            Z = Convert.ToDouble(n.Value.Remove(0, 1).Replace('.', ','));
                            Zb = true;
                        }
                        //MessageBox.Show("Z= " + n.Value);
                    }

                    if (n.Value.StartsWith("I"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                            I = Convert.ToDouble(n.Value.Remove(0, 1).Replace('.', ','));
                        //MessageBox.Show("I= " + n.Value);
                    }

                    if (n.Value.StartsWith("J"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                            J = Convert.ToDouble(n.Value.Remove(0, 1).Replace('.', ','));
                        //MessageBox.Show("J= " + n.Value);
                    }

                    if (n.Value.StartsWith("M"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                            M = Convert.ToInt32(n.Value.Remove(0, 1).Replace('.', ','));
                        //MessageBox.Show("J= " + n.Value);
                    }

                    if (n.Value.StartsWith("S"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                            S = Convert.ToInt32(n.Value.Remove(0, 1).Replace('.', ','));
                            
                        //MessageBox.Show("J= " + n.Value);
                    }

                    if (n.Value.StartsWith("F"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                            F = Convert.ToDouble(n.Value.Remove(0, 1).Replace('.', ','));
                        //MessageBox.Show("J= " + n.Value);
                    }

                    if (n.Value.StartsWith("R"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                            R = Convert.ToDouble(n.Value.Remove(0, 1).Replace('.', ','));
                        //MessageBox.Show("X= " + n.Value);
                    }
                }

                if (Zb)
                {
                    if (Z == 0.0) S = 255;
                    else if (Z > 0) S = 0;
                    else S = 255;
                }

                licznik++;
                switch (G)
                {
                    case 1:


                        if (S != 0 && Xb && Yb && !Zb)
                        {
                            
                            time = (int)(Math.Sqrt((X - X_pr) * (X - X_pr) + (Y - Y_pr) * (Y - Y_pr)) / F * 60 * 1000);
                            czy_rysowac.Add(true);
                            t.Add(time);
                            opcja.Add(1);
                            X_pr_list.Add((int)X_pr + poczatek_ukladu_wspolrzednych.X);
                            Y_pr_list.Add((int)Y_pr + poczatek_ukladu_wspolrzednych.Y);
                            X_list.Add((int)X + poczatek_ukladu_wspolrzednych.X);
                            Y_list.Add((int)Y + poczatek_ukladu_wspolrzednych.Y);
                            a_list.Add(0);
                            b_list.Add(0);
                            w_list.Add(0);
                            kat_start_list.Add(0);
                            kat_rys_list.Add(0);
                        }
                        else
                        {
                            
                            time = (int)(Math.Sqrt((X - X_pr) * (X - X_pr) + (Y - Y_pr) * (Y - Y_pr)) / F * 60 * 1000);
                            czy_rysowac.Add(false);
                            t.Add(time);
                            opcja.Add(1);
                            X_pr_list.Add((int)X_pr + poczatek_ukladu_wspolrzednych.X);
                            Y_pr_list.Add((int)Y_pr + poczatek_ukladu_wspolrzednych.Y);
                            X_list.Add((int)X + poczatek_ukladu_wspolrzednych.X);
                            Y_list.Add((int)Y + poczatek_ukladu_wspolrzednych.Y);
                            a_list.Add(0);
                            b_list.Add(0);
                            w_list.Add(0);
                            kat_start_list.Add(0);
                            kat_rys_list.Add(0);
                        }
                        X_pr = X;
                        Y_pr = Y;

                        break;

                    case 2:
                        if (true)
                        {
                            OX = X_pr + I;
                            OY = Y_pr + J;
                            X_l = X - OX;
                            Y_l = Y - OY;
                            X_pr_l = X_pr - OX;
                            Y_pr_l = Y_pr - OY;
                            promien = Math.Sqrt(I * I + J * J);

                            kat_rys = Math.Atan2(Y_l, X_l);
                            kat_rys = 180.0 * kat_rys / Math.PI;
                            if (kat_rys >= 0) kat_rys = 360.0 - kat_rys;
                            else kat_rys = -kat_rys;

                            kat_start = Math.Atan2((Y_pr - OY), (X_pr - OX));
                            kat_start = 180.0 * kat_start / Math.PI;
                            if (kat_start >= 0) kat_start = 360.0 - kat_start;
                            else kat_start = -kat_start;

                            if ((cwiartka(X_l, Y_l) == 4 && cwiartka(X_pr_l, Y_pr_l) == 4 && X_pr_l < X_l) || (cwiartka(X_l, Y_l) == 4 && cwiartka(X_pr_l, Y_pr_l) == 3) || (cwiartka(X_l, Y_l) == 3 && cwiartka(X_pr_l, Y_pr_l) == 3 && X_pr_l < X_l) || (cwiartka(X_l, Y_l) == 4 && cwiartka(X_pr_l, Y_pr_l) == 2) || (cwiartka(X_l, Y_l) == 3 && cwiartka(X_pr_l, Y_pr_l) == 2) || (cwiartka(X_l, Y_l) == 2 && cwiartka(X_pr_l, Y_pr_l) == 2 && X_pr_l > X_l) || (cwiartka(X_l, Y_l) == 4 && cwiartka(X_pr_l, Y_pr_l) == 1) || (cwiartka(X_l, Y_l) == 3 && cwiartka(X_pr_l, Y_pr_l) == 1) || (cwiartka(X_l, Y_l) == 2 && cwiartka(X_pr_l, Y_pr_l) == 1) || (cwiartka(X_l, Y_l) == 1 && cwiartka(X_pr_l, Y_pr_l) == 1 && X_pr_l > X_l))
                            {
                                kat_rys = 360.0 - kat_start + kat_rys;
                                if (kat_rys > 360.0) kat_rys = kat_rys - 360.0;
                            }
                            else
                            {
                                kat_rys = kat_rys - kat_start;
                                if (kat_rys > 360.0) kat_rys = kat_rys - 360.0;
                            }

                            kat_start_int = (Int32)kat_start;
                            kat_rys_int = (Int32)kat_rys;

                            

                            a = (Int32)(OX - promien);
                            b = (Int32)(OY + promien);
                            w = (Int32)(2 * promien);

                            if (a < 0 || b < 0 || b > 1000 || promien > 200 || R > 200)
                            {
                                
                                time = (int)(Math.Sqrt((X - X_pr) * (X - X_pr) + (Y - Y_pr) * (Y - Y_pr)) / F * 60 * 1000);
                                opcja.Add(1);
                                czy_rysowac.Add(true);
                                t.Add(time);
                                X_pr_list.Add((int)X_pr + poczatek_ukladu_wspolrzednych.X);
                                Y_pr_list.Add((int)Y_pr + poczatek_ukladu_wspolrzednych.Y);
                                X_list.Add((int)X + poczatek_ukladu_wspolrzednych.X);
                                Y_list.Add((int)Y + poczatek_ukladu_wspolrzednych.Y);
                                a_list.Add(0);
                                b_list.Add(0);
                                w_list.Add(0);
                                kat_start_list.Add(0);
                                kat_rys_list.Add(0);
                            }
                            else if (w != 0 && kat_start_int != kat_rys_int)
                            {
                                
                                time = (int)(2 * Math.PI * promien / F * kat_rys_int / 360.0 * 60 * 1000);
                                czy_rysowac.Add(true);
                                t.Add(time);
                                opcja.Add(2);
                                X_pr_list.Add(0);
                                Y_pr_list.Add(0);
                                X_list.Add(0);
                                Y_list.Add(0);
                                a_list.Add(a + poczatek_ukladu_wspolrzednych.X);
                                b_list.Add(b + poczatek_ukladu_wspolrzednych.Y);
                                w_list.Add(w);
                                kat_start_list.Add(kat_start_int);
                                kat_rys_list.Add(kat_rys_int);

                            }
                            X_pr = X;
                            Y_pr = Y;


                        }
                        break;

                    case 3:
                        if (true)
                        {
                            OX = X_pr + I;
                            OY = Y_pr + J;
                            X_l = X - OX;
                            Y_l = Y - OY;
                            X_pr_l = X_pr - OX;
                            Y_pr_l = Y_pr - OY;
                            promien = Math.Sqrt(I * I + J * J);

                            kat_rys = Math.Atan2(Y_l, X_l);
                            kat_rys = 180.0 * kat_rys / Math.PI;
                            if (kat_rys < 0) kat_rys = 360.0 + kat_rys;
                            

                            kat_start = Math.Atan2((Y_pr - OY), (X_pr - OX));
                            kat_start = 180.0 * kat_start / Math.PI;
                            if (kat_start < 0) kat_start = 360.0 + kat_start;
                            

                            if ((cwiartka(X_l, Y_l) == 1 && cwiartka(X_pr_l, Y_pr_l) == 1 && X_pr_l < X_l) || (cwiartka(X_l, Y_l) == 1 && cwiartka(X_pr_l, Y_pr_l) == 2) || (cwiartka(X_l, Y_l) == 2 && cwiartka(X_pr_l, Y_pr_l) == 2 && X_pr_l < X_l) || (cwiartka(X_l, Y_l) == 1 && cwiartka(X_pr_l, Y_pr_l) == 3) || (cwiartka(X_l, Y_l) == 2 && cwiartka(X_pr_l, Y_pr_l) == 3) || (cwiartka(X_l, Y_l) == 3 && cwiartka(X_pr_l, Y_pr_l) == 3 && X_pr_l > X_l) || (cwiartka(X_l, Y_l) == 1 && cwiartka(X_pr_l, Y_pr_l) == 4) || (cwiartka(X_l, Y_l) == 2 && cwiartka(X_pr_l, Y_pr_l) == 4) || (cwiartka(X_l, Y_l) == 3 && cwiartka(X_pr_l, Y_pr_l) == 4) || (cwiartka(X_l, Y_l) == 4 && cwiartka(X_pr_l, Y_pr_l) == 4 && X_pr_l > X_l))
                            {
                                kat_rys = 360.0 - kat_start + kat_rys;
                                if (kat_rys > 360.0) kat_rys = kat_rys - 360.0;
                            }
                            else
                            {
                                kat_rys = kat_rys - kat_start;
                                if (kat_rys > 360.0) kat_rys = kat_rys - 360.0;
                            }

                            kat_start_int = -(Int32)kat_start;
                            kat_rys_int = -(Int32)kat_rys;
                            a = (Int32)(OX - promien);
                            b = (Int32)(OY + promien);
                            w = (Int32)(2 * promien);

                            if (a < 0 || b < 0 || b > 1000 || promien > 200 || R > 200 || w <= 1)
                            {
                                
                                time = (int)(Math.Sqrt((X - X_pr) * (X - X_pr) + (Y - Y_pr) * (Y - Y_pr)) / F * 60 * 1000);
                                t.Add(time);
                                czy_rysowac.Add(true);
                                opcja.Add(1);
                                X_pr_list.Add((int)X_pr + poczatek_ukladu_wspolrzednych.X);
                                Y_pr_list.Add((int)Y_pr + poczatek_ukladu_wspolrzednych.Y);
                                X_list.Add((int)X + poczatek_ukladu_wspolrzednych.X);
                                Y_list.Add((int)Y + poczatek_ukladu_wspolrzednych.Y);
                                a_list.Add(0);
                                b_list.Add(0);
                                w_list.Add(0);
                                kat_start_list.Add(0);
                                kat_rys_list.Add(0);
                            }
                            else if (w != 0 && kat_start_int != kat_rys_int)
                            {
                                
                                time = (int)(2 * Math.PI * promien / F * (-kat_rys_int) / 360.0 * 60 * 1000);
                                t.Add(time);
                                czy_rysowac.Add(true);
                                opcja.Add(2);
                                X_pr_list.Add(0);
                                Y_pr_list.Add(0);
                                X_list.Add(0);
                                Y_list.Add(0);
                                a_list.Add(a + poczatek_ukladu_wspolrzednych.X);
                                b_list.Add(b + poczatek_ukladu_wspolrzednych.Y);
                                w_list.Add(w);
                                kat_start_list.Add(kat_start_int);
                                kat_rys_list.Add(kat_rys_int);
                            }
                            X_pr = X;
                            Y_pr = Y;


                        }
                        break;
                }
                
                Xb = false; Yb = false; Zb = false; Fb = false; Gb = false; Ib = false; Jb = false; Nb = false; Mb = false; Sb = false; Rs = false;
            }

            gcodeflag = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Machina()
        {
            
            Regex Gcode = new Regex("[ijngxyzfmspr][+-]?[0-9]*\\.?[0-9]*", RegexOptions.IgnoreCase);
            MatchCollection m;
            bool Xb = false, Yb = false, Zb = false, Fb = false, Gb = false, Ib = false, Jb = false, Nb = false, Mb = false, Sb = false, Rb = false, Pb = false;
            double X = 0.0, Y = 0.0, Z = 0.0, I = 0.0, J = 0.0, F = 0.0, promien = 0.0, OX = 0.0, OY = 0.0, X_pr = 0.0, Y_pr = 0.0, kat_start, kat_rys, X_l = 0.0, Y_l = 0.0, X_pr_l, Y_pr_l, R = 0.0;
            int G = 0, N = 0, M = 0, S = 0, licznik = 0, P = 0;
            Int32 kat_rys_int, kat_start_int, a, b, w, h;
            string gcode_inst = "";
            string_start = 0;
            string_length = 0;
            bool startujemy = true;

            Rejestrowanie rejestrowanie_form = new Rejestrowanie();
            rejestrowanie_form.Enabled = false;

            
            rejestrowanie_form.Show();
            rejestrowanie_form.richTextBox1.Text = "Rozpoczynanie rejestrowania";
            //Thread.Sleep(10000);

            //RichTextBox textboxok = new RichTextBox();
            //textboxok.Visible = true;
            //textboxok.Show();
            int NumerWiadomosciZwrotnej = 0;

            //FormOverlay overlayform_machina = overlayform;
            //overlayform_machina.Show();

            overlayform_machina = new FormOverlay(this, panel1, groupBox1, richTextBox1);
            overlayform_machina.Show();

            

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(gcode_string ?? ""));
            StreamReader reader = new StreamReader(stream);
            string line;

            syncObj = new object();
            paused = false;

            if (taskok == null || taskok.IsCompleted)
            {
                //taskok = new Task(() => Statusok());
                //taskok.Start();
            }

            //if (!serialport.IsOpen)
            //{
            //    serialport.Open();
            //}

            //while (!timeout)
            //{
            //    Synchro();
            //}
            //timeout = false;
            //
            //serialport.ReadTimeout = 10000;

            while (true)
            {
                line = reader.ReadLine();
                string_length = line.Length;
                przewin = true;

                overlayform_machina.Location = new Point(groupBox1.Location.X + panel1.Location.X + this.Location.X + 8, groupBox1.Location.Y + panel1.Location.Y + this.Location.Y + 30);

                if (line == null) break;
                m = Gcode.Matches(line);

                

                foreach (Match n in m)
                {

                    String lolz = n.Value.Remove(0, 1).Replace('.', ',');
                    if(lolz != "")
                    {
                        if (gcode_inst == "")
                        {
                            gcode_inst = "" + n;
                        }
                        else
                        {
                            gcode_inst = gcode_inst + " " + n;
                        }
                        
                    }

                    if (n.Value.StartsWith("G"))
                    {
                        
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                        {
                            G = Convert.ToInt32(n.Value.Remove(0, 1));
                            Gb = true;
                            //gcode_inst = ""
                        }
                            
                        //MessageBox.Show("command = " + n.Value);
                    }

                    if (n.Value.StartsWith("P"))
                    {

                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                        {
                            P = Convert.ToInt32(n.Value.Remove(0, 1));
                            Pb = true;
                            //gcode_inst = ""
                        }

                        //MessageBox.Show("command = " + n.Value);
                    }


                    if (n.Value.StartsWith("X"))
                    {

                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                        {
                            X = Convert.ToDouble(n.Value.Remove(0, 1).Replace('.', ','));
                            Xb = true;
                        }
                        //MessageBox.Show("X= " + n.Value);
                    }

                    if (n.Value.StartsWith("Y"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                        {
                            Y = Convert.ToDouble(n.Value.Remove(0, 1).Replace('.', ','));
                            Yb = true;
                        }
                        //MessageBox.Show("Y= " + n.Value);
                    }

                    if (n.Value.StartsWith("Z"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                        {
                            Z = Convert.ToDouble(n.Value.Remove(0, 1).Replace('.', ','));
                            Zb = true;
                        }
                        //MessageBox.Show("Z= " + n.Value);
                    }

                    if (n.Value.StartsWith("I"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                        {
                            I = Convert.ToDouble(n.Value.Remove(0, 1).Replace('.', ','));
                            Ib = true;
                        }
                        //MessageBox.Show("I= " + n.Value);
                    }

                    if (n.Value.StartsWith("J"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                        {
                            J = Convert.ToDouble(n.Value.Remove(0, 1).Replace('.', ','));
                            Jb = true;
                        }
                        //MessageBox.Show("J= " + n.Value);
                    }

                    if (n.Value.StartsWith("M"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                        {
                            M = Convert.ToInt32(n.Value.Remove(0, 1).Replace('.', ','));
                            Mb = true;
                        }
                        //MessageBox.Show("J= " + n.Value);
                    }

                    if (n.Value.StartsWith("S"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                        {
                            S = Convert.ToInt32(n.Value.Remove(0, 1).Replace('.', ','));
                            Sb = true;
                            moc_lasera = S;
                        }
                        //MessageBox.Show("J= " + n.Value);
                    }

                    if (n.Value.StartsWith("F"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                        {
                            F = Convert.ToDouble(n.Value.Remove(0, 1).Replace('.', ','));
                            Fb = true;
                        }
                        //MessageBox.Show("J= " + n.Value);
                    }

                    if (n.Value.StartsWith("R"))
                    {
                        String lol = n.Value.Remove(0, 1).Replace('.', ',');
                        if (lol != "")
                        {
                            R = Convert.ToDouble(n.Value.Remove(0, 1).Replace('.', ','));
                            Rb = true;
                        }
                        //MessageBox.Show("X= " + n.Value);
                    }
                }

                if(gcode_inst != "")
                {
                    //if(!startujemy) Synchro();

                    //timeout = false;
                    //Thread.Sleep(10);
                    //gcode_inst = gcode_inst + '\r';
                    //serialport.Write(gcode_inst);
                }
                startujemy = false;



                if (Zb)
                {
                    if (Z == 0.0) S = 255;
                    else if (Z > 0) S = 0;
                    else S = 255;
                }

                licznik++;
                double t;

                rejestrowanie_form.richTextBox1.Text += "Wysłana instrukcja: " + gcode_inst + "\n" + "Odpowiedź: ";

                switch (G)
                {
                    case 1:


                        X_lasera = (int)X;
                        Y_lasera = (int)Y;

                        overlayform_machina.X_lasera = X_lasera;
                        overlayform_machina.Y_lasera = Y_lasera;
                        overlayform_machina.celownik_flaga = true;
                        overlayform_machina.kolor = 1;
                        overlayform_machina.Refresh();


                        ///////////////////////////////////////////////////
                        //Obliczanie czasu operacji


                      t = Math.Sqrt((X - X_pr) * (X - X_pr) + (Y - Y_pr) * (Y - Y_pr)) / F * 60 * 1000;
                        double dl = X - X_pr, wys = Y - Y_pr;
                        int ilosc_krokow, ostatni_krok;

                        if (S != 0 && Xb && Yb && !Zb)
                        {
                            if (t > 20)
                            {
                                ilosc_krokow = (int)t / 20;
                                ostatni_krok = (int)t % 20;
                                for (int i = 1; i < ilosc_krokow; i++)
                                {
                                    overlayform_machina.residentsliper(20);
                                    X_lasera = (int)X_pr + (int)dl * 20 * i / (int)t;
                                    Y_lasera = (int)Y_pr + (int)wys * 20 * i / (int)t;

                                    overlayform_machina.X_lasera = X_lasera;
                                    overlayform_machina.Y_lasera = Y_lasera;
                                    overlayform_machina.celownik_flaga = true;
                                    overlayform_machina.kolor = 1;
                                    overlayform_machina.Refresh();

                                    if (przerwij_wypalanie)
                                    {
                                        przerwij_wypalanie = false;
                                        po_automatycznym = true;
                                        X_lasera = overlayform_machina.X_lasera;
                                        Y_lasera = overlayform_machina.Y_lasera;
                                        rejestrowanie_form.Close();
                                        overlayform_machina.Close();
                                        return;
                                    }

                                    overlayform_machina.Location = new Point(groupBox1.Location.X + panel1.Location.X + this.Location.X + 8, groupBox1.Location.Y + panel1.Location.Y + this.Location.Y + 30);
                                    
                                }
                                overlayform.residentsliper(ostatni_krok);
                                X_lasera = (int)X;
                                Y_lasera = (int)Y;

                                overlayform_machina.X_lasera = X_lasera;
                                overlayform_machina.Y_lasera = Y_lasera;
                                overlayform_machina.celownik_flaga = true;
                                overlayform_machina.kolor = 1;
                                overlayform_machina.Refresh();
                            }
                            else
                            {
                                overlayform_machina.residentsliper((int)t);
                                X_lasera = (int)X;
                                Y_lasera = (int)Y;

                                overlayform_machina.X_lasera = X_lasera;
                                overlayform_machina.Y_lasera = Y_lasera;
                                overlayform_machina.celownik_flaga = true;
                                overlayform_machina.kolor = 1;
                                overlayform_machina.Refresh();
                            }
                        }
                        else
                        {
                            if (t > 20)
                            {
                                ilosc_krokow = (int)t / 20;
                                ostatni_krok = (int)t % 20;
                                for (int i = 1; i < ilosc_krokow; i++)
                                {
                                    overlayform_machina.residentsliper(20);
                                    X_lasera = (int)X_pr + (int)dl * 20 * i / (int)t;
                                    Y_lasera = (int)Y_pr + (int)wys * 20 * i / (int)t;
                                    overlayform_machina.X_lasera = X_lasera;
                                    overlayform_machina.Y_lasera = Y_lasera;
                                    overlayform_machina.celownik_flaga = true;
                                    overlayform_machina.kolor = 2;
                                    overlayform_machina.Refresh();

                                    if (przerwij_wypalanie)
                                    {
                                        przerwij_wypalanie = false;
                                        po_automatycznym = true;
                                        X_lasera = overlayform_machina.X_lasera;
                                        Y_lasera = overlayform_machina.Y_lasera;
                                        rejestrowanie_form.Close();
                                        overlayform_machina.Close();
                                        return;
                                    }

                                    overlayform_machina.Location = new Point(groupBox1.Location.X + panel1.Location.X + this.Location.X + 8, groupBox1.Location.Y + panel1.Location.Y + this.Location.Y + 30);
                                }
                                overlayform_machina.residentsliper(ostatni_krok);
                                X_lasera = (int)X;
                                Y_lasera = (int)Y;

                                overlayform_machina.X_lasera = X_lasera;
                                overlayform_machina.Y_lasera = Y_lasera;
                                overlayform_machina.celownik_flaga = true;
                                overlayform_machina.kolor = 2;
                                overlayform_machina.Refresh();
                            }
                            else
                            {
                                overlayform_machina.residentsliper((int)t);
                                X_lasera = (int)X;
                                Y_lasera = (int)Y;

                                overlayform_machina.X_lasera = X_lasera;
                                overlayform_machina.Y_lasera = Y_lasera;
                                overlayform_machina.celownik_flaga = true;
                                overlayform_machina.kolor = 2;

                                overlayform_machina.Refresh();

                            }
                        }
                        X_pr = X;
                        Y_pr = Y;
                        break;

                    case 2:
                        if (true)
                        {

                            X_lasera = (int)X;
                            Y_lasera = (int)Y;

                            overlayform_machina.X_lasera = X_lasera;
                            overlayform_machina.Y_lasera = Y_lasera;
                            overlayform_machina.celownik_flaga = true;
                            overlayform_machina.kolor = 1;
                            overlayform_machina.Refresh();


                           //Obliczanie czasu operacji

                            double dl_luku;
                            OX = X_pr + I;
                            OY = Y_pr + J;
                            X_l = X - OX;
                            Y_l = Y - OY;
                            X_pr_l = X_pr - OX;
                            Y_pr_l = Y_pr - OY;
                            promien = Math.Sqrt(I * I + J * J);

                            kat_rys = Math.Atan2(Y_l, X_l);
                            kat_rys = 180.0 * kat_rys / Math.PI;
                            if (kat_rys >= 0) kat_rys = 360.0 - kat_rys;
                            else kat_rys = -kat_rys;

                            kat_start = Math.Atan2((Y_pr - OY), (X_pr - OX));
                            kat_start = 180.0 * kat_start / Math.PI;
                            if (kat_start >= 0) kat_start = 360.0 - kat_start;
                            else kat_start = -kat_start;

                            if ((cwiartka(X_l, Y_l) == 4 && cwiartka(X_pr_l, Y_pr_l) == 4 && X_pr_l < X_l) || (cwiartka(X_l, Y_l) == 4 && cwiartka(X_pr_l, Y_pr_l) == 3) || (cwiartka(X_l, Y_l) == 3 && cwiartka(X_pr_l, Y_pr_l) == 3 && X_pr_l < X_l) || (cwiartka(X_l, Y_l) == 4 && cwiartka(X_pr_l, Y_pr_l) == 2) || (cwiartka(X_l, Y_l) == 3 && cwiartka(X_pr_l, Y_pr_l) == 2) || (cwiartka(X_l, Y_l) == 2 && cwiartka(X_pr_l, Y_pr_l) == 2 && X_pr_l > X_l) || (cwiartka(X_l, Y_l) == 4 && cwiartka(X_pr_l, Y_pr_l) == 1) || (cwiartka(X_l, Y_l) == 3 && cwiartka(X_pr_l, Y_pr_l) == 1) || (cwiartka(X_l, Y_l) == 2 && cwiartka(X_pr_l, Y_pr_l) == 1) || (cwiartka(X_l, Y_l) == 1 && cwiartka(X_pr_l, Y_pr_l) == 1 && X_pr_l > X_l))
                            {
                                kat_rys = 360.0 - kat_start + kat_rys;
                                if (kat_rys > 360.0) kat_rys = kat_rys - 360.0;
                            }
                            else
                            {
                                kat_rys = kat_rys - kat_start;
                                if (kat_rys > 360.0) kat_rys = kat_rys - 360.0;
                            }

                            kat_start_int = (Int32)kat_start;
                            kat_rys_int = (Int32)kat_rys;


                            dl_luku = 2 * Math.PI * promien * kat_rys_int / 360.0;
                            t = 2 * Math.PI * promien / F * kat_rys_int / 360.0 * 60 * 1000;



                            a = (Int32)(OX - promien);
                            b = (Int32)(OY + promien);
                            w = (Int32)(2 * promien);

                            if (a < 0 || b < 0 || b > 1000 || promien > 200 || R > 200)
                            {
                                t = Math.Sqrt((X - X_pr) * (X - X_pr) + (Y - Y_pr) * (Y - Y_pr)) / F * 60 * 1000;
                                dl = X - X_pr;
                                wys = Y - Y_pr;

                                if (S != 0 && Xb && Yb && !Zb)
                                {
                                    if (t > 20)
                                    {
                                        ilosc_krokow = (int)t / 20;
                                        ostatni_krok = (int)t % 20;
                                        for (int i = 1; i < ilosc_krokow; i++)
                                        {
                                            overlayform_machina.residentsliper(20);
                                            X_lasera = (int)X_pr + (int)dl * 20 * i / (int)t;
                                            Y_lasera = (int)Y_pr + (int)wys * 20 * i / (int)t;
                                            overlayform_machina.X_lasera = X_lasera;
                                            overlayform_machina.Y_lasera = Y_lasera;
                                            overlayform_machina.celownik_flaga = true;
                                            overlayform_machina.kolor = 1;
                                            overlayform_machina.Refresh();

                                            if (przerwij_wypalanie)
                                            {
                                                przerwij_wypalanie = false;
                                                po_automatycznym = true;
                                                X_lasera = overlayform_machina.X_lasera;
                                                Y_lasera = overlayform_machina.Y_lasera;
                                                rejestrowanie_form.Close();
                                                overlayform_machina.Close();
                                                return;
                                            }

                                            overlayform_machina.Location = new Point(groupBox1.Location.X + panel1.Location.X + this.Location.X + 8, groupBox1.Location.Y + panel1.Location.Y + this.Location.Y + 30);
                                        }
                                        overlayform_machina.residentsliper(ostatni_krok);
                                        X_lasera = (int)X;
                                        Y_lasera = (int)Y;

                                        overlayform_machina.X_lasera = X_lasera;
                                        overlayform_machina.Y_lasera = Y_lasera;
                                        overlayform_machina.celownik_flaga = true;
                                        overlayform_machina.kolor = 1;
                                        overlayform_machina.Refresh();
                                    }
                                    else
                                    {
                                        overlayform_machina.residentsliper((int)t);
                                        X_lasera = (int)X;
                                        Y_lasera = (int)Y;

                                        overlayform_machina.X_lasera = X_lasera;
                                        overlayform_machina.Y_lasera = Y_lasera;
                                        overlayform_machina.celownik_flaga = true;
                                        overlayform_machina.kolor = 1;
                                        overlayform_machina.Refresh();
                                    }
                                }
                            }
                            else if (w != 0 && kat_start_int != kat_rys_int)
                            {
                                if (t > 20)
                                {
                                    ilosc_krokow = (int)t / 20;
                                    ostatni_krok = (int)t % 20;
                                    double kat;
                                    for (int i = 1; i < ilosc_krokow; i++)
                                    {
                                        kat = i * 20 * 360 * F / (2 * Math.PI * promien * 60 * 1000);

                                        double A_x = X_pr - OX, A_y = Y_pr - OY;
                                        double newpoint_x = Math.Cos(kat / 180 * Math.PI) * A_x + Math.Sin(kat / 180 * Math.PI) * A_y;
                                        double newpoint_y = -Math.Sin(kat / 180 * Math.PI) * A_x + Math.Cos(kat / 180 * Math.PI) * A_y;

                                        Point newpoint = new Point((int)OX + (int)newpoint_x, (int)OY + (int)newpoint_y);

                                        overlayform_machina.residentsliper(20);

                                        X_lasera = newpoint.X;
                                        Y_lasera = newpoint.Y;
                                        overlayform_machina.X_lasera = X_lasera;
                                        overlayform_machina.Y_lasera = Y_lasera;
                                        overlayform_machina.celownik_flaga = true;
                                        overlayform_machina.kolor = 1;
                                        overlayform_machina.Refresh();

                                        if (przerwij_wypalanie)
                                        {
                                            przerwij_wypalanie = false;
                                            po_automatycznym = true;
                                            X_lasera = overlayform_machina.X_lasera;
                                            Y_lasera = overlayform_machina.Y_lasera;
                                            rejestrowanie_form.Close();
                                            overlayform_machina.Close();
                                            return;
                                        }
                                        overlayform_machina.Location = new Point(groupBox1.Location.X + panel1.Location.X + this.Location.X + 8, groupBox1.Location.Y + panel1.Location.Y + this.Location.Y + 30);
                                    }
                                    overlayform_machina.residentsliper(ostatni_krok);
                                    X_lasera = (int)X;
                                    Y_lasera = (int)Y;

                                    overlayform_machina.X_lasera = X_lasera;
                                    overlayform_machina.Y_lasera = Y_lasera;
                                    overlayform_machina.celownik_flaga = true;
                                    overlayform_machina.kolor = 1;
                                    overlayform_machina.Refresh();
                                }
                                else
                                {
                                    overlayform_machina.residentsliper((int)t);
                                    X_lasera = (int)X;
                                    Y_lasera = (int)Y;

                                    overlayform_machina.X_lasera = X_lasera;
                                    overlayform_machina.Y_lasera = Y_lasera;
                                    overlayform_machina.celownik_flaga = true;
                                    overlayform_machina.kolor = 1;
                                    overlayform_machina.Refresh();
                                }
                            }
                            ///////////////////////////////////////////////////
                            
                            

                            X_pr = X;
                            Y_pr = Y;
                        }
                        break;

                    case 3:
                        if (true)
                        {

                            X_lasera = (int)X;
                            Y_lasera = (int)Y;

                            overlayform_machina.X_lasera = X_lasera;
                            overlayform_machina.Y_lasera = Y_lasera;
                            overlayform_machina.celownik_flaga = true;
                            overlayform_machina.kolor = 1;
                            overlayform_machina.Refresh();


                            ///////////////////////////////////////////////////
                            //Obliczanie czasu operacji

                            double dl_luku;
                            OX = X_pr + I;
                            OY = Y_pr + J;
                            X_l = X - OX;
                            Y_l = Y - OY;
                            X_pr_l = X_pr - OX;
                            Y_pr_l = Y_pr - OY;
                            promien = Math.Sqrt(I * I + J * J);

                            kat_rys = Math.Atan2(Y_l, X_l);
                            kat_rys = 180.0 * kat_rys / Math.PI;
                            if (kat_rys < 0) kat_rys = 360.0 + kat_rys;
                            //else kat_rys = -kat_rys;

                            kat_start = Math.Atan2((Y_pr - OY), (X_pr - OX));
                            kat_start = 180.0 * kat_start / Math.PI;
                            if (kat_start < 0) kat_start = 360.0 + kat_start;
                            //else kat_start = -kat_start;

                            if ((cwiartka(X_l, Y_l) == 1 && cwiartka(X_pr_l, Y_pr_l) == 1 && X_pr_l < X_l) || (cwiartka(X_l, Y_l) == 1 && cwiartka(X_pr_l, Y_pr_l) == 2) || (cwiartka(X_l, Y_l) == 2 && cwiartka(X_pr_l, Y_pr_l) == 2 && X_pr_l < X_l) || (cwiartka(X_l, Y_l) == 1 && cwiartka(X_pr_l, Y_pr_l) == 3) || (cwiartka(X_l, Y_l) == 2 && cwiartka(X_pr_l, Y_pr_l) == 3) || (cwiartka(X_l, Y_l) == 3 && cwiartka(X_pr_l, Y_pr_l) == 3 && X_pr_l > X_l) || (cwiartka(X_l, Y_l) == 1 && cwiartka(X_pr_l, Y_pr_l) == 4) || (cwiartka(X_l, Y_l) == 2 && cwiartka(X_pr_l, Y_pr_l) == 4) || (cwiartka(X_l, Y_l) == 3 && cwiartka(X_pr_l, Y_pr_l) == 4) || (cwiartka(X_l, Y_l) == 4 && cwiartka(X_pr_l, Y_pr_l) == 4 && X_pr_l > X_l))
                            {
                                kat_rys = 360.0 - kat_start + kat_rys;
                                if (kat_rys > 360.0) kat_rys = kat_rys - 360.0;
                            }
                            else
                            {
                                kat_rys = kat_rys - kat_start;
                                if (kat_rys > 360.0) kat_rys = kat_rys - 360.0;
                            }

                            kat_start_int = -(Int32)kat_start;
                            kat_rys_int = -(Int32)kat_rys;
                            //kat_rys = -kat_rys;


                            dl_luku = 2 * Math.PI * promien * (-kat_rys_int) / 360.0;
                            t = 2 * Math.PI * promien / F * (-kat_rys_int) / 360.0 * 60 * 1000;



                            a = (Int32)(OX - promien);
                            b = (Int32)(OY + promien);
                            w = (Int32)(2 * promien);

                            if (a < 0 || b < 0 || b > 1000 || promien > 200 || R > 200 || w <= 1)
                            {
                                t = Math.Sqrt((X - X_pr) * (X - X_pr) + (Y - Y_pr) * (Y - Y_pr)) / F * 60 * 1000;
                                dl = X - X_pr;
                                wys = Y - Y_pr;

                                if (t > 20)
                                {
                                    ilosc_krokow = (int)t / 20;
                                    ostatni_krok = (int)t % 20;
                                    for (int i = 1; i < ilosc_krokow; i++)
                                    {
                                        overlayform_machina.residentsliper(20);
                                        X_lasera = (int)X_pr + (int)dl * 20 * i / (int)t;
                                        Y_lasera = (int)Y_pr + (int)wys * 20 * i / (int)t;
                                        overlayform_machina.X_lasera = X_lasera;
                                        overlayform_machina.Y_lasera = Y_lasera;
                                        overlayform_machina.celownik_flaga = true;
                                        overlayform_machina.kolor = 1;
                                        overlayform_machina.Refresh();

                                        if (przerwij_wypalanie)
                                        {
                                            przerwij_wypalanie = false;
                                            po_automatycznym = true;
                                            X_lasera = overlayform_machina.X_lasera;
                                            Y_lasera = overlayform_machina.Y_lasera;
                                            rejestrowanie_form.Close();
                                            overlayform_machina.Close();
                                            return;
                                        }

                                        overlayform_machina.Location = new Point(groupBox1.Location.X + panel1.Location.X + this.Location.X + 8, groupBox1.Location.Y + panel1.Location.Y + this.Location.Y + 30);
                                    }
                                    overlayform_machina.residentsliper(ostatni_krok);
                                    X_lasera = (int)X;
                                    Y_lasera = (int)Y;

                                    overlayform_machina.X_lasera = X_lasera;
                                    overlayform_machina.Y_lasera = Y_lasera;
                                    overlayform_machina.celownik_flaga = true;
                                    overlayform_machina.kolor = 1;
                                    overlayform_machina.Refresh();
                                }
                                else
                                {
                                    overlayform_machina.residentsliper((int)t);
                                    X_lasera = (int)X;
                                    Y_lasera = (int)Y;

                                    overlayform_machina.X_lasera = X_lasera;
                                    overlayform_machina.Y_lasera = Y_lasera;
                                    overlayform_machina.celownik_flaga = true;
                                    overlayform_machina.kolor = 1;
                                    overlayform_machina.Refresh();
                                }

                            }
                            else if (w != 0 && kat_start_int != kat_rys_int)
                            {
                                if (t > 20)
                                {
                                    ilosc_krokow = (int)t / 20;
                                    ostatni_krok = (int)t % 20;
                                    double kat;
                                    for (int i = 1; i < ilosc_krokow; i++)
                                    {
                                        kat = -i * 20 * 360 * F / (2 * Math.PI * promien * 60 * 1000);

                                        double A_x = X_pr - OX, A_y = Y_pr - OY;
                                        double newpoint_x = Math.Cos(kat / 180 * Math.PI) * A_x + Math.Sin(kat / 180 * Math.PI) * A_y;
                                        double newpoint_y = -Math.Sin(kat / 180 * Math.PI) * A_x + Math.Cos(kat / 180 * Math.PI) * A_y;

                                        Point newpoint = new Point((int)OX + (int)newpoint_x, (int)OY + (int)newpoint_y);

                                        overlayform_machina.residentsliper(20);
                                        X_lasera = newpoint.X;
                                        Y_lasera = newpoint.Y;
                                        overlayform_machina.X_lasera = X_lasera;
                                        overlayform_machina.Y_lasera = Y_lasera;
                                        overlayform_machina.celownik_flaga = true;
                                        overlayform_machina.kolor = 1;
                                        overlayform_machina.Refresh();

                                        if (przerwij_wypalanie)
                                        {
                                            przerwij_wypalanie = false;
                                            po_automatycznym = true;
                                            X_lasera = overlayform_machina.X_lasera;
                                            Y_lasera = overlayform_machina.Y_lasera;
                                            rejestrowanie_form.Close();
                                            overlayform_machina.Close();
                                            return;
                                        }
                                        overlayform_machina.Location = new Point(groupBox1.Location.X + panel1.Location.X + this.Location.X + 8, groupBox1.Location.Y + panel1.Location.Y + this.Location.Y + 30);
                                    }
                                    overlayform_machina.residentsliper(ostatni_krok);
                                    X_lasera = (int)X;
                                    Y_lasera = (int)Y;

                                    overlayform_machina.X_lasera = X_lasera;
                                    overlayform_machina.Y_lasera = Y_lasera;
                                    overlayform_machina.celownik_flaga = true;
                                    overlayform_machina.kolor = 1;
                                    overlayform_machina.Refresh();
                                }
                                else
                                {
                                    overlayform_machina.residentsliper((int)t);
                                    X_lasera = (int)X;
                                    Y_lasera = (int)Y;

                                    overlayform_machina.X_lasera = X_lasera;
                                    overlayform_machina.Y_lasera = Y_lasera;
                                    overlayform_machina.celownik_flaga = true;
                                    overlayform_machina.kolor = 1;
                                    overlayform_machina.Refresh();
                                }
                            }
                            ///////////////////////////////////////////////////
                            ///

                            X_pr = X;
                            Y_pr = Y;


                        }
                        break;
                        
                        
                }

                NumerWiadomosciZwrotnej++;
                rejestrowanie_form.richTextBox1.Text += "OK " + NumerWiadomosciZwrotnej + "\n\n";
                //textboxok.Text += "OK" + NumerWiadomosciZwrotnej + "\n";

                Xb = false; Yb = false; Zb = false; Fb = false; Gb = false; Ib = false; Jb = false; Nb = false; Mb = false; Sb = false; Rb = false; Pb = false;
                if (przerwij_wypalanie)
                {
                    przerwij_wypalanie = false;
                    po_automatycznym = true;

                    overlayform_machina.X_lasera = X_lasera;
                    overlayform_machina.Y_lasera = Y_lasera;
                    overlayform_machina.celownik_flaga = true;
                    overlayform_machina.kolor = 2;
                    overlayform_machina.Refresh();

                    X_lasera += 50;
                    Y_lasera += 50;

                    //serialport.ReadTimeout = 200;

                    //if (!serialport.IsOpen) serialport.Open();
                    //string gcode_line = "G92 " + "X" + overlayform.X_lasera + " Y" + overlayform.Y_lasera;
                    //serialport.Write(gcode_line + '\r');
                    //while (!timeout)
                    //{
                    //    Synchro();
                    //}
                    //timeout = false;
                    //status_exit = true;
                    //Thread.Sleep(10);
                    //Synchro();
                    //taskok.Wait();
                    //status_exit = false;
                    rejestrowanie_form.Close();
                    overlayform_machina.Close();
                    return;
                }
                Xb = false; Yb = false; Zb = false;

                gcode_inst = "";
                
            }
            //status_exit = true;
            //Thread.Sleep(20);
            //Synchro();
            //taskok.Wait();
            //status_exit = false;
            po_automatycznym = true;
            rejestrowanie_form.Close();

        }



        private int cwiartka(double x, double y)
        {
            if (x > 0 && y >= 0) return 1;
            else if (x <= 0 && y > 0) return 2;
            else if (x < 0 && y <= 0) return 3;
            else if (x >= 0 && y < 0) return 4;
            else return 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.ForeColor = Color.Red;
            panel = new Bitmap(panel1.Width, panel1.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            X_pr_list.Clear();
            Y_pr_list.Clear();
            X_list.Clear();
            Y_list.Clear();
            a_list.Clear();
            b_list.Clear();
            w_list.Clear();
            kat_start_list.Clear();
            kat_rys_list.Clear();
            opcja.Clear();
            czy_rysowac.Clear();
            t.Clear();
            taskA = new Task(() => Gcode());
            taskA.Start();
            task = true;

        }

        private void Menu_Paint(object sender, PaintEventArgs e)
        {
            if (task)
            {
                if (taskA.IsCompleted)
                {
                    taskC = new Task(() => rysowanko());
                    taskC.Start();
                    taskB_bool = true;
                    task = false;
                }
                
            }

            if (taskB_bool)
            {
                if (taskC != null && taskC.IsCompleted)
                {
                    button2.ForeColor = Color.Black;
                    taskC.Dispose();
                    taskB_bool = false;
                }
            }

            if(po_automatycznym)
            {
                po_automatycznym = false;
                label39.Text = "OCZEKIWANIE";
                label39.ForeColor = Color.YellowGreen;
                label39.Refresh();
            }

        }


        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            gcode_string = File.ReadAllText(openFileDialog1.FileName);
            richTextBox1.Text = gcode_string;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            overlayform.Show();
            overlayform.X_lasera = X_lasera-50;
            overlayform.Y_lasera = Y_lasera-50;
            overlayform.celownik_flaga = true;
            overlayform.kolor = 2;
            overlayform.Refresh();
            panel = new Bitmap(panel1.Width, panel1.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }


        private void button4_Click(object sender, EventArgs e)
        {
            X_pr_list.Clear();
            Y_pr_list.Clear();
            X_list.Clear();
            Y_list.Clear();
            a_list.Clear();
            b_list.Clear();
            w_list.Clear();
            kat_start_list.Clear();
            kat_rys_list.Clear();
            opcja.Clear();
            czy_rysowac.Clear();
            t.Clear();
            panel.Dispose();
            panel = new Bitmap(panel1.Width, panel1.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            panel1.Refresh();

        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        { 
        }

        private void Menu_LocationChanged(object sender, EventArgs e)
        {
            overlayform.Location = new Point(groupBox1.Location.X + panel1.Location.X + this.Location.X + 8, groupBox1.Location.Y + panel1.Location.Y + this.Location.Y + 30);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            label39.Text = "STEROWANIE AUTOMATYCZNE";
            label39.ForeColor = Color.MediumAquamarine;
            label39.Refresh();
            taskB = new Task(() => Machina());
            taskB.Start();   
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

            e.Graphics.DrawImage(panel, 0, 0, panel.Width, panel.Height);

            Pen myPen = new Pen(Color.Black, 2);
            e.Graphics.DrawLine(myPen, 50, panel1.Height - 50, 50, 50);
            for (int i = 1; i < 30; ++i)
            {
                e.Graphics.DrawLine(myPen, 45, panel1.Height - 50 - i * 2 * 10, 55, panel1.Height - 50 - i * 2 * 10);
            }
            e.Graphics.DrawLine(myPen, 50, 50, 40, 60);
            e.Graphics.DrawLine(myPen, 50, 50, 60, 60);

            for (int i = 1; i < 36; ++i)
            {
                e.Graphics.DrawLine(myPen, 50 + i * 2 * 10, panel1.Height - 45, 50 + i * 2 * 10, panel1.Height - 55);
            }

            e.Graphics.DrawLine(myPen, panel1.Width - 50, panel1.Height - 50, panel1.Width - 60, panel1.Height - 40);
            e.Graphics.DrawLine(myPen, panel1.Width - 50, panel1.Height - 50, panel1.Width - 60, panel1.Height - 60);

            e.Graphics.DrawLine(myPen, 50, panel1.Height - 50, panel1.Width - 50, panel1.Height - 50);
            myPen.Dispose();

        }

       
        private void rysowanko()
        {
            if (gcodeflag)
            {
                Pen myPen;
                Graphics g = Graphics.FromImage(panel);
                int i = 0;
                foreach (int n in opcja)
                {
                    if (czy_rysowac[i])
                    {
                        if (n == 1)
                        {
                            myPen = new Pen(Color.Red, 1);
                            g.DrawLine(myPen, X_pr_list[i], panel1.Height - Y_pr_list[i], X_list[i], panel1.Height - Y_list[i]);
                            myPen.Dispose();
                        }
                        else if (n == 2)
                        {
                            myPen = new Pen(Color.Red, 1);
                            g.DrawArc(myPen, a_list[i], panel1.Height - b_list[i], w_list[i], w_list[i], kat_start_list[i], kat_rys_list[i]);
                            myPen.Dispose();
                        }
                        else
                        {

                        }
                    }
                    i++;
                }
                panel1.Invalidate();
                gcodeflag = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (taskB == null || taskB.IsCompleted)
            {
                syncObj = new object();
                paused = false;

                

                //serialport.ReadTimeout = 200;

                //if (taskok == null || taskok.IsCompleted)
                //{
                //    taskok = new Task(() => Statusok());
                //    taskok.Start();
                //}

                int skok;
                if (!int.TryParse(textBox1.Text, out skok))
                {
                    MessageBox.Show("Zły format");
                    return;
                }

                double F;

                if (!double.TryParse(textBox2.Text, out F))
                {
                    MessageBox.Show("Zły format");
                    return;
                }

                label39.Text = "STEROWANIE RĘCZNE";
                label39.ForeColor = Color.Blue;
                label39.Refresh();

                double t, X_pr = X_lasera, Y_pr = Y_lasera, X = X_pr + skok;
                t = Math.Sqrt((double)skok * (double)skok) / F * 60.0 * 1000.0;
                double dl = (double)skok;
                int ilosc_krokow, ostatni_krok;



                //if(!serialport.IsOpen)
                //    serialport.Open();

                //string gcode_line;
                //char[] ok = new char[12];

                //gcode_line = "M05 S0";
                //serialport.Write(gcode_line + '\r');


                //gcode_line = "G21";
                //serialport.Write(gcode_line + '\r');

                //gcode_line = "G90";
                //serialport.Write(gcode_line + '\r');

                //gcode_line = "G1 " + "F" + F;
                //serialport.Write(gcode_line + '\r');

                //skok = overlayform.X_lasera + skok;

                //gcode_line = "G1 " + "X" + skok;
                //serialport.Write(gcode_line + '\r');

                //gcode_line = "M05 S0";
                //serialport.Write(gcode_line + '\r');

                if (t > 20)
                {
                    ilosc_krokow = (int)t / 20;
                    ostatni_krok = (int)t % 20;
                    for (int i = 1; i < ilosc_krokow; i++)
                    {
                        overlayform.residentsliper(20);
                        X_lasera = (int)X_pr + (int)dl * 20 * i / (int)t;
                        overlayform.X_lasera = X_lasera - 50;
                        overlayform.Y_lasera = Y_lasera - 50;
                        overlayform.celownik_flaga = true;
                        overlayform.kolor = 2;
                        overlayform.Refresh();
                    }
                    overlayform.residentsliper(ostatni_krok);
                    X_lasera = (int)X;

                    overlayform.X_lasera = X_lasera - 50;
                    overlayform.Y_lasera = Y_lasera - 50;
                    overlayform.celownik_flaga = true;
                    overlayform.kolor = 2;
                    overlayform.Refresh();
                }
                else
                {
                    overlayform.residentsliper((int)t);
                    X_lasera = (int)X;

                    overlayform.X_lasera = X_lasera - 50;
                    overlayform.Y_lasera = Y_lasera - 50;
                    overlayform.celownik_flaga = true;
                    overlayform.kolor = 2;
                    overlayform.Refresh();
                }

                //while (!timeout)
                //{
                //    Synchro();
                //}
                //timeout = false;
                //status_exit = true;
                //Thread.Sleep(10);
                //Synchro();
                //taskok.Wait();
                //status_exit = false;
                //paused = false;

                label39.Text = "OCZEKIWANIE";
                label39.ForeColor = Color.YellowGreen;
                label39.Refresh();

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            
            if (taskB == null || taskB.IsCompleted)
            {
                syncObj = new object();
                //paused = false;
                //serialport.ReadTimeout = 200;

                //if (taskok == null || taskok.IsCompleted)
                //{
                //    taskok = new Task(() => Statusok());
                //    taskok.Start();
                //}

                

                int skok;
                if (!int.TryParse(textBox1.Text, out skok))
                {
                    MessageBox.Show("Zły format");
                    return;
                }

                double F;

                if (!double.TryParse(textBox2.Text, out F))
                {
                    MessageBox.Show("Zły format");
                    return;
                }

                label39.Text = "STEROWANIE RĘCZNE";
                label39.ForeColor = Color.Blue;
                label39.Refresh();


                double t, X_pr = X_lasera, Y_pr = Y_lasera, Y = Y_pr + skok;
                t = Math.Sqrt((double)skok * (double)skok) / F * 60.0 * 1000.0;
                double wys = (double)skok;
                int ilosc_krokow, ostatni_krok;

                //if (!serialport.IsOpen)
                //    serialport.Open();

                /*
                string gcode_line;

                gcode_line = "M05 S0";
                serialport.Write(gcode_line + '\r');

                gcode_line = "G21";
                serialport.Write(gcode_line + '\r');

                gcode_line = "G90";
                serialport.Write(gcode_line + '\r');

                gcode_line = "G1 " + "F" + F;
                serialport.Write(gcode_line + '\r');

                skok = overlayform.Y_lasera + skok;

                gcode_line = "G1 " + "Y" + skok;
                serialport.Write(gcode_line + '\r');

                gcode_line = "M05 S0";
                serialport.Write(gcode_line + '\r');
                */
                

                if (t > 20)
                {
                    ilosc_krokow = (int)t / 20;
                    ostatni_krok = (int)t % 20;
                    for (int i = 1; i < ilosc_krokow; i++)
                    {
                        overlayform.residentsliper(20);
                        Y_lasera = (int)Y_pr + (int)wys * 20 * i / (int)t;
                        overlayform.X_lasera = X_lasera-50;
                        overlayform.Y_lasera = Y_lasera-50;
                        overlayform.celownik_flaga = true;
                        overlayform.kolor = 2;
                        overlayform.Refresh();
                    }
                    overlayform.residentsliper(ostatni_krok);
                    Y_lasera = (int)Y;

                    overlayform.X_lasera = X_lasera-50;
                    overlayform.Y_lasera = Y_lasera-50;
                    overlayform.celownik_flaga = true;
                    overlayform.kolor = 2;
                    overlayform.Refresh();
                }
                else
                {
                    overlayform.residentsliper((int)t);
                    Y_lasera = (int)Y;

                    overlayform.X_lasera = X_lasera-50;
                    overlayform.Y_lasera = Y_lasera-50;
                    overlayform.celownik_flaga = true;
                    overlayform.kolor = 2;
                    overlayform.Refresh();
                }

                //while (!timeout)
                //{
                //    Synchro();
                //}
                //timeout = false;
                //status_exit = true;
                //Thread.Sleep(10);
                //Synchro();
                //taskok.Wait();
                //status_exit = false;

                label39.Text = "OCZEKIWANIE";
                label39.ForeColor = Color.YellowGreen;
                label39.Refresh();

            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (taskB == null || taskB.IsCompleted)
            {
                syncObj = new object();

                

                //paused = false;
                //serialport.ReadTimeout = 200;

                //if (taskok == null || taskok.IsCompleted)
                //{
                //    taskok = new Task(() => Statusok());
                //    taskok.Start();
                //}

                int skok;
                if (!int.TryParse(textBox1.Text, out skok))
                {
                    MessageBox.Show("Zły format");
                    return;
                }

                double F;

                if (!double.TryParse(textBox2.Text, out F))
                {
                    MessageBox.Show("Zły format");
                    return;
                }

                label39.Text = "STEROWANIE RĘCZNE";
                label39.ForeColor = Color.Blue;
                label39.Refresh();


                double t, X_pr = X_lasera, Y_pr = Y_lasera, Y = Y_pr - skok;
                t = Math.Sqrt((double)skok * (double)skok) / F * 60.0 * 1000.0;
                double wys = -(double)skok;
                int ilosc_krokow, ostatni_krok;

                //if (!serialport.IsOpen)
                //    serialport.Open();

                //string gcode_line;

                //gcode_line = "M05 S0";
                //serialport.Write(gcode_line + '\r');

                //gcode_line = "G21";
                //serialport.Write(gcode_line + '\r');

                //gcode_line = "G90";
                //serialport.Write(gcode_line + '\r');

                //gcode_line = "G1 " + "F" + F;
                //serialport.Write(gcode_line + '\r');

                //skok = overlayform.Y_lasera - skok;

                //gcode_line = "G1 " + "Y" + skok;
                //serialport.Write(gcode_line + '\r');

                //gcode_line = "M05 S0";
                //serialport.Write(gcode_line + '\r');


                if (t > 20)
                {
                    ilosc_krokow = (int)t / 20;
                    ostatni_krok = (int)t % 20;
                    for (int i = 1; i < ilosc_krokow; i++)
                    {
                        overlayform.residentsliper(20);
                        Y_lasera = (int)Y_pr + (int)wys * 20 * i / (int)t;
                        overlayform.X_lasera = X_lasera-50;
                        overlayform.Y_lasera = Y_lasera-50;
                        overlayform.celownik_flaga = true;
                        overlayform.kolor = 2;
                        overlayform.Refresh();
                    }
                    overlayform.residentsliper(ostatni_krok);
                    Y_lasera = (int)Y;

                    overlayform.X_lasera = X_lasera-50;
                    overlayform.Y_lasera = Y_lasera-50;
                    overlayform.celownik_flaga = true;
                    overlayform.kolor = 2;
                    overlayform.Refresh();
                }
                else
                {
                    overlayform.residentsliper((int)t);
                    Y_lasera = (int)Y;

                    overlayform.X_lasera = X_lasera;
                    overlayform.Y_lasera = Y_lasera;
                    overlayform.celownik_flaga = true;
                    overlayform.kolor = 2;
                    overlayform.Refresh();
                }
                //while (!timeout)
                //{
                //    Synchro();
                //}
                //timeout = false;
                //status_exit = true;
                //Thread.Sleep(10);
                //Synchro();
                //taskok.Wait();
                //status_exit = false;

                label39.Text = "OCZEKIWANIE";
                label39.ForeColor = Color.YellowGreen;
                label39.Refresh();

            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (taskB == null || taskB.IsCompleted)
            {
                syncObj = new object();
                //paused = false;
                //serialport.ReadTimeout = 200;

                //if (taskok == null || taskok.IsCompleted)
                //{
                //    taskok = new Task(() => Statusok());
                //    taskok.Start();
                //}

                X_lasera = 50;
                Y_lasera = 50;
                overlayform.X_lasera = 0;
                overlayform.Y_lasera = 0;
                overlayform.celownik_flaga = true;
                overlayform.kolor = 2;
                overlayform.Refresh();

                //if (!serialport.IsOpen) serialport.Open();
                //string gcode_line = "G92 " + "X" + overlayform.X_lasera + " Y" + overlayform.Y_lasera;
                //serialport.Write(gcode_line + '\r');

                //while (!timeout)
                //{
                //    Synchro();
                //}
                //timeout = false;
                //status_exit = true;
                //Thread.Sleep(10);
                //Synchro();
                //taskok.Wait();
                //status_exit = false;
                
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        //private void Synchro()
        //{
        //    Resume();
        //    do_sprawdzenia = true;
        //    while (!mozna_wyslac)
        //    {
        //        if (do_sprawdzenia == false && mozna_wyslac == false) break;
        //        if (status_exit)
        //        { break; }
        //    }
            
        //    do_sprawdzenia = false;
        //    Pause();
        //    mozna_wyslac = false;
            
        //}

        //private void Statusok()
        //{
        //    string ok_string;
        //    while (!status_exit)
        //    {
        //        if (do_sprawdzenia)
        //        {
        //            try
        //            {
        //                //ok_string = serialport.ReadLine();
        //                //zwrotna = ok_string;
        //                //if (ok_string == "ok\r")
        //                //{
        //                //    mozna_wyslac = true;
        //                //
        //                //}
        //            }
        //            catch(System.TimeoutException)
        //            {
        //                timeout = true;
        //                do_sprawdzenia = false;
        //                zwrotna = "ni ma nic";
        //            }
        //        }
                

        //        lock (syncObj) { }
        //    }

            
        //}

        private void Pause()
        {
            if (paused == false)
            {
                
                try
                { Monitor.Enter(syncObj); }
                catch(System.Threading.SynchronizationLockException)
                {
                    
                }
                
                paused = true;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label39_Click(object sender, EventArgs e)
        {

        }

        private void Resume()
        {
            if (paused)
            {
                paused = false;
                
                try
                { Monitor.Exit(syncObj); }
                catch(System.Threading.SynchronizationLockException)
                {
                    
                }
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            //serialport = new SerialPort("COM" + com, 115200, Parity.None, 8, StopBits.One);
            //serialport.ReadTimeout = 3000;
            //if (!serialport.IsOpen)
            //{

            //    string gcode_line;
            //    serialport.Open();

            //    gcode_line = "G92 X0 Y0";
            //    serialport.Write(gcode_line + '\r');
            //    serialport.ReadTimeout = 200;
            //    MessageBox.Show("Połączono!");

            //}

            label39.Text = "POŁĄCZONO";
            label39.ForeColor = Color.Green;
        }

        private void Menu_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            if (serialport != null && serialport.IsOpen)
            {
                serialport.DiscardInBuffer();
                serialport.DiscardOutBuffer();
                serialport.Dispose();
            }

            
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {
            syncObj = new object();
            paused = false;

            //serialport.ReadTimeout = 200;

            //if (taskok == null || taskok.IsCompleted)
            //{
            //    taskok = new Task(() => Statusok());
            //    taskok.Start();
            //}

            int moc;
            if (!int.TryParse(textBox4.Text, out moc))
            {
                MessageBox.Show("Zły format");
                return;
            }
            if(moc<0 || moc>255)
            {
                MessageBox.Show("Wprowadź liczbę w przedziale 0-255");
                return;
            }
            moc_lasera = moc;

           //if(!serialport.IsOpen)
           // {
           //     serialport.Open();
           // }

            //string gcode_line = "M03 S"+moc;
            //serialport.Write(gcode_line + '\r');

            
            if (moc>0)
            {
                overlayform.X_lasera = X_lasera-50;
                overlayform.Y_lasera = Y_lasera-50;
                overlayform.celownik_flaga = true;
                overlayform.kolor = 1;
                overlayform.Refresh();
            }
            else
            {
                overlayform.X_lasera = X_lasera-50;
                overlayform.Y_lasera = Y_lasera-50;
                overlayform.celownik_flaga = true;
                overlayform.kolor = 2;
                overlayform.Refresh();
            }

            //while (!timeout)
            //{
            //    Synchro();
            //}
            //timeout = false;
            //status_exit = true;
            //Thread.Sleep(10);
            //Synchro();
            //taskok.Wait();
            //status_exit = false;


        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            przerwij_wypalanie = true;
           
        }

        private void button13_Click(object sender, EventArgs e)
        {
            syncObj = new object();
            paused = false;

            serialport.ReadTimeout = 200;

            //if (taskok == null || taskok.IsCompleted)
            //{
            //    taskok = new Task(() => Statusok());
            //    taskok.Start();
            //}

            int skok;
            if (!int.TryParse(textBox3.Text, out skok))
            {
                MessageBox.Show("Zły format");
                return;
            }

            double F;

            if (!double.TryParse(textBox2.Text, out F))
            {
                MessageBox.Show("Zły format");
                return;
            }

            //if (!serialport.IsOpen)
            //    serialport.Open();

            string gcode_line;


            gcode_line = "M03 S0";
            serialport.Write(gcode_line + '\r');

            gcode_line = "G21";
            serialport.Write(gcode_line + '\r');

            gcode_line = "G90";
            serialport.Write(gcode_line + '\r');

            gcode_line = "G1 " + "F" + F;
            serialport.Write(gcode_line + '\r');

            gcode_line = "G1 " + "Z" + (Z_lasera - skok);
            serialport.Write(gcode_line + '\r');

            gcode_line = "M03 S0";
            serialport.Write(gcode_line + '\r');

            //while (!timeout)
            //{
            //    Synchro();
            //}
            //timeout = false;
            //status_exit = true;
            //Thread.Sleep(10);
            //Synchro();
            //taskok.Wait();
            //status_exit = false;

            Z_lasera -= skok;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            syncObj = new object();
            paused = false;

            //serialport.ReadTimeout = 200;

            //if (taskok == null || taskok.IsCompleted)
            //{
            //    taskok = new Task(() => Statusok());
            //    taskok.Start();
            //}

            int skok;
            if (!int.TryParse(textBox3.Text, out skok))
            {
                MessageBox.Show("Zły format");
                return;
            }

            double F;

            if (!double.TryParse(textBox2.Text, out F))
            {
                MessageBox.Show("Zły format");
                return;
            }

            //if (!serialport.IsOpen)
            //    serialport.Open();

            string gcode_line;


            gcode_line = "M03 S0";
            serialport.Write(gcode_line + '\r');

            gcode_line = "G21";
            serialport.Write(gcode_line + '\r');

            gcode_line = "G90";
            serialport.Write(gcode_line + '\r');

            gcode_line = "G1 " + "F" + F;
            serialport.Write(gcode_line + '\r');

            gcode_line = "G1 " + "Z" + (Z_lasera + skok);
            serialport.Write(gcode_line + '\r');

            gcode_line = "M03 S0";
            serialport.Write(gcode_line + '\r');

            //while (!timeout)
            //{
            //    Synchro();
            //}
            //timeout = false;
            //status_exit = true;
            //Thread.Sleep(10);
            //Synchro();
            //taskok.Wait();
            //status_exit = false;

            Z_lasera += skok;
        }

        private void Menu_Resize(object sender, EventArgs e)
        {

        }

        private void Menu_Deactivate(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (taskB == null || taskB.IsCompleted)
            {
                syncObj = new object();
                paused = false;
                //serialport.ReadTimeout = 200;

                //if (taskok == null || taskok.IsCompleted)
                //{
                //    taskok = new Task(() => Statusok());
                //    taskok.Start();
                //}

                

                int skok;
                if (!int.TryParse(textBox1.Text, out skok))
                {
                    MessageBox.Show("Zły format");
                    return;
                }

                double F;

                if (!double.TryParse(textBox2.Text, out F))
                {
                    MessageBox.Show("Zły format");
                    return;
                }

                label39.Text = "STEROWANIE RĘCZNE";
                label39.ForeColor = Color.Blue;
                label39.Refresh();


                double t, X_pr = X_lasera, Y_pr = Y_lasera, X = X_pr - skok;
                t = Math.Sqrt((double)skok * (double)skok) / F * 60.0 * 1000.0;
                double dl = -(double)skok;
                int ilosc_krokow, ostatni_krok;

                //if (!serialport.IsOpen)
                //    serialport.Open();

                //string gcode_line;

                //gcode_line = "M05 S0";
                //serialport.Write(gcode_line + '\r');

                //gcode_line = "G21";
                //serialport.Write(gcode_line + '\r');

                //gcode_line = "G90";
                //serialport.Write(gcode_line + '\r');

                //gcode_line = "G1 " + "F" + F;
                //serialport.Write(gcode_line + '\r');

                //skok = overlayform.X_lasera - skok;

                //gcode_line = "G1 " + "X" + skok;
                //serialport.Write(gcode_line + '\r');

                //gcode_line = "M05 S0";
                //serialport.Write(gcode_line + '\r');

                if (t > 20)
                {
                    ilosc_krokow = (int)t / 20;
                    ostatni_krok = (int)t % 20;
                    for (int i = 1; i < ilosc_krokow; i++)
                    {
                        overlayform.residentsliper(20);
                        X_lasera = (int)X_pr + (int)dl * 20 * i / (int)t;
                        overlayform.X_lasera = X_lasera-50;
                        overlayform.Y_lasera = Y_lasera-50;
                        overlayform.celownik_flaga = true;
                        overlayform.kolor = 2;
                        overlayform.Refresh();
                    }
                    overlayform.residentsliper(ostatni_krok);
                    X_lasera = (int)X;

                    overlayform.X_lasera = X_lasera-50;
                    overlayform.Y_lasera = Y_lasera-50;
                    overlayform.celownik_flaga = true;
                    overlayform.kolor = 2;
                    overlayform.Refresh();
                }
                else
                {
                    overlayform.residentsliper((int)t);
                    X_lasera = (int)X;

                    overlayform.X_lasera = X_lasera-50;
                    overlayform.Y_lasera = Y_lasera-50;
                    overlayform.celownik_flaga = true;
                    overlayform.kolor = 2;
                    overlayform.Refresh();
                }

                //while (!timeout)
                //{
                //    Synchro();
                //}
                //timeout = false;
                //status_exit = true;
                //Thread.Sleep(10);
                //Synchro();
                //taskok.Wait();
                //status_exit = false;

                label39.Text = "OCZEKIWANIE";
                label39.ForeColor = Color.YellowGreen;
                label39.Refresh();
            }
            
        }
    }
}
