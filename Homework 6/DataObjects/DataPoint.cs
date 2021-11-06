using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework_6.DataObjects
{
    public class DataPoint
    {
        public double x { get; set; }
        public double y { get; set; }
        public DataPoint(double inX, double inY)
        {
            this.x = inX;
            this.y = inY;
        }
    }
}
