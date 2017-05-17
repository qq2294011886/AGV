using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using Astar;
using AGV_V1._0.Properties;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Xml.Linq;
using System.IO;
//using GMap.NET.WindowsForms;
//using GMap.NET;
//using GMap.NET.MapProviders;

namespace AGV_V1._0
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            InitialGame();
        }

////////////////////////////////////////////变量定义///////////////////////////////////////////

        public Image img_Belt;                    //传送带
        public Image img_Mid;                     //中间隔带
        public Image img_Road;                    //道路
        public Image img_Destination;             //投送处
        public Image img_ChargeStation;           //充电区
        public Image img_Obstacle;                //障碍物
        public Image img_Scanner;                 //扫描仪
        public Image img_Icon;                    //图标
        public Image img_Alter;                   //替换
        public Image img_White;                   //白色图片，当某个节点图图片为空时，显示与北京背景相同颜色的白色
        public Image img_Png;                     //透明图片，当用户想要使某个节点消失时，点击设置
        public Image img_Flag;                    //点中目的地插小旗
        public Image img_Display;                //被选中为目的地时的状态
        public Image img_Orange;                //被选中为目的地时的状态
        public Image img_Yellow;

        //新建两个全局对象  小车、电子地图
        Vehicle[] vehicle;

        static ElecMap Elc;

        //要替换图片的名字
        String PicString;

        //点击图片放置的图片和类型
        Image SetImage;
        bool SetType;

        //小车是否可以启动的标志位,true 可以启动
        bool Vehicle_Start;

        //是否可以更改物体的标志位，true 可以更改
        bool Object_Change;

        //目的地按钮的标志位，true 
        bool Destination_Get;

        //AGV小车按钮的标志位
        bool Vehicle_Get;

        //AGV小车添加按钮
        bool AGV_Add;

        //控制当系统停止运行的时候防止控制点击小车和目的地的开始按钮工作
        bool CanStart;

        //小车和目的地是否配套，防止只有目的地而没有小车或者只有小车而没有目的地
        bool Des;
        bool Veh;

        //用来记录点击的目的地的坐标和点击的小车的下标
        static int Set_Destination_X;
        static int Set_Destination_Y;
        static int Vehicle_Index;

        Stack<float> Picture_Length;   //堆栈->存放大缩小的图片
        Stack<int> Panel_Width;       //堆栈->用来存panel的宽度
        Stack<int> Panel_Height;      //堆栈->用来存panel的长度

        //控制缩放比例的变量
        float small_number;

        //用来存panel的宽度和长度
        int iWidth, iHeight;

        private Bitmap surface;
        private Graphics g;

        //初始化点击设置长宽按钮弹出的  窗体
        Form F_HegWethBech;

        //点击显示小车路径时弹出的文本框
        Form F_VehicleRoute;
        Label L_VehicleRoute;

//////////////////////////////设置按钮下的变量///////////////////////////////////

        Panel P_TopPanel;      //最上面的选择条
        Panel P_HWForm; //下面的设置框体长宽的panel
        Panel P_HWMap;  //下面的设置地图长款的panel
        Panel P_BottomPanel;

        Button B_HWForm;    //框体的长宽按钮
        Button B_HWMap;     //地图长宽按钮
        Button B_OKBotton;  //最底部的确认按钮，设置框体长宽和地图长宽后的确认
        Button B_CANCELBotton; //最底部的取消按钮，设置框体长宽和地图长宽后的取消

        //初始化窗体上的长度、宽度、基准的输入框
        TextBox T_HegForm;
        TextBox T_WethForm;
        TextBox T_Bech;
        TextBox T_HegMap;
        TextBox T_WethMap;

        //初始化输入框左侧的标签
        Label L_HegForm;
        Label L_WethForm;
        Label L_Bech;
        Label L_HegMap;
        Label L_WethMap;


