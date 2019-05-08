using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Demo
{
    public partial class DisplayControler : UserControl
    {
        private Point _start_point = new Point(0, 0);
        private float _scale = 1.0f;
        private Image _image = null;
        private Image _label = null;
        private Point _mouse_down_location = Point.Empty;
        private Point _mouse_down_start_point = Point.Empty;
        private float _max_scale = 32f;
        private float _min_scale = 1f / 32;

        public event Action<object, MouseEventArgs, Point> ImageMouseMove;
        public event Action<PaintEventArgs> ImageOnPaint;

        public Image Image
        {
            get { return this._image; }
            set
            {
                lock(this)
                {
                    this._image = value;
                }
            }
        }

        public Image Label
        {
            get { return this._label; }
            set
            {
                lock (this)
                {
                    this._label = value;
                }
            }
        }

        public DisplayControler()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.MouseMove += DisplayControler_MouseMove;
            this.MouseDown += DisplayControler_MouseDown;
            this.MouseWheel += DisplayControler_MouseWheel;
        }

        private void DisplayControler_MouseWheel(object sender, MouseEventArgs e)
        {
            if(e.Delta > 0)
            {
                if (this._scale < this._max_scale)
                {
                    this._scale *= 2;
                    this._start_point = PointMult(this._start_point, 1f/2);
                }
            }
            else
            {
                if (this._scale > this._min_scale)
                {
                    this._scale /=  2;
                    this._start_point = PointMult(this._start_point, 2f);
                }
            }
            this.Refresh();
        }

        private Point PointAdd(Point a, Point b) { return new Point(a.X + b.X, a.Y + b.Y); }
        private Point PointSub(Point a, Point b) { return new Point(a.X - b.X, a.Y - b.Y); }
        private Point PointMult(Point a, float scale) { return new Point((int)(a.X * scale), (int)(a.Y * scale)); }

        private void DisplayControler_MouseDown(object sender, MouseEventArgs e)
        {
            this._mouse_down_location = e.Location;
            this._mouse_down_start_point = this._start_point;
        }

        private void DisplayControler_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._mouse_down_location != Point.Empty)
            {
                if (e.Button != MouseButtons.Left)
                {
                    this._mouse_down_location = Point.Empty;
                }
                else
                {
                    this._start_point = PointAdd(this._mouse_down_start_point, PointMult(PointSub(e.Location, this._mouse_down_location), 1 / this._scale));
                }
                this.Invalidate();
            }
            if (this.ImageMouseMove != null)
            {
                this.ImageMouseMove(this, e, new Point((int)((e.X - this._start_point.X * this._scale) / this._scale), (int)((e.Y - this._start_point.Y * this._scale) / this._scale)));
            }
        }

        private void ClipStartPoint()
        {
            Size img_size = this.Image.Size;
            int max_x = -(int)((img_size.Width * this._scale - this.Width) / this._scale);
            int max_y = -(int)((img_size.Height * this._scale - this.Height) / this._scale);
            max_x = max_x > 0 ? max_x / 2 : max_x;
            max_y = max_y > 0 ? max_y / 2 : max_y;
            this._start_point = new Point(Math.Max(max_x, Math.Min(0, this._start_point.X)), Math.Max(max_y, Math.Min(0, this._start_point.Y)));
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this._image != null)
            {
                lock (this)
                {
                    if(this._image != null)
                    {
                        this.ClipStartPoint();
                  
                        e.Graphics.ScaleTransform(this._scale, this._scale);
                        e.Graphics.DrawImage(this._image, this._start_point);
                        

                        if (this._label != null)
                        {
                            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                            float[][] matrixItems ={
                                   new float[] { this._scale, 0, 0, 0, 0},
                                   new float[] {0, this._scale, 0, 0, 0},
                                   new float[] {0, 0, this._scale, 0, 0},
                                   new float[] {0, 0, 0, 0.5f, 0},
                                   new float[] {0, 0, 0, 0, 1}};
                            ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
                            ImageAttributes attributes = new ImageAttributes();
                            attributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            
                            e.Graphics.DrawImage(
                                this._label,
                                new Rectangle(this._start_point, new Size(this._label.Width, this._label.Height)),
                                0.0f,
                                0.0f,
                                this._label.Width,
                                this._label.Height,
                                GraphicsUnit.Pixel,
                                attributes);
                        }
                        e.Graphics.ScaleTransform(1.0f / this._scale, 1.0f / this._scale);
                    }
                }
            }
            if (this.ImageOnPaint != null)
            {
                //e.Graphics.ScaleTransform(this._scale, this._scale);
                this.ImageOnPaint(e);
                //e.Graphics.ScaleTransform(1f, 1f);
            }
        }
    }
}
