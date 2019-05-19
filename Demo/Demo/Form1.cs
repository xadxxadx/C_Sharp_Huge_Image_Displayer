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
        Bitmap _label_image;
        List<Bitmap> _reverse_list = new List<Bitmap>();
        string _label_file_path = "";
        public Form1()
        {
            InitializeComponent();
            this.displayer1.Image = new Bitmap(@"\\192.168.1.120\Data\Pictures\20180102_新陽\白點\c2.bmp");
            this._label_image = new Bitmap(this.displayer1.Image.Width, this.displayer1.Image.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            this.displayer1.Label = this._label_image;
            this.displayer1.ImageMouseMove += DisplayControler1_ImageMouseMove;
            this.displayer1.ImageMouseMove += DrawOnMouseMove;
            this.displayer1.ImageOnPaint += DisplayControler1_ImageOnPaint;
            this.displayer1.ImageMouseUp += Displayer1_ImageMouseUp;
        }

        private void Displayer1_ImageMouseUp(object arg1, MouseEventArgs arg2, Point arg3)
        {
            this._reverse_list.Add(this._label_image.Clone(new Rectangle(0,0,this._label_image.Width, this._label_image.Height), this._label_image.PixelFormat));
            if(this._reverse_list.Count() > 10)
            {
                Bitmap drop = this._reverse_list[0];
                drop.Dispose();
                this._reverse_list.RemoveAt(0);
            }
        }

        private void DrawOnMouseMove(object arg1, MouseEventArgs arg2, Point arg3)
        {
            if(arg2.Button == MouseButtons.Right)
            {
                float brushSize = (float)this.numericUpDown1.Value;
                using (Graphics g = Graphics.FromImage(this._label_image))
                {
                    g.FillEllipse(Brushes.Red, arg3.X - brushSize/ 2, arg3.Y - brushSize / 2, brushSize  , brushSize);
                }
                float scale = this.displayer1.ImageScale;
                float min_width = Math.Max(3, (brushSize + 2)* scale );
                RectangleF rect_new = new RectangleF(arg2.X - scale - brushSize * scale / 2, arg2.Y - scale - brushSize * scale / 2, min_width, min_width);
                Region r = new Region(rect_new);
                this.displayer1.InvalidPannelRegion(r);
            }
            Console.WriteLine(arg2.Location.ToString());
        }

        private void DisplayControler1_ImageOnPaint(PaintEventArgs obj)
        {
            float scale = this.displayer1.ImageScale;
            int brushSize = (int)this.numericUpDown1.Value;
            obj.Graphics.DrawEllipse(Pens.Red, this.mouse_location.X- brushSize * scale/ 2, this.mouse_location.Y- brushSize * scale / 2, (int)(brushSize * scale), (int)(brushSize * scale));
            //Console.WriteLine(this.mouse_location.ToString());
        }

        private void DisplayControler1_ImageMouseMove(object arg1, MouseEventArgs arg2, Point pixelLoc)
        {
            float scale = this.displayer1.ImageScale;
            int brushSize = (int)this.numericUpDown1.Value;
            float min_width = Math.Max(3, brushSize * scale + 2);
            RectangleF rect_old = new RectangleF(this.mouse_location.X - brushSize * scale / 2 - 1, this.mouse_location.Y - brushSize * scale / 2 - 1, min_width, min_width);
            RectangleF rect_new = new RectangleF(arg2.X - brushSize * scale / 2 - 1, arg2.Y - brushSize * scale / 2 - 1, min_width, min_width);
            mouse_location = arg2.Location;
            Region r = new Region(rect_old);
            r.Union(rect_new);
            this.displayer1.InvalidPannelRegion(r);
            //this.displayer1.Refresh();
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

        private void loadNewImage(FileInfo fileInfo)
        {
            foreach (Bitmap tmp in this._reverse_list)
                tmp.Dispose();
            this._reverse_list.Clear();
            if (fileInfo != null)
            {
                this.displayer1.Image = new Bitmap(fileInfo.FullName);
                this._label_file_path = System.IO.Path.Combine(fileInfo.Directory.FullName, Path.GetFileNameWithoutExtension(fileInfo.Name) + "_mask" + fileInfo.Extension);
                if (File.Exists(this._label_file_path))
                    this._label_image = new Bitmap(this._label_file_path);
                else
                    this._label_image = new Bitmap(this.displayer1.Image.Width, this.displayer1.Image.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                this._reverse_list.Add(this._label_image.Clone(new Rectangle(0, 0, this._label_image.Width, this._label_image.Height), this._label_image.PixelFormat));
                this.displayer1.Label = this._label_image;
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            FileInfo fileInfo = e.Node.Tag as FileInfo;
            this.loadNewImage(fileInfo);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Control && e.KeyCode == Keys.Z)
            {
                if (this._reverse_list.Count() > 1)
                {
                    Bitmap tmp = this._reverse_list[this._reverse_list.Count() - 2];
                    this._reverse_list.RemoveAt(this._reverse_list.Count() - 1);
                    using (Graphics g = Graphics.FromImage(this._label_image))
                        g.DrawImage(tmp, Point.Empty);
                }
            }
        }

    }
}
