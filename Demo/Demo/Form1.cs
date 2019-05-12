using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Demo
{
    public partial class Form1 : Form
    {
        Point mouse_location;
        Image _label_image;
        int _brush_size = 5;
        public Form1()
        {
            InitializeComponent();
            this.displayer1.Image = new Bitmap(@"\\192.168.1.120\Data\Pictures\20180102_新陽\白點\c2.bmp");
            this._label_image = new Bitmap(this.displayer1.Image.Width, this.displayer1.Image.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            this.displayer1.Label = this._label_image;
            this.displayer1.ImageMouseMove += DisplayControler1_ImageMouseMove;
            this.displayer1.ImageMouseMove += DrawOnMouseMove;
            this.displayer1.ImageOnPaint += DisplayControler1_ImageOnPaint;
        }

        private void DrawOnMouseMove(object arg1, MouseEventArgs arg2, Point arg3)
        {
            if(arg2.Button == MouseButtons.Right)
            {
                using (Graphics g = Graphics.FromImage(this._label_image))
                {
                    g.FillEllipse(Brushes.Red, new Rectangle(arg3.X - this._brush_size/2, arg3.Y - this._brush_size/2, this._brush_size, this._brush_size));
                }
            }
        }

        private void DisplayControler1_ImageOnPaint(PaintEventArgs obj)
        {
            float scale = this.displayer1.ImageScale;
            obj.Graphics.DrawEllipse(Pens.Red, this.mouse_location.X-this._brush_size* scale/ 2, this.mouse_location.Y- this._brush_size* scale / 2, (int)(this._brush_size * scale), (int)(this._brush_size * scale));
            //Console.WriteLine(this.mouse_location.ToString());
        }

        private void DisplayControler1_ImageMouseMove(object arg1, MouseEventArgs arg2, Point pixelLoc)
        {
            float scale = this.displayer1.ImageScale;
            float min_width = Math.Max(3, this._brush_size * scale + 2);
            RectangleF rect_old = new RectangleF(this.mouse_location.X - this._brush_size * scale / 2 - 1, this.mouse_location.Y - this._brush_size * scale / 2 - 1, min_width, min_width);
            RectangleF rect_new = new RectangleF(arg2.X - this._brush_size * scale / 2 - 1, arg2.Y - this._brush_size * scale / 2 - 1, min_width, min_width);
            mouse_location = arg2.Location;
            Region r = new Region(rect_old);
            r.Union(rect_new);
            this.displayer1.InvalidPannelRegion(r);
            //this.displayer1.Invalidate();
            //this.displayControler1.Invalidate()
            //Console.WriteLine(pixelLoc.ToString());
        }

        private string _search_pattern = "*.bmp|*.jpg|*.tif|*.png";
        private void selectFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = this.folderBrowserDialog1.SelectedPath;
                this.treeView1.Nodes.Clear();
                DirectoryInfo dirInfo = new DirectoryInfo(this.folderBrowserDialog1.SelectedPath);
                foreach (string pattern in this._search_pattern.Split('|'))
                {
                    foreach (FileInfo fileInfo in dirInfo.GetFiles(pattern))
                    {
                        TreeNode node = new TreeNode(fileInfo.Name + fileInfo.Extension);
                        node.Tag = fileInfo;
                        this.treeView1.Nodes.Add(node);
                    }
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            FileInfo fileInfo = e.Node.Tag as FileInfo;
            if(fileInfo != null)
            {
                this.displayer1.Image = new Bitmap(fileInfo.FullName);
                string label_str = System.IO.Path.Combine(fileInfo.Directory.FullName, Path.GetFileNameWithoutExtension(fileInfo.Name) + "_mask" + fileInfo.Extension);
                if(File.Exists(label_str))
                    this._label_image = new Bitmap(label_str);
                else
                    this._label_image = new Bitmap(this.displayer1.Image.Width, this.displayer1.Image.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                this.displayer1.Label = this._label_image;
            }
        }
    }
}
