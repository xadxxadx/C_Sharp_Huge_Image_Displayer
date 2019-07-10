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
using System.Drawing.Imaging;

namespace Demo
{
    public enum REVERSE_TYPE
    {
        DRAWING,
        ADD_LAYER,
        REMOVE_LAYER
    }
    public struct ReverseStruct
    {
        public REVERSE_TYPE ReverseType;
        public ColorLayer Layer;
        public Bitmap Image;
        public ReverseStruct(REVERSE_TYPE type, ColorLayer layer, Bitmap image)
        {
            ReverseType = type;
            Layer = layer;
            Image = image;
        }
    }
    public partial class Form1 : Form
    {
        Point mouse_location;
        List<ReverseStruct> _reverse_data = new List<ReverseStruct>();
        public Form1()
        {
            InitializeComponent();
            this.displayer1.Image = new Bitmap(@"\\192.168.1.120\Data\Pictures\20180102_新陽\白點\c2.bmp");
            //this._label_images.Add(new Bitmap(@"\\192.168.1.120\Data\Pictures\20180102_新陽\白點\c3.bmp"));
            //this._label_images.Add(new Bitmap(this.displayer1.Image.Width, this.displayer1.Image.Height, PixelFormat.Format24bppRgb));
            this.displayer1.ImageMouseMove += DisplayControler1_ImageMouseMove;
            this.displayer1.ImageMouseMove += DrawOnMouseMove;
            this.displayer1.ImageOnPaint += DisplayControler1_ImageOnPaint;
            this.displayer1.LabelOnPaint += Displayer1_LabelOnPaint;
            this.displayer1.ImageMouseUp += Displayer1_ImageMouseUp;
            //this.layersPannel1.ColorChanged += LayersPannel1_ColorChanged;
            //this.layersPannel1.AddLayer(Color.Red, 1);
            this.layersPannel1.DrawCheckChanged += LayersPannel1_DrawCheckChanged;
        }

        private void LayersPannel1_DrawCheckChanged(object arg1, EventArgs arg2)
        {
            this.displayer1.Refresh();
        }

        private void LayersPannel1_ColorChanged(object arg1, EventArgs arg2)
        {
            this.displayer1.Refresh();
        }

        private void Displayer1_LabelOnPaint(PaintEventArgs obj)
        {
            lock (this)
            {
                Graphics g = obj.Graphics;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;


                foreach (Control ctrl in this.layersPannel1.Controls)
                {
                    ColorLayer layer = ctrl as ColorLayer;
                    if (layer != null && layer.DrawEnable && layer.Tag != null)
                    {
                        Bitmap image = layer.Tag as Bitmap;
                        Color color = layer.SelectColor;
                        float[][] matrixItems ={
                                   new float[] { color.R, 0, 0, 0, 0},
                                   new float[] {0, color.G, 0, 0, 0},
                                   new float[] {0, 0, color.B, 0, 0},
                                   new float[] {0, 0, 0, 0.5f, 0},
                                   new float[] {0, 0, 0, 0, 1}};
                        ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
                        ImageAttributes attributes = new ImageAttributes();
                        attributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                        g.DrawImage(
                        image,
                        new Rectangle(new Point(0, 0), new Size(image.Width, image.Height)),
                        0.0f,
                        0.0f,
                        image.Width,
                        image.Height,
                        GraphicsUnit.Pixel,
                        attributes);
                    }
                }
            }
        }

        private void Displayer1_ImageMouseUp(object arg1, MouseEventArgs arg2, Point arg3)
        {
            if (arg2.Button == MouseButtons.Right)
            {
                ColorLayer layer = this.layersPannel1.SelectedLayer;
                if(layer != null && layer.Tag != null)
                {
                    Bitmap img = layer.Tag as Bitmap;
                    this._reverse_data.Add(new ReverseStruct(REVERSE_TYPE.DRAWING, layer, img.Clone(new Rectangle(0, 0, img.Width, img.Height), img.PixelFormat)));
                    if(this._reverse_data.Count > 10)
                    {
                        this._reverse_data[0].Image.Dispose();
                        this._reverse_data.RemoveAt(0);
                    }
                }
            }
        }

