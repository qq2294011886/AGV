using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Astar
{
    class myPoint
    {
      public  float x;
      public  float y;

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public myPoint(myPoint point)
        {
            this.x = point.x;
            this.y = point.y;

        }
        public myPoint(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

         
    }
}
