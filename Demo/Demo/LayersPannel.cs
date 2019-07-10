using System;
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
    public partial class LayersPannel : UserControl
    {
        public int LayerCount
        {
            get { return this.Controls.Count; }
        }

        public Color GetIndexColor(int index)
        {
            //return ((ColorLayer)this.Controls[index]).LayerColor;
            return new Color();
        }
        public ColorLayer SelectedLayer
        {
            get
            {
                foreach(Control ctrl in this.Controls)
                {
                    ColorLayer layer = ctrl as ColorLayer;
                    if (layer != null && layer.Selected)
                        return layer;
                }
                return null;
            }
        }
        public event Action<object, EventArgs> SelectedChaged;
        public event Action<object, EventArgs> ColorChanged;
        public event Action<object, EventArgs> DrawCheckChanged;
        public ColorLayer AddLayer()
        {
            this.SuspendLayout();
            ColorLayer layer = new ColorLayer();
            layer.DrawCheckChanged += Layer_DrawCheckChanged;
            layer.ColorChanged += Layer_ColorChanged;
            layer.MouseDown += Layer_MouseDown;
            int y = layer.Height * this.LayerCount;
            this.Controls.Add(layer);
            layer.Location = new Point(0, y);
            layer.Width = this.Width;
            layer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))));
           
            if (this.SelectedChaged != null)
                this.SelectedChaged(layer, null);
            this.AligmentContorls();
            this.ResumeLayout();
            return layer;
        }

        public ColorLayer AddLayer(ColorLayer layer)
        {
            this.Controls.Add(layer);
            int y = layer.Height * this.LayerCount;
            layer.Location = new Point(0, y);
            layer.Width = this.Width;
            layer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))));
            this.AligmentContorls();
            return layer;
        }

        public void RemoveLayer(ColorLayer layer)
        {
            if (layer != null)
            {
                this.Controls.Remove(layer);
                this.AligmentContorls();
            }
        }

        private void Layer_DrawCheckChanged(object arg1, EventArgs arg2)
        {
            if (this.DrawCheckChanged != null)
                this.DrawCheckChanged(arg1, arg2);
        }

        private void SelectLayer(ColorLayer layer)
        {
            foreach(Control ctrl in this.Controls)
            {
                ColorLayer ctrlLayer = ctrl as ColorLayer;
                if (ctrlLayer != null)
                {
                    ctrlLayer.BackColor = Color.FromKnownColor(KnownColor.Control);
                    ctrlLayer.Selected = false;
                }
            }
            layer.Selected = true;
            layer.BackColor = Color.OrangeRed;
        }
        private void AligmentContorls()
        {
            for(int i = 0; i < this.Controls.Count; ++i)
            {
                this.Controls[i].Location = new Point(0, this.Controls[i].Height * i);
                this.Controls[i].Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))));
            }
        }

        private void Layer_ColorChanged(object arg1, EventArgs arg2)
        {
            if (this.ColorChanged != null)
                this.ColorChanged(arg1, arg2);
        }

        private void Layer_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.SelectedChaged != null)
                this.SelectedChaged(sender, e);
        }

        public LayersPannel()
        {
            InitializeComponent();
            this.SelectedChaged += LayersPannel_SelectedChaged;
            this.AutoScroll = true;
        }

        private void LayersPannel_SelectedChaged(object arg1, EventArgs arg2)
        {
            SelectLayer(arg1 as ColorLayer);
        }
    }
}