///////////////////////////////////////////////变量定义完毕//////////////////////////////////////////////


        /// <summary>
        /// 初始化游戏
        /// </summary>
        private void InitialGame()
        {
            InitVariable();    //初始化变量
            InitialXml();      //初始化XML配置文件
            InitialElc();      //初始化电子地图
            InitialVehicle();   //初始化AGV小车
            InitStack();       //初始化堆栈
        }

        /// <summary>
        /// 初始化变量
        /// </summary>
        public void InitVariable()
        {
            img_Belt = Resources.Belt;                    //传送带
            img_Mid = Resources.Mid;                      //中间隔带
            img_Road = Resources.Road;                    //道路
            img_Destination = Resources.Destination;      //投送处
            img_ChargeStation = Resources.ChargeStation;  //充电区
            img_Obstacle = Resources.Obstacle;            //障碍物
            img_Scanner = Resources.Scanner;              //扫描仪
            img_Alter = Resources.Alter;                  //替换
            img_Png = Resources.Png;                      //透明按钮
            img_White = Resources.White;                  //白色图片
            img_Flag = Resources.Flag;                    //小旗
            img_Display = Resources.Display;
            img_Orange = Resources.Vehicle_Orange;
            img_Yellow = Resources.Vehicle_Yellow;


            Vehicle_Start = false;

            Object_Change = false;

            Destination_Get = false;

            Vehicle_Get = false;

            AGV_Add = false;

            CanStart = false;

            Des = false;
            Veh = false;

            surface = null;
            g = null;

            F_HegWethBech = new Form();

            T_HegForm = new TextBox();
            T_WethForm = new TextBox();
            T_Bech = new TextBox();

            L_HegForm = new Label();
            L_WethForm = new Label();
            L_Bech = new Label();

            F_VehicleRoute = new Form();
            L_VehicleRoute = new Label();

            P_TopPanel = new Panel();
            P_HWForm = new Panel();
            P_HWMap = new Panel();
            P_BottomPanel = new Panel();

            B_HWForm = new Button();
            B_HWMap = new Button();
            B_OKBotton = new Button();
            B_CANCELBotton = new Button();

            T_HegMap = new TextBox();
            T_WethMap = new TextBox();

            L_HegMap = new Label();
            L_WethMap = new Label();

            B_OKBotton.Click += new System.EventHandler(this.B_OKBotton_Click);
        }

        /// <summary>
        /// 初始化XML配置文件
        /// </summary>
        public void InitialXml()
        {
            //配置文件的路径，此相对路径是相对于.exe文件的路径
            string path = "../../XMLFile1.xml";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            //获取根节点
            XmlNode root = xmlDoc.SelectSingleNode("config");

            //获取关键字是"WIDTH"的节点的值，并将其转化成int类型
            XmlNode xn = root.SelectSingleNode("WIDTH");
            constDefine.WIDTH = Convert.ToInt32(xn.InnerText);

            //获取关键字是"HEIGHT"的节点的值，并将其转化成int类型
            xn = root.SelectSingleNode("HEIGHT");
            constDefine.HEIGHT = Convert.ToInt32(xn.InnerText);

            //获取关键字是"VEHICL_COUNT"的节点的值，并将其转化成int类型
            xn = root.SelectSingleNode("VEHICL_COUNT");
            constDefine.VEHICL_COUNT = Convert.ToInt32(xn.InnerText);

            //获取关键字是"Form_Height"的节点的值，并将其转化成int类型
            xn = root.SelectSingleNode("Form_Height");
            constDefine.Form_Height = Convert.ToInt32(xn.InnerText);

            //获取关键字是"Form_Width"的节点的值，并将其转化成int类型
            xn = root.SelectSingleNode("Form_Width");
            constDefine.Form_Width = Convert.ToInt32(xn.InnerText);

        }

        public void InitialElc()
        {

            Elc = new ElecMap(constDefine.WIDTH, constDefine.HEIGHT);

            Elc.heightNum = constDefine.HEIGHT / constDefine.BENCHMARK;
            Elc.widthNum = (constDefine.WIDTH - constDefine.BLANK_X) / constDefine.BENCHMARK;

            Elc.mapnode = new MapNode[Elc.heightNum, Elc.widthNum];
            Elc.TempMapNode = new MapNode[Elc.heightNum, Elc.widthNum];

            //设置滚动条滚动的区域
            this.AutoScrollMinSize = new Size(constDefine.WIDTH + constDefine.BEGIN_X, constDefine.HEIGHT);

            //初始化地图位置            
            Elc.SetObject();    //初始化电子地图，同时把电子地图中的物体都摆放好

            //设置pictureBox的尺寸和位置
            pic.Location = Point.Empty;
            pic.ClientSize = new System.Drawing.Size(constDefine.WIDTH, constDefine.HEIGHT);
            surface = new Bitmap(constDefine.WIDTH, constDefine.HEIGHT);
            g = Graphics.FromImage(surface);

            //给点击图片放置的图片和类型赋值，每点击图片默认赋值道路图片
            SetImage = img_Road;
            SetType = true;

            //设置panel的尺寸
            panel1.ClientSize = new System.Drawing.Size(constDefine.WIDTH, constDefine.HEIGHT);
            panel2.ClientSize = new System.Drawing.Size(constDefine.PANEL_X, constDefine.HEIGHTPANEL2);

            Add_Panel2();

            //将pictureBox加入到panel上
            pic.Image = surface;
            panel1.Controls.Add(pic);

            this.ClientSize = new System.Drawing.Size(constDefine.Form_Width, constDefine.Form_Height);
        }

        /// <summary>
        /// 初始化小车
        /// </summary>
        public void InitialVehicle()
        {
           //初始化小车位置
            vehicle = new Vehicle[1000];

            //AGV小车的起始坐标
            int startX;
            int startY;

            //AGV小车横纵坐标在XML文件中的关键字
            string Str_H;
            string Str_Z;

            for (int i = 0; i < constDefine.VEHICL_COUNT; i++)
            {
               
                //配置文件的路径，此相对路径是相对于.exe文件的路径
                string path = "../../XMLFile1.xml";

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);

                //获取根节点
                XmlNode root = xmlDoc.SelectSingleNode("config");

                Str_Z = "agv" + (i+1).ToString() + "-Z";

                Str_H = "agv" + (i+1).ToString() + "-H";

                //获取关键字是"WIDTH"的节点的值，并将其转化成int类型
                XmlNode xn = root.SelectSingleNode(Str_H);
                startY = Convert.ToInt32(xn.InnerText);

                xn = root.SelectSingleNode(Str_Z);
                startX = Convert.ToInt32(xn.InnerText);
                
                vehicle[i] = new Vehicle();

                vehicle[i] = new Vehicle(startX, startY, vehicle[i].img_Vehicle[5], false);

                vehicle[i].endX = 0;
                vehicle[i].endY = 0;

                //Elc.mapnode[startX, startY].nodeCanUsed = false;
                //Elc.TempMapNode[startX, startY].nodeCanUsed = false;

                vehicle[i].endX = vehicle[i].startX;
                vehicle[i].endY = vehicle[i].startY;
            }
            //把小车所在的节点设为占用状态
            VehicleOcuppyNode();

            //搜索路径 
            for (int i = 0; i < constDefine.VEHICL_COUNT; i++)
            {
                vehicle[i].SearchRoute(Elc, vehicle[i].startX, vehicle[i].startY, vehicle[i].endX, vehicle[i].endY);
            }

            //检测冲突的节点，重新规划路线
            CheckeConflictNode();
        }

        /// <summary>
        /// 初始化堆栈，用来储存放大缩小的各值
        /// </summary>
        public void InitStack()
        {
            small_number = 1;                       //缩放比例的初始值
            Picture_Length = new Stack<float>();    //每一个节点图片的堆栈初始化
            Panel_Width = new Stack<int>();         //panel宽度的堆栈初始化
            Panel_Height = new Stack<int>();        //panel高度的堆栈初始化
            Picture_Length.Push(small_number);
            iWidth = constDefine.WIDTH;             //panel宽度的控制变量
            iHeight = constDefine.HEIGHT;           //panel高度的控制变量
            Panel_Width.Push(iWidth);
            Panel_Height.Push(iHeight);
        }

        /// <summary>
        /// 窗体重绘时执行的操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Draw(g);
            timer1.Start();
        }

        /// <summary>
        /// 定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Vehicle_Start)
            {
                for (int i = 0; i < constDefine.VEHICL_COUNT; i++)
                {
                    vehicle[i].Vehicle_Move(Elc);

                    //if ((vehicle[i].Y / constDefine.BENCHMARK) == vehicle[i].endX && (vehicle[i].X / constDefine.BENCHMARK) == vehicle[i].endY)
                    //{
                    //    Elc.TempMapNode[vehicle[i].endX, vehicle[i].endY].oth = Elc.mapnode[vehicle[i].endX, vehicle[i].endY].oth;
                    //}
                   
                }
            }
            //对窗体进行更新
            this.Refresh();
        }

        /// <summary>
        /// 绘制电子地图
        /// </summary>
        /// <param name="e"></param>
        public void Draw(Graphics g)
        {
            // Graphics g = e.Graphics;
            //绘制地图            
            for (int i = 0; i < Elc.heightNum; i++)
            {
                for (int j = 0; j < Elc.widthNum; j++)
                {
                    if (Elc.TempMapNode[i, j].oth == null)
                    {
                        Elc.TempMapNode[i, j].oth = img_White;
                    }
                        g.DrawImage(Elc.TempMapNode[i, j].oth, Elc.TempMapNode[i, j].x, Elc.TempMapNode[i, j].y);
                }
            }
           
            //绘制小车
            int count = Elc.heightNum;
            if (Elc.heightNum > constDefine.VEHICL_COUNT)
            {
                count = constDefine.VEHICL_COUNT;
            }
           
            for (int i = 0; i < count; i++)
            {
                vehicle[i].Draw(g,Elc);
                if(vehicle[i].route.Count-1==vehicle[i].routeIndex)
                {
                    Elc.TempMapNode[vehicle[i].endX, vehicle[i].endY].oth = Elc.mapnode[vehicle[i].endX, vehicle[i].endY].oth;
                }
            }
        }

        /// <summary>
        /// 给panel2上面加入按钮图例等
        /// </summary>
        public void Add_Panel2()
        {
            //指定目的地、指定小车、开始按钮
            panel2.Controls.Add(button4);
            panel2.Controls.Add(button12);
            panel2.Controls.Add(button15);

            //将图例按钮加入到panel2中
            panel2.Controls.Add(button5);
            panel2.Controls.Add(button6);
            panel2.Controls.Add(button7);
            panel2.Controls.Add(button8);
            panel2.Controls.Add(button9);
            panel2.Controls.Add(button10);
            panel2.Controls.Add(button11);
            panel2.Controls.Add(button1);
            panel2.Controls.Add(button2);


            //将左侧的label加入到左侧的panel2中
            panel2.Controls.Add(label1);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(label5);
            panel2.Controls.Add(label6);
            panel2.Controls.Add(label7);
            panel2.Controls.Add(label8);
            panel2.Controls.Add(label9);
        }

        /// <summary>
        /// 把小车所在的节点设为占用状态
        /// </summary>
        public void VehicleOcuppyNode()
        {
            for (int p = 0; p < Elc.heightNum; p++)
            {
                for (int q = 0; q < Elc.widthNum; q++)
                {
                    Elc.TempMapNode[p, q].nodeCanUsed = true;
                    Elc.mapnode[p, q].nodeCanUsed = true;
                }
            }
            int count = constDefine.VEHICL_COUNT;
            for (int i = 0; i < count; i++)
            {
                Elc.TempMapNode[changeY(vehicle[i].Y), changeX(vehicle[i].X)].nodeCanUsed = false;
                Elc.mapnode[changeY(vehicle[i].Y), changeX(vehicle[i].X)].nodeCanUsed = false;
            }

            //bool a1 = Elc.TempMapNode[5, 5].nodeCanUsed;
            //bool a2 = Elc.TempMapNode[6, 5].nodeCanUsed;
            //bool a3 = Elc.TempMapNode[7, 5].nodeCanUsed;
            //bool a4 = Elc.TempMapNode[8, 5].nodeCanUsed;
            //bool a5 = Elc.TempMapNode[9, 5].nodeCanUsed;
            //bool a6 = Elc.TempMapNode[10, 5].nodeCanUsed;
            //bool a7 = Elc.TempMapNode[11, 5].nodeCanUsed;
            //bool a8 = Elc.TempMapNode[12, 5].nodeCanUsed;
            //bool a9 = Elc.TempMapNode[13, 5].nodeCanUsed;
            //bool a10 = Elc.TempMapNode[14, 5].nodeCanUsed;

        }
        /// <summary>
        /// //检测冲突的节点，重新规划路线
        /// </summary>
        public void CheckeConflictNode()
        {
            //根据cost把小车从小到大排序
            SortRoute();
            VehicleOcuppyNode();
            int count = constDefine.VEHICL_COUNT;
            for (int i = 0; i < count - 1; i++)
            {
                Boolean flag = false;
                for (int j = 0; j <= i; j++)
                {
                    if (vehicle[i].vehical_state != v_state.normal || vehicle[j].vehical_state != v_state.normal)
                    {
                        continue;
                    }
                    for (int k = 0; k < vehicle[i + 1].route.Count; k++)
                    {
                        if (k <=vehicle[j].route.Count - 1)
                        {
                            if ((vehicle[j].route[k].x == vehicle[i + 1].route[k].x) && (vehicle[j].route[k].y == vehicle[i + 1].route[k].y))
                            {
                                Elc.TempMapNode[(int)vehicle[j].route[k].x, (int)vehicle[j].route[k].y].nodeCanUsed = false;
                                Elc.mapnode[(int)vehicle[j].route[k].x, (int)vehicle[j].route[k].y].nodeCanUsed = false;
                                flag = true;
                            }
                            //else
                            //{
                            //    Elc.TempMapNode[(int)vehicle[j].route[k].x, (int)vehicle[j].route[k].y].nodeCanUsed = true;
                            //    Elc.mapnode[(int)vehicle[j].route[k].x, (int)vehicle[j].route[k].y].nodeCanUsed = true;
                            //}
                            if (k > 0)
                            {
                                if ((vehicle[j].route[k - 1].x == vehicle[i + 1].route[k].x) && (vehicle[j].route[k - 1].y == vehicle[i + 1].route[k].y))
                                {
                                    Elc.TempMapNode[(int)vehicle[j].route[k].x, (int)vehicle[j].route[k].y].nodeCanUsed = false;
                                    Elc.mapnode[(int)vehicle[j].route[k].x, (int)vehicle[j].route[k].y].nodeCanUsed = false;
                                    flag = true;
                                }
                                //else
                                //{
                                //    Elc.TempMapNode[(int)vehicle[j].route[k].x, (int)vehicle[j].route[k].y].nodeCanUsed = true;
                                //    Elc.mapnode[(int)vehicle[j].route[k].x, (int)vehicle[j].route[k].y].nodeCanUsed = true;
                                //}
                            }
                        }
                        else
                        {
                            if ((vehicle[j].route[vehicle[j].route.Count - 1].y == vehicle[i + 1].route[k].y) && (vehicle[j].route[vehicle[j].route.Count - 1].x == vehicle[i + 1].route[k].x))
                            {
                                Elc.TempMapNode[(int)vehicle[j].route[vehicle[j].route.Count - 1].x, (int)vehicle[j].route[vehicle[j].route.Count - 1].y].nodeCanUsed = false;
                                Elc.mapnode[(int)vehicle[j].route[vehicle[j].route.Count - 1].x, (int)vehicle[j].route[vehicle[j].route.Count - 1].y].nodeCanUsed = false;
                                flag = true;
                            }
                            //else
                            //{
                            //    Elc.TempMapNode[(int)vehicle[j].route[vehicle[j].route.Count - 1].x, (int)vehicle[j].route[vehicle[j].route.Count - 1].y].nodeCanUsed = true;
                            //    Elc.mapnode[(int)vehicle[j].route[vehicle[j].route.Count - 1].x, (int)vehicle[j].route[vehicle[j].route.Count - 1].y].nodeCanUsed = true;
                            //}

                        }
                    }
                    if (flag == true)
                    {
                        vehicle[i + 1].SearchRoute(Elc, vehicle[i + 1].startX, vehicle[i + 1].startY, vehicle[i + 1].endX, vehicle[i + 1].endY);
                    }
                }

            }
        }
        /// <summary>
        /// //根据cost把小车从大到小排序
        /// </summary>
        public void SortRoute()
        {
            int count = constDefine.VEHICL_COUNT;
            for (int i = 0; i < count; i++)
            {
                Vehicle temp = vehicle[i];
                for (int j = i; j < count; j++)
                {
                    if (vehicle[i].cost > vehicle[j].cost)
                    {
                        temp = vehicle[j];
                        vehicle[j] = vehicle[i];
                        vehicle[i] = temp;
                    }
                }
                // vehicleSorted[i] = vehicle[index];
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //让控件不闪烁
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);


            StringBuilder sb1 = new StringBuilder("route:");


            for (int w = 0; w < vehicle[1].route.Count; w++)
            {
                //sb1.Append("[" + vehicle[1].route[w].Height + "," + vehicle[1].route[w].Width + "]->");
            }
            //label9.Text = sb1.ToString();


            StringBuilder sb = new StringBuilder();
            int[,] mapstring = new int[Elc.widthNum, Elc.heightNum];
            mapstring = AstarSearch.mapString();

            /*for (int i = 0; i < Elc.heightNum - 1; i++)
            {
                for (int j = 0; j < Elc.widthNum - 1; j++)
                {

                    Boolean flag = false;
                    for (int k = 0; k < vehicle[1].route.Count; k++)
                    {
                        if ((int)vehicle[1].route[k].Height == i && (int)vehicle[1].route[k].Width == j)
                        {
                            sb.Append("* ");
                            flag = true;
                            break;
                        }
                    }
                    if (flag == false)
                    {
                        for (int k = 0; k < vehicle[0].route.Count; k++)
                        {
                            if ((int)vehicle[0].route[k].Height == i && (int)vehicle[0].route[k].Width == j)
                            {
                                sb.Append("# ");
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (flag == false)
                    {
                        if (mapstring[i, j] == 0)
                            sb.Append("0 ");
                        else
                            sb.Append("1 ");
                    }


                }
                sb.Append("\n\t");
            }*/
            //label8.Text = sb.ToString();


        }

        /// <summary>
        /// 启动按钮触发的操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartToolStripMenuItem.BackColor = System.Drawing.Color.Chartreuse;
            PauseToolStripMenuItem.BackColor = System.Drawing.Color.White;

            Vehicle_Start = true;
            CanStart = true;
        }

        /// <summary>
        /// 停止按钮触发的操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartToolStripMenuItem.BackColor = System.Drawing.Color.White;
            PauseToolStripMenuItem.BackColor = System.Drawing.Color.Chartreuse;

            Vehicle_Start = false;
            CanStart = false;
        }
        /// <summary>
        /// 放大键触发的函数
        /// Stack没有获取栈顶元素的函数，所以先弹出栈顶元素，然后弹出第二个元素并获取，然后将第二个元素压入栈
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void enlargeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //缩放比 
            small_number = small_number * 0.95f;

            //每次放大时将缩放比压入栈
            Picture_Length.Push(small_number);

            iWidth = (int)(iWidth / 0.95) + 1;
            Panel_Width.Push(iWidth);

            iHeight = (int)(iHeight / 0.95) + 1;
            Panel_Height.Push(iHeight);


            pic.Image = GetSmall(surface, small_number);

            //根据缩放比改变panel和pictureBox的大小
            this.panel1.Size = new System.Drawing.Size(iWidth, iHeight);
            this.pic.Size = new System.Drawing.Size(iWidth, iHeight);
        }

        /// <summary>
        /// 缩小键触发的函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reduceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Picture_Length.Count > 1)
            {

                //每次缩小时将缩放比弹出
                Picture_Length.Pop();
                small_number = Picture_Length.Pop();

                Picture_Length.Push(small_number);
                pic.Image = GetSmall(surface, (double)small_number);

                Panel_Width.Pop();
                iWidth = Panel_Width.Pop();
                Panel_Width.Push(iWidth);

                Panel_Height.Pop();
                iHeight = Panel_Height.Pop();
                Panel_Height.Push(iHeight);


                //根据缩放比改变panel和pictureBox的大小
                this.panel1.Size = new System.Drawing.Size(iWidth, iHeight);
                this.pic.Size = new System.Drawing.Size(iWidth, iHeight);
            }
        }

        /// <summary>
        /// 获取改变后的图片，放大缩小功能使用
        /// </summary>
        /// <param name="bm">要缩小的图片</param>
        /// <param name="times">要缩小的倍数</param>
        /// <returns></returns>
        private Bitmap GetSmall(Bitmap bm, double times)
        {
            int nowWidth = (int)(bm.Width / times);
            int nowHeight = (int)(bm.Height / times);
            Bitmap newbm = new Bitmap(nowWidth, nowHeight);//新建一个放大后大小的图片

            if (times >= 1 && times <= 1.1)
            {
                newbm = bm;
            }
            else
            {
                Graphics g = Graphics.FromImage(newbm);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(bm, new Rectangle(0, 0, nowWidth, nowHeight), new Rectangle(0, 0, bm.Width, bm.Height), GraphicsUnit.Pixel);
                g.Dispose();
            }
            return newbm;
        }

        public int changeX(int X)
        {
            return (X - constDefine.BEGIN_X) / constDefine.BENCHMARK;
        }
        public int changeY(int Y)
        {
            return Y / constDefine.BENCHMARK;
        }

        /// <summary>
        /// panel1重绘时执行的函数，右边的panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
           
        }

        /// <summary>
        /// panel2重绘时执行的函数，左边的panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
   
        }

        /// <summary>
        /// 点击右边的pictureBox触发的函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pic_MouseClick(object sender, MouseEventArgs e)
        {
            Point point = e.Location;

            //测试点击点的坐标
            string X = point.X.ToString();
            //string Y = point.Y.ToString();
            //MessageBox.Show(point.ToString(), X + Y);
            int WidthNum, HeightNum;

            string tempstring;

            //获得鼠标点击的位置的下标，并且改变原图中的物体类型
            WidthNum = point.X / constDefine.BENCHMARK;
            HeightNum = point.Y / constDefine.BENCHMARK;
            if (Object_Change == true)
            {
                string path = "../../XMLFile1.xml";
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNode root = xmlDoc.SelectSingleNode("config");//查找要修改的节点

                tempstring = "data";
                tempstring = tempstring + (HeightNum + 1).ToString() + "-" + (WidthNum + 1).ToString();

                XmlNode xn = root.SelectSingleNode(tempstring);
                if(xn==null)
                {
                    XmlElement xe = xmlDoc.CreateElement(tempstring);//创建一个节点   
                    xe.InnerText = PicString;
                    root.AppendChild(xe);//添加到<bookstore>节点中   
                    xmlDoc.Save("../../XMLFile1.xml");

                    Elc.TempMapNode[HeightNum, WidthNum].oth = SetImage;
                    Elc.TempMapNode[HeightNum, WidthNum].Node_Type = SetType;
                    Elc.mapnode[HeightNum, WidthNum].oth = SetImage;
                    Elc.mapnode[HeightNum, WidthNum].Node_Type = SetType;
                }
                else
                {
                    XmlElement xe = (XmlElement)xn;
                    xe.InnerText = PicString;
                    xmlDoc.Save(path);

                    Elc.TempMapNode[HeightNum, WidthNum].oth = SetImage;
                    Elc.TempMapNode[HeightNum, WidthNum].Node_Type = SetType;
                    Elc.mapnode[HeightNum, WidthNum].oth = SetImage;
                    Elc.mapnode[HeightNum, WidthNum].Node_Type = SetType;
                }   
            }

            //查找点击的是哪一个小车
            if (Vehicle_Get == true)
            {   
                int i;
                for ( i= 0; i < constDefine.VEHICL_COUNT; i++)
                {
                    if (vehicle[i].startX == HeightNum && vehicle[i].startY == WidthNum)
                    {
                        Vehicle_Index = i;
                        vehicle[Vehicle_Index].V_Picture = img_Orange;
                        break;
                    }
                }
                if(i<vehicle.Length)
                {
                    Veh = true;
                }
                Vehicle_Get = false;
            }

            //将点击的目的地的坐标记录下来
            if (Destination_Get == true&&CanStart==true)
            {
                vehicle[Vehicle_Index].V_Picture = img_Yellow;
                Set_Destination_X = WidthNum;
                Set_Destination_Y = HeightNum;

                if (Elc.TempMapNode[HeightNum, WidthNum].Node_Type == false)
                    MessageBox.Show("点击的目的地不可达", "警告！！！");

                else
                {
                    Elc.TempMapNode[HeightNum, WidthNum].oth = Elc.mapnode[HeightNum, WidthNum].another;

                    vehicle[Vehicle_Index].endX = Set_Destination_Y;
                    vehicle[Vehicle_Index].endY = Set_Destination_X;

                    VehicleOcuppyNode();

                    vehicle[Vehicle_Index].SearchRoute(Elc, vehicle[Vehicle_Index].startX, vehicle[Vehicle_Index].startY, vehicle[Vehicle_Index].endX, vehicle[Vehicle_Index].endY);

                    if (vehicle[Vehicle_Index].Arrive == true)
                    {
                        vehicle[Vehicle_Index].startX = vehicle[Vehicle_Index].endX;
                        vehicle[Vehicle_Index].startY = vehicle[Vehicle_Index].endY;
                    }

                    //for (int i = 0; i < constDefine.VEHICL_COUNT; i++)
                    //{
                    //    VehicleOcuppyNode();
                    //    vehicle[i].SearchRoute(Elc, vehicle[i].startX, vehicle[i].startY, vehicle[i].endX, vehicle[i].endY);
                    //    if (vehicle[i].Arrive == true)
                    //    {
                    //        vehicle[i].startX = vehicle[i].endX;
                    //        vehicle[i].startY = vehicle[i].endY;
                    //    }
                    //}
                }
                
                Destination_Get = false;           
            } 

            //如果点击了AGV小车按钮
            if (AGV_Add == true)
            {
                string path = "../../XMLFile1.xml";
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNode root = xmlDoc.SelectSingleNode("config");//查找要修改的节点

                string Str_H;
                string Str_Z;

                constDefine.VEHICL_COUNT = constDefine.VEHICL_COUNT + 1; 

                Str_H = "agv" + (constDefine.VEHICL_COUNT).ToString() + "-H";
                Str_Z = "agv" + (constDefine.VEHICL_COUNT).ToString() + "-Z";

                XmlElement xe = xmlDoc.CreateElement(Str_H);//创建一个节点   
                xe.InnerText = WidthNum.ToString();
                root.AppendChild(xe);//添加到节点中   

                xe = xmlDoc.CreateElement(Str_Z);//创建一个节点   
                xe.InnerText = HeightNum.ToString();
                root.AppendChild(xe);//添加到节点中   

                string s = Convert.ToString(constDefine.VEHICL_COUNT);
                XmlNode xn = root.SelectSingleNode("VEHICL_COUNT");
                xe = (XmlElement)xn;
                xe.InnerText = s;
                xmlDoc.Save(path);

                vehicle[constDefine.VEHICL_COUNT-1] = new Vehicle();

                vehicle[constDefine.VEHICL_COUNT - 1] = new Vehicle(HeightNum, WidthNum, vehicle[constDefine.VEHICL_COUNT - 1].img_Vehicle[5], false);

                vehicle[constDefine.VEHICL_COUNT - 1].endX = vehicle[constDefine.VEHICL_COUNT - 1].startX;
                vehicle[constDefine.VEHICL_COUNT - 1].endY = vehicle[constDefine.VEHICL_COUNT - 1].startY;

                //Elc.mapnode[vehicle[constDefine.VEHICL_COUNT - 1].startX, vehicle[constDefine.VEHICL_COUNT - 1].startY].nodeCanUsed = false;
                //Elc.TempMapNode[vehicle[constDefine.VEHICL_COUNT - 1].startX, vehicle[constDefine.VEHICL_COUNT - 1].startY].nodeCanUsed = false;

                Elc.mapnode[HeightNum, WidthNum].nodeCanUsed = false;
                Elc.TempMapNode[HeightNum, WidthNum].nodeCanUsed = false;

                //把小车所在的节点设为占用状态
                VehicleOcuppyNode();

                vehicle[constDefine.VEHICL_COUNT - 1].SearchRoute(Elc, vehicle[constDefine.VEHICL_COUNT - 1].startX, vehicle[constDefine.VEHICL_COUNT - 1].startY, vehicle[constDefine.VEHICL_COUNT - 1].endX, vehicle[constDefine.VEHICL_COUNT - 1].endY);

                //检测冲突的节点，重新规划路线
                CheckeConflictNode();
            }
        }

        /// <summary>
        /// 点击小车按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click_1(object sender, EventArgs e)
        {
            //当点击小车按钮时，目的地按钮和替换物体按钮失效
            Object_Change = false;  //Unable更改物体
            Destination_Get = false; //Unable点击目的地
            Vehicle_Get = true;    //Enable点击AGV小车
            AGV_Add = false;
        }

        /// <summary>
        /// 点击目的地按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            //当点击目的地按钮时，小车按钮和替换物体按钮失效
            Vehicle_Get = false;
            Object_Change = false;  //Unable更改物体
            Destination_Get = true; //Enable点击目的地
            AGV_Add = false;
        }

        ///// <summary>
        ///// 命令小车全部移动按钮
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void button15_Click(object sender, EventArgs e)
        //{
        //    //如果系统启动按钮打开
        //    if (CanStart == true)
        //    {
        //        VehicleOcuppyNode();

        //        for (int i = 0; i < vehicle.Length; i++)
        //        {
        //            vehicle[i].SearchRoute(Elc, vehicle[i].startX, vehicle[i].startY, vehicle[i].endX, vehicle[i].endY);
        //        }

        //        //检测冲突的节点，重新规划路线
        //        CheckeConflictNode();

        //                //小车到达目的地都，原来的目的地坐标即现在的起始坐标
        //                for (int i = 0; i < vehicle.Length; i++)
        //                {
        //                    if (vehicle[i].Arrive==true)
        //                    {
        //                        vehicle[i].startX = vehicle[i].endX;
        //                        vehicle[i].startY = vehicle[i].endY;
        //                    }         
        //                }
        //    }
            
        //}
        //点击AGV小车按钮，向地图中添加AGV小车
        private void button2_Click(object sender, EventArgs e)
        {
            AGV_Add = true;
            //当点击目的地按钮时，小车按钮和替换物体按钮失效
            Vehicle_Get = false;
            Object_Change = false;  //Unable更改物体
            Destination_Get = false; //Unable点击目的地
        }

        //点击道路图片，改变图片和节点可达性
        private void button5_Click(object sender, EventArgs e)
        {
            //当点击替换的物体图片的按钮时，目的地按钮和小车按钮失效
            Vehicle_Get = false;
            Destination_Get = false;
            Object_Change = true;  //Enable更改物体
            SetImage = img_Road;
            SetType = true;
            PicString = "道路";
            AGV_Add = false;
        }

        //点击投递处按钮，改变图片和节点可达性
        private void button8_Click(object sender, EventArgs e)
        {
            //当点击替换的物体图片的按钮时，目的地按钮和小车按钮失效
            Vehicle_Get = false;
            Destination_Get = false;
            Object_Change = true; //Enable更改物体
            SetImage = img_Destination;
            SetType = false;
            PicString = "投送处";
            AGV_Add = false;
        }

        //点击隔道按钮，改变图片和节点可达性
        private void button6_Click(object sender, EventArgs e)
        {
            //当点击替换的物体图片的按钮时，目的地按钮和小车按钮失效
            Vehicle_Get = false;
            Destination_Get = false;
            Object_Change = true; //Enable更改物体
            SetImage = img_Mid;
            SetType = true;
            PicString = "隔道";
            AGV_Add = false;
        }

        //点击传送带按钮，改变图片和节点可达性
        private void button7_Click(object sender, EventArgs e)
        {
            //当点击替换的物体图片的按钮时，目的地按钮和小车按钮失效
            Vehicle_Get = false;
            Destination_Get = false;
            Object_Change = true;  //Enable更改物体
            SetImage = img_Belt;
            SetType = true;
            PicString = "传送带";
            AGV_Add = false;
        }

        //点击充电区按钮，改变图片和节点可达性
        private void button9_Click(object sender, EventArgs e)
        {
            //当点击替换的物体图片的按钮时，目的地按钮和小车按钮失效
            Vehicle_Get = false;
            Destination_Get = false;
            Object_Change = true;  //Enable更改物体
            SetImage = img_ChargeStation;
            SetType = true;
            PicString = "充电区";
            AGV_Add = false;
        }

        //点击障碍物按钮，改变图片和节点可达性
        private void button10_Click(object sender, EventArgs e)
        {
            //当点击替换的物体图片的按钮时，目的地按钮和小车按钮失效
            Vehicle_Get = false;
            Destination_Get = false;
            Object_Change = true;  //Enable更改物体
            SetImage = img_Obstacle;
            SetType = false;
            PicString = "障碍物";
            AGV_Add = false;
        }

        //点击扫描仪按钮，改变图片和节点可达性
        private void button11_Click(object sender, EventArgs e)
        {
            //当点击替换的物体图片的按钮时，目的地按钮和小车按钮失效
            Vehicle_Get = false;
            Destination_Get = false;
            Object_Change = true;  //Enable更改物体
            SetImage = img_Scanner;
            SetType = true;
            PicString = "扫描仪";
            AGV_Add = false;
        }
        //点击空白按钮，改变图片和节点可达性
        private void button1_Click(object sender, EventArgs e)
        {
            //当点击替换的物体图片的按钮时，目的地按钮和小车按钮失效
            Vehicle_Get = false;
            Destination_Get = false;
            Object_Change = true;  //Enable更改物体
            SetImage = img_White;
            SetType = false;
            PicString = "空白";
            AGV_Add = false;
        }

        /// <summary>
        /// 当鼠标滑过按钮的时候变色功能，下同
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_MouseMove(object sender, MouseEventArgs e)
        {
            button4.BackColor = System.Drawing.Color.DeepSkyBlue;
        }


        /// <summary>
        /// 当鼠标从按钮离开时按钮变色，下同
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_MouseLeave(object sender, EventArgs e)
        {
            button4.BackColor = System.Drawing.Color.PowderBlue;
        }

        private void button12_MouseMove(object sender, MouseEventArgs e)
        {
            button12.BackColor = System.Drawing.Color.DeepSkyBlue;
        }

        private void button12_MouseLeave(object sender, EventArgs e)
        {
            button12.BackColor = System.Drawing.Color.PowderBlue;
        }

        private void button15_MouseMove(object sender, MouseEventArgs e)
        {
            button15.BackColor = System.Drawing.Color.DeepSkyBlue;
        }

        private void button15_MouseLeave(object sender, EventArgs e)
        {
            button15.BackColor = System.Drawing.Color.PowderBlue;
        }

        /// <summary>
        /// 显示小车行驶路径按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowVehicleRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //更改图标
            F_VehicleRoute.Icon = ((System.Drawing.Icon)(Resources.youzheng));

            //设置框体和文本框的大小
            F_VehicleRoute.Size = new System.Drawing.Size(500, 500);
            L_VehicleRoute.Size = new System.Drawing.Size(480, 480);

            //StringBuilder sb1 = new StringBuilder("route:");
            StringBuilder[] sb1 = new StringBuilder[constDefine.VEHICL_COUNT];

            StringBuilder Together = new StringBuilder();

            for (int i = 0; i < constDefine.VEHICL_COUNT; i++)
            {
                sb1[i] = new StringBuilder("第"+(i+1)+"辆车的路径：");
            }


            for (int i = 0; i < constDefine.VEHICL_COUNT;i++ )
            {
                for (int w = 0; w < vehicle[i].route.Count; w++)
                {
                    if(w>=1)
                    {
                        sb1[i].Append("[" + vehicle[i].route[w].Y + "," + vehicle[i].route[w].X + "]");
                        if(w < vehicle[i].route.Count - 1)
                        {
                            sb1[i].Append("->");
                        }
                       
                    }                   
                }
                Together.Append(sb1[i]+"\n\t");
            }
            L_VehicleRoute.Text = Together.ToString();

            //将Label加入到框体中
            F_VehicleRoute.Controls.Add(L_VehicleRoute);

            F_VehicleRoute.ShowDialog();    
        }

        /// <summary>
        /// 设置长宽，包括框体长宽和地图长宽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setHWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //框体的图标
            F_HegWethBech.Icon = ((System.Drawing.Icon)(Resources.youzheng));

            F_HegWethBech.Size = new System.Drawing.Size(350,350);  //设置弹出的框体的尺寸

            ///////////////////////////最上边的panel////////////////////////////
            P_TopPanel.Location = new System.Drawing.Point(10,10);    //设置最上边panel的位置
            this.P_TopPanel.Size = new System.Drawing.Size(340, 30); //设置最上边panel的尺寸
            B_HWForm.Location = new System.Drawing.Point(0,0);
            B_HWForm.BackColor = System.Drawing.Color.Chartreuse;
            B_HWForm.Text = "框体长宽";
            B_HWMap.Location = new System.Drawing.Point(80,0);
            B_HWMap.Text = "地图长宽";
            B_HWMap.BackColor = System.Drawing.Color.PowderBlue;

            B_HWForm.Click += new System.EventHandler(this.B_HWForm_Click);
            B_HWMap.Click += new System.EventHandler(this.B_HWMap_Click);

            P_TopPanel.Controls.Add(B_HWForm);
            P_TopPanel.Controls.Add(B_HWMap);

            ///////////////////////////最下边的panel////////////////////////////
            P_BottomPanel.Location = new System.Drawing.Point(0,210);
            P_BottomPanel.Size = new System.Drawing.Size(350,140);

            B_OKBotton.Location = new System.Drawing.Point(75,10);
            B_CANCELBotton.Location = new System.Drawing.Point(175, 10);

            B_OKBotton.Text = "确认";
            B_CANCELBotton.Text = "取消";

            //B_OKBotton.Click += new System.EventHandler(this.B_OKBotton_Click);
            B_CANCELBotton.Click += new System.EventHandler(this.B_CANCELBotton_Click);

            P_BottomPanel.Controls.Add(B_OKBotton);
            P_BottomPanel.Controls.Add(B_CANCELBotton);

            ///////////////////////////Form长宽的panel///////////////////////////
            P_HWForm.Location = new System.Drawing.Point(0,40);
            P_HWForm.Size = new System.Drawing.Size(350,170);

            //标签的名字
            L_HegForm.Text = "框体长度";
            L_WethForm.Text = "框体宽度";
            L_Bech.Text = "基准";

            //标签的位置
            this.L_HegForm.Location = new System.Drawing.Point(60, 65);
            this.L_WethForm.Location = new System.Drawing.Point(60, 105);
            this.L_Bech.Location = new System.Drawing.Point(60, 125);



            //输入框的位置、长宽
            T_HegForm.SetBounds(160, 60, 90, 60);
            T_WethForm.SetBounds(160, 100, 90, 60);
            T_Bech.SetBounds(120, 120, 90, 60);

            P_HWForm.Controls.Add(T_HegForm);
            P_HWForm.Controls.Add(T_WethForm);
            P_HWForm.Controls.Add(L_HegForm);
            P_HWForm.Controls.Add(L_WethForm);

