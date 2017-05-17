using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Data;
using AGV_V1._0.Properties;
using Astar;

namespace AGV_V1._0
{
    /// <summary>
    /// AGV小车移动的方向
    /// </summary>
    enum Direction
    {
        ForWard,
        BackWard,
        Left,
        Right
    }
    class Vehicle
    {

        //车的横坐标
        public int X
        {
            get;
            set;
        }

        //车的纵坐标
        public int Y
        {
            get;
            set;
        }

        public int V_Number
        {
            get;
            set;
        }

        public Image V_Picture;

       public int routeIndex = 0;
       public int stopTime = constDefine.STOP_TIME;//0406 等待时长，超过则重新规划路线；
        //int length1 = coord.GetLength() ;

        //车的4个状态，0向后 1向前 2向左 3向右
        /*public Image[] img_Vehicle =
                                    {
                                      Resources.Vehicle_B,
                                      Resources.Vehicle_F,
                                      Resources.Vehicle_L,
                                      Resources.Vehicle_R
                                     };*/
        public Image[] img_Vehicle =
                                    {
                                      Resources.Vehicle_Red,
                                      Resources.Vehicle_Black,
                                      Resources.Vehicle_Green,
                                      Resources.Vehicle_Orange,
                                      Resources.Vehicle_White,
                                      Resources.Vehicle_Yellow
                                    };

        public Image[] img_Vehicle_Copy =
                                    {
                                      Resources.Vehicle_Red,
                                      Resources.Vehicle_Black,
                                      Resources.Vehicle_Green,
                                      Resources.Vehicle_Orange,
                                      Resources.Vehicle_White,
                                      Resources.Vehicle_Yellow
                                    };

        //小车的速度
        public int Speed
        {
            get;
            set;
        }

        //小车的电量
        public int Electricity
        {
            get;
            set;
        }

        //小车的加速度
        public int Acceleration
        {
            get;
            set;
        }

        //小车的最大速度
        public int MaxSpeed
        {
            get;
            set;
        }

        //判断小车是否到终点
        public bool Arrive;
        
        public v_state vehical_state;
        

        /// <summary>
        /// 构造函数，初始化所有变量
        /// </summary>
        /// <param name="speed">速度</param>
        /// <param name="electricity">电量</param>
        /// <param name="acceleration">加速度</param>
        /// <param name="maxspeed">最大速度</param>
        public Vehicle(int x, int y, int speed, int electricity, int acceleration, int maxspeed)
        {
            this.startX = x;
            this.startY = y;
            this.X = x;
            this.Y = y;
            this.Speed = speed;
            this.Electricity = electricity;
            this.Acceleration = acceleration;
            this.MaxSpeed = maxspeed;
            this.vehical_state = v_state.normal;
        }

        public Vehicle(int x, int y, Image picture,bool arrive)
        {
            this.startX = x;
            this.startY = y;
            this.X = y * constDefine.BENCHMARK + constDefine.BEGIN_X;
            this.Y = x * constDefine.BENCHMARK;
            V_Picture = picture;
            this.Arrive = arrive;
        }
        public int endX;
        public int endY;
        public int startX;
        public int startY;

        /// <summary>
        /// 重绘函数
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            if (route.Count == 0)
            {
                g.DrawImage(this.V_Picture, this.X, this.Y);
            }
            else
            {
                g.DrawImage(this.V_Picture, route[routeIndex].y * constDefine.BENCHMARK + constDefine.BEGIN_X, route[routeIndex].x * constDefine.BENCHMARK);

            }
        }
        
        public void Vehicle_Move(ElecMap Elc)
        {
            if (route.Count > 0)
            {
                if (routeIndex < this.route.Count - 1)
                {
                    routeIndex++;
                }
                else
                {
                    this.Arrive = true;

                    //if ((this.Y / constDefine.BENCHMARK) == this.endX && (this.X / constDefine.BENCHMARK) ==this.endY)
                    //{
                    //    Elc.TempMapNode[this.endX, this.endY].oth = Elc.mapnode[this.endX, this.endY].oth;
                    //}
                }
            }
            
        }

        /// <summary>
        ///小车移动函数，小车的横纵坐标每改变一次，窗体就执行重绘函数，就实现了小车的移动
        /// </summary>
        /// <param name="Elc"></param>
        public int SearchRoute(ElecMap Elc, int startX, int startY, int endX, int endY)//04022
        {
            AstarSearch astartSearch = new AstarSearch();
            Elc.mapnode[startX, startY].nodeCanUsed = true;//小车自己所在的地方解除占用
            Elc.TempMapNode[startX, startY].nodeCanUsed = true;//小车自己所在的地方解除占用
            this.route = astartSearch.search(Elc, Elc.Width, Elc.Height, startX, startY, endX, endY);
            routeIndex = 0;
            if (this.route == null)
            {
                this.vehical_state = v_state.cannotToDestination;
            }
            for (int ii = 0; ii < this.route.Count;ii++ )
            {
                this.cost = this.cost + 1;
                

                // Elc.mapnode[(int)this.route[ii].X, (int)this.route[ii].Y].vehiclePriority = new List<int>();
               // Elc.mapnode[(int)this.route[ii].Height, (int)this.route[ii].Width].vehiclePriority.Add(this.v_num);
               
            }
            //解除节点的占用
            for (int p = 0; p < Elc.heightNum; p++)
            {
                for (int q = 0; q < Elc.widthNum; q++)
                {
                    Elc.mapnode[p, q].nodeCanUsed = true;
                    Elc.TempMapNode[p, q].nodeCanUsed = true;//小车自己所在的地方解除占用
                }
            }
            return this.route.Count;

        }
        public List<myPoint>  route;//起点到终点的路线
        public float cost;   //完成任务需要的花费

        public int changeX(int X)
        {
            return (X - constDefine.BEGIN_X) / constDefine.BENCHMARK;
        }
        public int changeY(int Y)
        {
            return Y / constDefine.BENCHMARK;
        }

        //无参构造函数
        public Vehicle() { }
    }
}
