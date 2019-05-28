using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageLayer
{
    public partial class LayersPannel : UserControl
    {
        public int LayerCount
        {
            get { return this.Controls.Count; }
        }

        private int selectIndex = -1;
        public int SelectIndex
        {
            get { return this.selectIndex; }
            set
            {
                this.selectIndex = value;
                if (this.SelectedChaged != null)
                    this.SelectedChaged(this, new EventArgs());//todo
            }
        }
        public event Action<object, EventArgs> SelectedChaged;
        public void AddLayer(Color color = new Color(), int index = 1)
        {
            Layer layer = new Layer(color, index);
            layer.MouseDown += Layer_MouseDown;
            int y = layer.Height * this.LayerCount;
            this.Controls.Add(layer);
            layer.Location = new Point(0, y);
            this.SelectIndex = this.LayerCount - 1;
        }

        private void Layer_MouseDown(object sender, MouseEventArgs e)
        {
            this.SelectIndex = this.Controls.IndexOf((Control)sender);
        }

        public LayersPannel()
        {
            InitializeComponent();
            this.SelectedChaged += LayersPannel_SelectedChaged;
            this.AutoScroll = true;
        }

        private void LayersPannel_SelectedChaged(object arg1, EventArgs arg2)
        {
            if(this.SelectIndex != -1)
            {
                foreach(Control ctrl in this.Controls)
                    ctrl.BackColor = Color.FromKnownColor(KnownColor.Control);
                this.Controls[this.selectIndex].BackColor = Color.OrangeRed;
            }
        }
    }
}
