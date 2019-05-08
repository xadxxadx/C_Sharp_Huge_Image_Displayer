using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo
{
    public partial class Form1 : Form
    {
        Point mouse_location;
        public Form1()
        {
            InitializeComponent();
            this.displayControler1.Image = new Bitmap(@"\\192.168.1.120\Data\Pictures\20180102_新陽\白點\c2.bmp");
            this.displayControler1.Label = new Bitmap(@"\\192.168.1.120\Data\Pictures\20180102_新陽\白點\c3.bmp");
            this.displayControler1.ImageMouseMove += DisplayControler1_ImageMouseMove;
            this.displayControler1.ImageOnPaint += DisplayControler1_ImageOnPaint;
        }

        private void DisplayControler1_ImageOnPaint(PaintEventArgs obj)
        {
            obj.Graphics.DrawEllipse(Pens.Red, this.mouse_location.X, this.mouse_location.Y, 5, 5);
            Console.WriteLine(this.mouse_location.ToString());
        }

        private void DisplayControler1_ImageMouseMove(object arg1, MouseEventArgs arg2, Point pixelLoc)
        {
            mouse_location = arg2.Location;
            this.displayControler1.Invalidate();
            //Console.WriteLine(pixelLoc.ToString());
        }
    }
}
