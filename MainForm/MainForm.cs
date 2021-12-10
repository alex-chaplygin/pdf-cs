using PdfCS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            PdfGraphics.Rectangle rectangle = new PdfGraphics.Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);

            PdfGraphics.Init(e.Graphics, rectangle);
            PdfGraphics.SetSize(pictureBox1.Width, pictureBox1.Height);
            PdfGraphics.Render(textBox1.Text.ToCharArray().Select<char, byte>(c => (byte)c).ToArray(), null);
        }
    }
}
