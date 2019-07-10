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
    public partial class ColorLayer : UserControl
    {
        private Random rnd = new Random();

        public event Action<object, EventArgs> CheckedChanged;
        public event Action<object, EventArgs> ColorChanged;
        public event Action<object, EventArgs> DrawCheckChanged;
        public bool LayerVisible
        { get { return this.checkBox1.Checked; } }

        public object Tag { get; set; }
        public string IndexName { get { return this.textBox2.Text; } }
        public int Index { get { return (int)this.numericUpDown1.Value; } }
        public Color SelectColor { get { return this.colorDialog1.Color; } }
        public bool Selected { get; set; }
        public bool DrawEnable { get { return this.checkBox1.Checked; } }
        public ColorLayer()
        {
            InitializeComponent();
            Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            this.button1.BackColor = randomColor;
            this.colorDialog1.Color = randomColor;
            this.checkBox1.CheckedChanged += CheckBox1_CheckedChanged;
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.DrawCheckChanged != null)
                this.DrawCheckChanged(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.button1.BackColor = this.colorDialog1.Color;
                if (this.ColorChanged != null)
                    this.ColorChanged(sender, e);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CheckedChanged != null)
                this.CheckedChanged(sender, e);
        }
    }
}
