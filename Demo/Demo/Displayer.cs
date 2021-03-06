﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo
{
    public partial class Displayer : UserControl
    {
        public Displayer()
        {
            InitializeComponent();
            this.displayControler1.ImageMouseMove += DisplayControler1_ImageMouseMove;
            this.displayControler1.ImageOnPaint += DisplayControler1_ImageOnPaint;
            this.displayControler1.LabelOnPaint += DisplayControler1_LabelOnPaint;
            this.displayControler1.ImageMouseDown += DisplayControler1_ImageMouseMove;
            this.displayControler1.ImageMouseUp += DisplayControler1_ImageMouseUp;
            this.SizeChanged += UpdateScrollBar;
            this.displayControler1.ScrollBarNeedToChange += UpdateScrollBar;
            this.hScrollBar1.ValueChanged += ScrollBarH_ValueChanged;
            this.vScrollBar1.ValueChanged += ScrollBarV_ValueChanged;
            this.Invalidated += Displayer_Invalidated;
        }

        private void DisplayControler1_LabelOnPaint(PaintEventArgs obj)
        {
            if (this.LabelOnPaint != null)
                this.LabelOnPaint(obj);
        }

        private void DisplayControler1_ImageMouseUp(object arg1, MouseEventArgs arg2, Point arg3)
        {
            if (this.ImageMouseUp != null)
                this.ImageMouseUp(arg1, arg2, arg3);
        }

        private void Displayer_Invalidated(object sender, InvalidateEventArgs e)
        {
            this.displayControler1.Invalidate();
        }

        private void ScrollBarH_ValueChanged(object sender, EventArgs e)
        {
            if (!this.displayer2scrollbar)
            {
                this.displayControler1.StartPoint = new Point(
                    -(int)(this.hScrollBar1.Value / this.displayControler1.ImageScale),
                     this.displayControler1.StartPoint.Y
                    );
            }
        }

        private void ScrollBarV_ValueChanged(object sender, EventArgs e)
        {
            if (!this.displayer2scrollbar)
            {
                this.displayControler1.StartPoint = new Point(
                     this.displayControler1.StartPoint.X,
                    -(int)(this.vScrollBar1.Value / this.displayControler1.ImageScale)
                    );
            }
        }

        private void DisplayControler1_ImageOnPaint(PaintEventArgs obj)
        {
            if (this.ImageOnPaint != null)
                this.ImageOnPaint(obj);
        }

        private void DisplayControler1_ImageMouseMove(object arg1, MouseEventArgs arg2, Point arg3)
        {
            if (this.ImageMouseMove != null)
                this.ImageMouseMove(arg1, arg2, arg3);
        }

        public Image Image
        {
            get { return this.displayControler1.Image; }
            set
            {
                this.displayControler1.StartPoint = Point.Empty;
                this.displayControler1.Image = value;
            }
        }

        public float ImageScale { get { return this.displayControler1.ImageScale; } }

        public event Action<object, MouseEventArgs, Point> ImageMouseMove;
        public event Action<object, MouseEventArgs, Point> ImageMouseUp;
        public event Action<PaintEventArgs> ImageOnPaint;
        public event Action<PaintEventArgs> LabelOnPaint;


        private bool displayer2scrollbar = false;
        private void UpdateScrollBar(object sender, EventArgs e)
        {
            if (this.displayControler1.Image != null)
            {
                this.displayer2scrollbar = true;
                this.hScrollBar1.LargeChange = this.displayControler1.Width;
                this.hScrollBar1.Maximum = Math.Max(1, (int)(this.displayControler1.Image.Size.Width * this.displayControler1.ImageScale - this.displayControler1.Width)) + this.hScrollBar1.LargeChange;
                this.hScrollBar1.SmallChange = Math.Max(1, (int)(this.displayControler1.ImageScale));
                this.hScrollBar1.Value = Math.Max(this.hScrollBar1.Minimum, Math.Min(this.hScrollBar1.Maximum, (int)Math.Abs(this.displayControler1.StartPoint.X * this.displayControler1.ImageScale)));

                this.vScrollBar1.LargeChange = this.displayControler1.Height;
                this.vScrollBar1.Maximum = Math.Max(1, (int)(this.displayControler1.Image.Size.Height * this.displayControler1.ImageScale - this.displayControler1.Height)) + this.vScrollBar1.LargeChange;
                this.vScrollBar1.SmallChange = Math.Max(1, (int)(this.displayControler1.ImageScale));
                this.vScrollBar1.Value = Math.Max(this.vScrollBar1.Minimum, Math.Min(this.vScrollBar1.Maximum, (int)Math.Abs(this.displayControler1.StartPoint.Y * this.displayControler1.ImageScale)));
                this.displayer2scrollbar = false;
            }
        }

        public void InvalidPannelRegion(Region r)
        {
            this.displayControler1.Invalidate(r);
        }
    }
}