//////////////////////////////////////////////Map长宽的panel///////////////////////////////////////////
            P_HWMap.Location = new System.Drawing.Point(0,40);
            P_HWMap.Size = new System.Drawing.Size(350,170);

            L_HegMap.Text = "地图长度";
            L_WethMap.Text = "地图宽度";

 
            //标签的位置
            this.L_HegMap.Location = new System.Drawing.Point(60, 65);
            this.L_WethMap.Location = new System.Drawing.Point(60, 105);
           
            //输入框的位置、长宽
            T_HegMap.SetBounds(160, 60, 90, 60);
            T_WethMap.SetBounds(160, 100, 90, 60);
            T_Bech.SetBounds(120, 120, 90, 60);

            P_HWMap.Controls.Add(T_HegMap);
            P_HWMap.Controls.Add(T_WethMap);
            P_HWMap.Controls.Add(L_HegMap);
            P_HWMap.Controls.Add(L_WethMap);
            
////////////////////////////////////////////////////////////////////////////////////////////////////////
            F_HegWethBech.Controls.Add(P_TopPanel);
            F_HegWethBech.Controls.Add(P_HWForm);
            F_HegWethBech.Controls.Add(P_BottomPanel);

            F_HegWethBech.ShowDialog();
        }

        /// <summary>
        /// 设置->设置长宽->框体长宽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void B_HWForm_Click(object sender, EventArgs e)
        {
            B_HWForm.BackColor = System.Drawing.Color.Chartreuse;
            B_HWMap.BackColor = System.Drawing.Color.PowderBlue;
            F_HegWethBech.Controls.Remove(P_HWMap);
            F_HegWethBech.Controls.Add(P_HWForm);
        }

        /// <summary>
        /// 设置->设置长宽->地图长宽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void B_HWMap_Click(object sender, EventArgs e)
        {
            B_HWForm.BackColor = System.Drawing.Color.PowderBlue;
            B_HWMap.BackColor = System.Drawing.Color.Chartreuse;
            F_HegWethBech.Controls.Remove(P_HWForm);
            F_HegWethBech.Controls.Add(P_HWMap);
        }

        /// <summary>
        /// 设置->设置长宽->确认按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void B_OKBotton_Click(object sender, EventArgs e)
        {

            string path = "../../XMLFile1.xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode root = xmlDoc.SelectSingleNode("config");//查找要修改的节点

///////////////////////////////////////框体///////////////////////////////////////////

            //将输入框中的字符串转化成数字，然后赋值给长度、宽度和基准
            int Out_HeighFormt;
            int Out_WidthForm;
            int Out_Bech;

            int.TryParse(T_HegForm.Text, out Out_HeighFormt);
            int.TryParse(T_HegForm.Text, out Out_WidthForm);
            int.TryParse(T_HegForm.Text, out Out_Bech);

            //如果长度或者宽度或者基准的输入为空，就对其不做任何操作
            if (String.IsNullOrEmpty(T_HegForm.Text))
            {
            }
            else
            {
                this.Height = Out_HeighFormt;

                //将用户在text里面设置的框体的长度值传到XML配置文件里面
                string s = Convert.ToString(this.Height);
                XmlNode xn = root.SelectSingleNode("Form_Height");
                XmlElement xe = (XmlElement)xn;
                xe.InnerText = s;
                xmlDoc.Save(path);
            }

            if (String.IsNullOrEmpty(T_WethForm.Text))
            {
            }
            else
            {
                //将用户在text里面设置的框体的宽度值传到XML配置文件里面
                this.Width = Out_WidthForm;
                string s = Convert.ToString(this.Width);
                XmlNode xn = root.SelectSingleNode("Form_Width");
                XmlElement xe = (XmlElement)xn;
                xe.InnerText = s;
                xmlDoc.Save(path);
            }


            if (String.IsNullOrEmpty(T_Bech.Text))
            {
            }
            else
                constDefine.BENCHMARK = Out_Bech;

////////////////////////////////////////地图////////////////////////////////////////////////

            //将输入框中的字符串转化成数字，然后赋值给长度、宽度和基准
            int Out_HeightMap;
            int Out_WeithMap;

            int.TryParse(T_HegMap.Text, out Out_HeightMap);
            int.TryParse(T_WethMap.Text, out Out_WeithMap);
            int.TryParse(T_Bech.Text, out Out_Bech);

            //如果长度或者宽度或者基准的输入为空，就对其不做任何操作
            if (String.IsNullOrEmpty(T_HegMap.Text))
            {
            }
            else
            {
                this.Height = Out_HeightMap;

                //将用户在text里面设置的框体的长度值传到XML配置文件里面
                string s = Convert.ToString(this.Height);
                XmlNode xn = root.SelectSingleNode("HEIGHT");
                XmlElement xe = (XmlElement)xn;
                xe.InnerText = s;
                xmlDoc.Save(path);
            }

            if (String.IsNullOrEmpty(T_WethMap.Text))
            {
            }
            else
            {
                //将用户在text里面设置的框体的宽度值传到XML配置文件里面
                this.Width = Out_WeithMap;
                string s = Convert.ToString(this.Width);
                XmlNode xn = root.SelectSingleNode("WIDTH");
                XmlElement xe = (XmlElement)xn;
                xe.InnerText = s;
                xmlDoc.Save(path);
            }

            //所有的操作完成后，清除输入框内的信息
            //T_HegMap.Clear();
            //T_WethMap.Clear();
            //T_HegForm.Clear();
            //T_WethForm.Clear();

            //关闭框体
            F_HegWethBech.Dispose();

            this.Dispose();
            //System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
         
            Application.Restart();
        }

        /// <summary>
        /// 设置->设置长宽->取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void B_CANCELBotton_Click(object sender, EventArgs e)
        {
            //所有的操作完成后，清除输入框内的信息
            T_HegMap.Clear();
            T_WethMap.Clear();
            T_HegForm.Clear();
            T_WethForm.Clear();
            //关闭框体
            F_HegWethBech.Close();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
        private void pic_Click(object sender, EventArgs e)
        {

        }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        //private GMapOverlay objects = new GMapOverlay("objects"); //放置marker的图层    

        //private void gMapControl1_Load_1(object sender, EventArgs e)
        //{
        //    gMapControl1.Manager.Mode = AccessMode.CacheOnly;
        //    MessageBox.Show("No internet connection avaible, going to CacheOnly mode.", "GMap.NET Demo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    gMapControl1.CacheLocation = Environment.CurrentDirectory + "\\GMapCache\\"; //缓存位置
        //    gMapControl1.MapProvider = GMapProviders.GoogleChinaMap; //google china 地图
        //    gMapControl1.MinZoom = 2;  //最小比例
        //    gMapControl1.MaxZoom = 17; //最大比例
        //    gMapControl1.Zoom = 5;     //当前比例
        //    gMapControl1.ShowCenter = false; //不显示中心十字点
        //    gMapControl1.DragButton = System.Windows.Forms.MouseButtons.Left; //左键拖拽地图
        //    gMapControl1.Position = new PointLatLng(32.064, 118.704); //地图中心位置：南京

        //    gMapControl1.Overlays.Add(objects);

        //}
    }    
}
