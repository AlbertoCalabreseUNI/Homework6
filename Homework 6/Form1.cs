using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Homework_6
{
    public partial class Form1 : Form
    {
        //In this case we have one single viewport, but we might want multiple ones. We can use a foreach loop on the list
        //to check for operations to do on each viewport (like mouse up/down/move).
        //TODO: Fix the bug where if you loop to check on every element inside the list, if you keep the mouse clicked and pass over multiple
        //viewports all of them change mode and get moved/resized even if we want to change just one.
        public List<Viewport> viewports = new List<Viewport>(); //public Viewport vp;

        //TODO: Make them variable
        public static int SequencesNumber = 100;
        public static int SequencesSize = 100;
        public static int InstantToPlotInstogram = 50;
        public static double SuccessProbability = 50 / 100;
        public static int Mode = 0;

        public Form1()
        {
            InitializeComponent();

            //Let's initialize our Viewport with an arbitrary size. I've chosen a rectangle that starts from the picturebox corner and is half of its size
            this.viewports.Add(new Viewport(this.pictureBox1, new Rectangle(this.pictureBox1.Location.X, this.pictureBox1.Location.Y, this.pictureBox1.Width / 2, this.pictureBox1.Height / 2)));


            //Creating functions to move/resize viewports
            this.pictureBox1.MouseDown += this.PictureBox1_MouseDown;
            this.pictureBox1.MouseUp += this.PictureBox1_MouseUp;
            this.pictureBox1.MouseMove += this.PictureBox1_MouseMove;
        }
        
        //It starts generation and drawing
        private void button1_Click(object sender, EventArgs e)
        {
            SequencesNumber = Int32.Parse(this.textBox1.Text);
            SequencesSize = Int32.Parse(this.textBox2.Text);
            SuccessProbability = Double.Parse(this.textBox3.Text) / 100;
            Mode = this.comboBox1.SelectedItem != null ? this.comboBox1.SelectedIndex : 0;

            foreach (Viewport vp in this.viewports)
                vp.CreateData();
        }
        #region Viewport Controller
        //We handle picturebox mouse in Form1 as it makes more sense since it's an instanced object inside the Form1 instance.
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            foreach(Viewport vp in this.viewports)
            {
                if(vp.Area.Contains(e.Location))
                {
                    vp.AreaOnMouseDown = vp.Area; //We temporary save the original area
                    vp.MouseClickLocation = e.Location;

                    if (e.Button == MouseButtons.Left) vp.dragMode = true;
                    else if (e.Button == MouseButtons.Right) vp.resizeMode = true;
                }
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            /* If we want to reset the mode of an individual viewport we can just manually disable them
             * this.vp.dragMode = false;
             * this.vp.resizeMode = false;
             */
            foreach (Viewport vp in this.viewports)
            {
                vp.dragMode = false;
                vp.resizeMode = false;
            }
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            foreach(Viewport vp in this.viewports)
            {
                //We calculate how much to move the viewport
                int deltaX = e.X - vp.MouseClickLocation.X;
                int deltaY = e.Y - vp.MouseClickLocation.Y;

                if (vp.dragMode)
                    vp.MoveArea(deltaX, deltaY);
                else if (vp.resizeMode)
                    vp.ResizeArea(deltaX, deltaY);
            }
        }
        #endregion
    }
}
