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
    public partial class Layer : UserControl
    {
        public Color LayerColor {
            get
            {
                return this.button1.BackColor;
            }
        }
        public int LayerIndex
        {
            get
            {
                return (int)this.numericUpDown1.Value;
            }
        }
        public event Action<object, EventArgs> ColorChanged;
        public event Action<object, EventArgs> IndexChanged;

        public Layer()
        {
            InitializeComponent();
        }

        public Layer(Color color, int index)
        {
            InitializeComponent();
            this.colorDialog1.Color = color;
            this.button1.BackColor = color;
            this.numericUpDown1.Value = index;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.button1.BackColor = this.colorDialog1.Color;
                if (this.ColorChanged != null)
                    this.ColorChanged(this, e);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (this.IndexChanged != null)
                this.IndexChanged(this, e);
        }

        private void numericUpDown1_MouseClick(object sender, MouseEventArgs e)
        {
            this.OnMouseDown(e);
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            this.OnMouseDown(e);
        }
    }
}
