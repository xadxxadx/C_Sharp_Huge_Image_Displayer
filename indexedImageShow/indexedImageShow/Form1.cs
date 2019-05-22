using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace indexedImageShow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Bitmap img = new Bitmap(100, 100, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, img.PixelFormat);
            int index = 0;
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                for (int h = 0; h < img.Height; ++h)
                {
                    for (int w = 0; w < img.Width; ++w)
                    {
                        *ptr++ = (byte)(index++ % 256);
                    }
                }
            }
            img.UnlockBits(data);

            ColorPalette cp = img.Palette;
            for (int i = 0; i < 256; ++i)
                cp.Entries[i] = Color.FromArgb(255, i, i, i);
            img.Palette = cp;

            this.pictureBox1.Image = img;


            Bitmap img2 = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(img2))
            {
                g.DrawImage(img, 0, 0);
            }
            this.pictureBox2.Image = img2;
            img2.Save("E:\\test.bmp");
        }
    }
}
