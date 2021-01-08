using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace Potezny_projekt
{
    public partial class FormOverlay : Form
    {
        Panel panel1;
        GroupBox groupBox1;
        Menu menu;
        RichTextBox richTextBox1;
        public int X_lasera = 0, Y_lasera = 0;
        public bool celownik_flaga = false;
        public int kolor;

        public FormOverlay(Menu frm, Panel panel, GroupBox groupbox, RichTextBox textbox)
        {
            menu = frm;
            panel1 = panel;
            groupBox1 = groupbox;
            richTextBox1 = textbox;
            InitializeComponent();
        }

        private void FormOverlay_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = panel1.Size;
            this.Location = new Point(groupBox1.Location.X + panel1.Location.X+menu.Location.X+8, groupBox1.Location.Y + panel1.Location.Y+ menu.Location.Y+30);

        }


        private void FormOverlay_Paint(object sender, PaintEventArgs e)
        {
            if(celownik_flaga)
            {
                Color color;

                if (kolor == 1)
                {
                    color = Color.Blue;
                }
                else if (kolor == 2)
                {
                    color = Color.Green;
                }
                else
                {
                    color = Color.Blue;
                }

                Pen myPen = new Pen(color, 2);
                e.Graphics.DrawLine(myPen,  50 + X_lasera - 5,  panel1.Height - Y_lasera - 50 , X_lasera + 5 + 50, panel1.Height - Y_lasera - 50);
                e.Graphics.DrawLine(myPen,  50 + X_lasera,  panel1.Height - Y_lasera - 5 - 50, X_lasera + 50, panel1.Height - Y_lasera + 5 - 50);
                myPen.Dispose();
                celownik_flaga = false;
            }

        }

        public void residentsliper(int slipper)
        {
            Thread.Sleep(slipper);
        }
    }
}
