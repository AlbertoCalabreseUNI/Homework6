using Homework_6.DataObjects;
using Homework_6.Generators;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Homework_6
{
    public class Viewport
    {
        //Viewport elements we need
        //Constructor
        private PictureBox PictureBox;
        public Rectangle Area { get; set; }
        //Temporary rectangle used for dragMode
        public Rectangle AreaOnMouseDown { get; set; }

        //Drawing elements. The bitmap is used ONLY to get the Graphics object, otherwise we'd have to pass it everytime in each function
        //Or call drawing functions from the Form1 object instance which is way messier.
        Bitmap Bmp;
        public Graphics G { get; set; }
        public Pen BorderColor = Pens.Black;

        //Booleans to decide which operation to do
        public bool dragMode;
        public bool resizeMode;

        //This gets set ONLY when we click on the viewport instance. It's used to calculate deltas
        public Point MouseClickLocation { get; set; }

        //TODO Add Histograms

        //Random Generators
        RandomGenerators Generators;

        public Viewport(PictureBox picturebox, Rectangle area)
        {
            this.PictureBox = picturebox;
            this.Area = area;

            this.Bmp = new Bitmap(this.PictureBox.Width, this.PictureBox.Height);
            this.G = Graphics.FromImage(this.Bmp);
            this.PictureBox.Image = this.Bmp;

            //Graphics options
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            //Mode initialization
            this.dragMode = false;
            this.resizeMode = false;

            //Random generators
            this.Generators = new RandomGenerators();
        }

        #region Viewport Move/Resize Region
        public void MoveArea(int DeltaX, int DeltaY)
        {
            //Let's avoid null pointer exceptions
            if (this.AreaOnMouseDown == null) return;
            this.Area = new Rectangle(this.AreaOnMouseDown.X + DeltaX, this.AreaOnMouseDown.Y + DeltaY, this.AreaOnMouseDown.Width, this.AreaOnMouseDown.Height);

            //Let's redraw everything after moving the viewport
            this.RedrawAfterMoveOrResize();
        }

        public void ResizeArea(int DeltaX, int DeltaY)
        {
            if (this.AreaOnMouseDown == null) return;
            this.Area = new Rectangle(this.AreaOnMouseDown.X, this.AreaOnMouseDown.Y, this.AreaOnMouseDown.Width + DeltaX, this.AreaOnMouseDown.Height + DeltaY);

            this.RedrawAfterMoveOrResize();
        }

        public void RedrawAfterMoveOrResize()
        {
            G.Clear(Color.White);
            G.DrawRectangle(this.BorderColor, this.Area);
            this.DrawLines();
            //this.DrawHistogram();
            this.PictureBox.Image = this.Bmp;
        }
        #endregion

        #region Viewport Structure Drawing
        public void DrawViewport(Pen pen)
        {
            this.ResetGraphic();
            G.Clear(Color.White);
            G.DrawRectangle(pen, this.Area);
        }

        private void ResetGraphic()
        {
            this.Bmp = new Bitmap(this.PictureBox.Width, this.PictureBox.Height);
            G = Graphics.FromImage(this.Bmp);
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            this.PictureBox.Image = this.Bmp;
        }
        #endregion

        #region Charts Drawing Functions
        public void DrawLines()
        {
            double minX = this.getMinX();
            double minY = this.getMinY();
            double maxX = this.getMaxX();
            double maxY = this.getMaxY();

            double rangeX = maxX - minX;
            double rangeY = maxY - minY;

            int index = 0;

            //Fail safe check
            if (this.G != null)
            {
                //Fail safe check
                if (this.Generators.RandomColorsList.Count() == 0)
                    this.Generators.PopulateColorList(this.Generators.Sequences.Count);

                foreach (List<DataPoint> realPoints in this.Generators.Sequences)
                {
                    List<Point> points = new List<Point>();

                    foreach (DataPoint realPoint in realPoints)
                    {
                        Point viewportP = this.FromRealWorldToViewport(realPoint.x, minX, rangeX, realPoint.y, minY, rangeY);
                        points.Add(viewportP);
                    }

                    G.DrawLines(new Pen(Generators.RandomColorsList[index]), points.ToArray());
                    index++;
                }
            }
        }
        #endregion

        #region Window to Viewport Calculation Function
        //Homework 4 Functions to calculate the scale for Window to Viewport position transformation for element's X axis
        private int XPosScale(double realX, double minX_Window, double rangeX)
        {
            //converting
            return (int)(this.Area.Left + this.Area.Width * (realX - minX_Window) / rangeX);

        }
        //Homework 4 Functions to calculate the scale for Window to Viewport position transformation for element's Y axis
        private int YPosScale(double realY, double minY_Window, double rangeY)
        {
            //converting
            return (int)(this.Area.Top + this.Area.Height - this.Area.Height * (realY - minY_Window) / rangeY);

        }

        //Homework 4 Function to calculate the position of a point from the window to a viewport
        private Point FromRealWorldToViewport(double realX, double minX_Window, double rangeX, double realY, double minY_Window, double rangeY)
        {
            int x_graphic = XPosScale(realX, minX_Window, rangeX);
            int y_graphic = YPosScale(realY, minY_Window, rangeY);

            return new Point(x_graphic, y_graphic);
        }
        #endregion

        #region Min and Max of Randomized points for Viewport calculation
        private double getMinX()
        {
            double minX = this.Generators.Plotted[0].x;

            foreach (DataPoint p in this.Generators.Plotted)
                if (p.x < minX)
                    minX = p.x;
            
            return minX;
        }

        private double getMinY()
        {
            double minY = this.Generators.Plotted[0].y;

            foreach (DataPoint p in this.Generators.Plotted)
                if (p.y < minY)

                    minY = p.y;

            return minY;
        }

        private double getMaxX()
        {
            double maxX = this.Generators.Plotted[0].x;

            foreach (DataPoint p in this.Generators.Plotted)
                if (p.x > maxX)
                    maxX = p.x;
            
            return maxX;
        }

        private double getMaxY()
        {
            double maxY = this.Generators.Plotted[0].y;

            foreach (DataPoint p in this.Generators.Plotted)
                if (p.y > maxY)
                    maxY = p.y;
            
            return maxY;
        }
        #endregion

        public void CreateData()
        {
            this.DrawViewport(this.BorderColor);
            if (this.Generators.Sequences.Count == 0)
                this.Generators.PopulateSequenceList();
            else
                this.Generators.ResetLists();

            this.DrawLines();
        }
    }
}

