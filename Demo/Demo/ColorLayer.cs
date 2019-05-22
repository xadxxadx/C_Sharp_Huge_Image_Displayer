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

        public event Action<object, int> IndexChanged;
        public event Action<object, EventArgs> CheckedChanged;
        public bool LayerVisible
        { get { return this.checkBox1.Checked; } }

        public object Tag { get; set; }

        public ColorLayer()
        {
            InitializeComponent();
            Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            this.button1.BackColor = randomColor;
            this.colorDialog1.Color = randomColor;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.button1.BackColor = this.colorDialog1.Color;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int val = 0;
            if (int.TryParse(this.textBox1.Text, out val))
                if (this.IndexChanged != null)
                    this.IndexChanged(this, val);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CheckedChanged != null)
                this.CheckedChanged(sender, e);
        }
    }
}