        Brush red = new SolidBrush(Color.FromArgb(255, 1, 1, 1));
        private void DrawOnMouseMove(object arg1, MouseEventArgs arg2, Point arg3)
        {
            if(arg2.Button == MouseButtons.Right)
            {
                float brushSize = (float)this.numericUpDown1.Value;
                ColorLayer layer = this.layersPannel1.SelectedLayer;
                if (layer != null)
                {
                    using (Graphics g = Graphics.FromImage(layer.Tag as Bitmap))
                    {
                        g.FillEllipse(red, arg3.X - brushSize / 2, arg3.Y - brushSize / 2, brushSize, brushSize);
                    }
                }
                float scale = this.displayer1.ImageScale;
                float min_width = Math.Max(3, (brushSize + 2)* scale );
                RectangleF rect_new = new RectangleF(arg2.X - scale - brushSize * scale / 2, arg2.Y - scale - brushSize * scale / 2, min_width, min_width);
                Region r = new Region(rect_new);
                this.displayer1.InvalidPannelRegion(r);
            }
            Console.WriteLine(arg2.Location.ToString());
        }
        Pen redPen = new Pen(Color.FromArgb(255, 255, 1));
        private void DisplayControler1_ImageOnPaint(PaintEventArgs obj)
        {
            float scale = this.displayer1.ImageScale;
            int brushSize = (int)this.numericUpDown1.Value;
            obj.Graphics.DrawEllipse(redPen, this.mouse_location.X- brushSize * scale/ 2, this.mouse_location.Y- brushSize * scale / 2, (int)(brushSize * scale), (int)(brushSize * scale));
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
            foreach (ReverseStruct tmp in this._reverse_data)
                tmp.Image.Dispose();
            this._reverse_data.Clear();
            if (fileInfo != null)
            {
                this.displayer1.Image = new Bitmap(fileInfo.FullName);
                foreach (ColorLayer cl in this.layersPannel1.Controls)
                {
                    Bitmap emptyImg = new Bitmap(this.displayer1.Image.Width, this.displayer1.Image.Height, PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(emptyImg))
                    {
                        using (Brush brush = new SolidBrush(Color.FromArgb(0, 0, 0, 0)))
                        {
                            g.FillRectangle(brush, new Rectangle(0, 0, emptyImg.Width, emptyImg.Height));
                        }
                    }
                    Bitmap tmp = cl.Tag as Bitmap;
                    cl.Tag = emptyImg;
                    if (tmp != null)
                        tmp.Dispose();
                }
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            FileInfo fileInfo = e.Node.Tag as FileInfo;
            this.loadNewImage(fileInfo);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeView1.SelectedNode != null)
            {
                FileInfo file = this.treeView1.SelectedNode.Tag as FileInfo;
                foreach (ColorLayer cl in this.layersPannel1.Controls)
                {
                    Bitmap labelColor = cl.Tag as Bitmap;
                    Bitmap labelGray = new Bitmap(labelColor.Width, labelColor.Height, PixelFormat.Format8bppIndexed);
                    BitmapData data = labelGray.LockBits(new Rectangle(0, 0, labelGray.Width, labelGray.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                    byte value = (byte)cl.Index;
                    unsafe
                    {
                        byte* ptr = (byte*)data.Scan0;
                        for (int h = 0; h < labelGray.Height; ++h)
                        {
                            byte* ptrRow = ptr + h * data.Stride;
                            for (int w = 0; w < labelGray.Width; ++w)
                            {
                                Color color = labelColor.GetPixel(w, h);
                                if (color.R + color.G + color.B != 0)
                                {
                                    ptrRow[w] = value;
                                }
                            }
                        }
                    }
                    labelGray.UnlockBits(data);
                    string distName = file.Directory.FullName + "\\" + Path.GetFileNameWithoutExtension(file.Name) + "_" + cl.IndexName + file.Extension;
                    labelGray.Save(distName);
                    labelGray.Dispose();
                }
            }
            //this._label_image.Save(this._label_file_path);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Control && e.KeyCode == Keys.Z)
            {
                if (this._reverse_data.Count > 1)
                {
                    lock (this)
                    {
                        ReverseStruct rs = this._reverse_data[this._reverse_data.Count - 1];
                        switch (rs.ReverseType)
                        {
                            case REVERSE_TYPE.DRAWING:
                                for (int i = this._reverse_data.Count - 2; i >= 0; --i)
                                {
                                    if (this._reverse_data[i].Layer == rs.Layer)
                                    {
                                        (rs.Layer.Tag as Bitmap).Dispose();
                                        rs.Layer.Tag = this._reverse_data[i].Image.Clone(new Rectangle(0, 0, this._reverse_data[i].Image.Width, this._reverse_data[i].Image.Height), this._reverse_data[i].Image.PixelFormat);
                                        break;
                                    }
                                }
                                break;
                            case REVERSE_TYPE.ADD_LAYER:
                                this.layersPannel1.RemoveLayer(rs.Layer);
                                break;
                            case REVERSE_TYPE.REMOVE_LAYER:
                                this.layersPannel1.AddLayer(rs.Layer);
                                break;
                        }
                        this._reverse_data[this._reverse_data.Count - 1].Image.Dispose();
                        this._reverse_data.RemoveAt(this._reverse_data.Count - 1);
                        Console.WriteLine(this._reverse_data.Count.ToString());
                    }
                }
                this.displayer1.Refresh();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap emptyImg = new Bitmap(this.displayer1.Image.Width, this.displayer1.Image.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(emptyImg))
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(0, 0, 0, 0)))
                {
                    g.FillRectangle(brush, new Rectangle(0, 0, emptyImg.Width, emptyImg.Height));
                }
            }
            lock (this)
            {
                ColorLayer layer = this.layersPannel1.AddLayer();
                /*List<Bitmap> historys = new List<Bitmap>();
                for (int i = 0; i < this._label_images.Count; ++i)
                    historys.Add(null);
                historys.Add(emptyImg.Clone(new Rectangle(0,0,emptyImg.Width, emptyImg.Height), emptyImg.PixelFormat));
                this._reverse_list.Add(historys);*/
                layer.Tag = emptyImg;
                this._reverse_data.Add(new ReverseStruct(REVERSE_TYPE.ADD_LAYER, layer, emptyImg.Clone(new Rectangle(0, 0, emptyImg.Width, emptyImg.Height), emptyImg.PixelFormat)));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.layersPannel1.SelectedLayer != null)
            {
                Bitmap label = this.layersPannel1.SelectedLayer.Tag as Bitmap;
                this._reverse_data.Add(new ReverseStruct(REVERSE_TYPE.REMOVE_LAYER, this.layersPannel1.SelectedLayer, 
                    label.Clone(new Rectangle(0,0,label.Width,label.Height), label.PixelFormat)));
                lock (this)
                {
                    this.layersPannel1.RemoveLayer(this.layersPannel1.SelectedLayer);
                }
            }
        }
    }
}

