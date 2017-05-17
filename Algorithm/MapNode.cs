using System;
using System.Linq;
using System.Text;
using System.Drawing;
using AGV_V1._0;
using System.Collections.Generic;




namespace Astar
{
    class MapNode
    {

        //public Image img_Belt = Resources.Belt;
        //public Image img_Mid = Resources.Mid;
        //public Image img_Road = Resources.Road;
        //public Image img_Destination = Resources.Destination;
        //public Image img_ChargeStation = Resources.ChargeStation;
        //public Image img_Obstacle = Resources.Obstacle;
        //public Image img_Scanner = Resources.Scanner;

      //  public   enum MapNodeType1 { img_Belt,img_Mid ,img_Road,img_Destination,img_ChargeStation,img_Obstacle,img_Scanner};

        public int x;              //节点的横坐标    
        public int y;               //节点的纵坐标
        public bool Node_Type;     //节点可不可达
        public int sur;             //邻接点的个数
        public int value;           //节点的值

        public Boolean nodeCanUsed;   //节点是否被占用
        public List<int> vehiclePriority{get;set;} //通过节点的小车优先级序列如{1,4,6},数字为小车编号；


        

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public Image oth;
        public Image another;

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        /// <summary>
        /// 含参构造函数
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        /// <param name="reachable"></param>
        public MapNode(int x, int y, int value, bool node_type)
        {
            this.x = x;
            this.y = y;
            this.value = value;
            this.Node_Type = node_type;
            this.vehiclePriority = new List<int>();

        }
        
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public MapNode()
        { }
    }
}
