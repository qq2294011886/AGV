using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Astar
{
    class Close
    {
       public  MapNode cur;

        internal MapNode Cur
        {
            get { return cur; }
            set { cur = value; }
        }
      public   bool vis;

      
      public   Close from;

        internal Close From
        {
            get { return from; }
            set { from = value; }
        }
      public   float F, G;

        public float G1
        {
            get { return G; }
            set { G = value; }
        }

        public float F1
        {
            get { return F; }
            set { F = value; }
        }
      public   int H;
    }
}
