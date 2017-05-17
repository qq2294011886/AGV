using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Astar
{
    class Open
    {
      public  int length;        //当前队列的长度
     public   Close[] Array = new Close[AstarSearch.MaxLength];    //评价结点的指针
    }
}
