using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Potezny_projekt
{
    public partial class Show_image : Form
    {
        private Bitmap obrazek;
        private Graphics g;
        private bool flaga = false;
        private int wysokosc = 0, szerokosc = 0;

        public Show_image(Bitmap bitmapa)
        {
            InitializeComponent();
            obrazek = bitmapa;
            
        }

        private void Show_image_Load(object sender, EventArgs e)
        {
            int szer = obrazek.Width, wys = obrazek.Height;
            //40 i 60
            if(szer>800||wys>800)
            {
                double proporcja;
                proporcja = szer / wys;
                if (szer >= wys) 
                {
                    double proszer = szer / 800;
                    wys = (int)(wys / proszer);
                    szer = 800;
                }
                else
                {
                    double prowys = wys / 800;
                    szer = (int)(wys / prowys);
                    wys = 800;
                }
                flaga = true;
            }
            else
            {
                flaga = false;
            }

            this.Width = szer+40;
            this.Height = wys+60;
            wysokosc = wys;
            szerokosc = szer;
            panel1.Width = obrazek.Width;
            panel1.Height = obrazek.Height;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(obrazek, 0, 0, szerokosc, wysokosc);
        }
    }
}
