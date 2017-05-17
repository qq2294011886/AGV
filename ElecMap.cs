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

namespace AGV_V1._0
{
    class ElecMap
    {
        public Image img_Belt = Resources.Belt;
        public Image img_Mid = Resources.Mid;
        public Image img_Road = Resources.Road;
        public Image img_Destination = Resources.Destination;
        public Image img_ChargeStation = Resources.ChargeStation;
        public Image img_Obstacle = Resources.Obstacle;
        public Image img_Scanner = Resources.Scanner;
        public Image img_Display = Resources.Display;


        //电子地图的宽度
        public int Width
        {
            get;
            set;
        }

        //电子地图的长度
        public int Height
        {
            get;
            set;
        }

        //电子地图的长被分割的个数
        public int heightNum
        {
            get;
            set;
        }

        //电子地图的宽被分割的个数
        public int widthNum
        {
            get;
            set;
        }
        //private MapNode[,] mapNode;

        public MapNode[,] mapnode;

        public MapNode[,] TempMapNode;

        public String[,] str;
        

        /// <summary>
        /// 构造函数，初始化电子地图的长度、宽度和每一个小块的基准长度
        /// </summary>
        /// <param name="width">电子地图的宽度</param>
        /// <param name="height">电子地图的长度</param>
        /// <param name="benchmark">每一个小块的基准长度</param>

        public ElecMap(int height, int width )
        {
            this.Width = width;
            this.Height = height;
        }

        //无参构造函数
        public ElecMap() { }

        public void Draw_Node(Graphics g)
        {
            //Graphics g = e.Graphics;
            int point_x = constDefine.BEGIN_X;
            int point_y = 0;
            int i, j;

            for (i = 0; i < heightNum; i++)
            {
                point_x = constDefine.BEGIN_X;
                for (j = 0; j < widthNum; j++)
                {
                    //Elc.mapnode[i, j] = new MapNode(point_x, point_y, Node_number, point_type);
                    TempMapNode[i, j].x = point_x;
                    TempMapNode[i, j].y = point_y;
                    point_x += constDefine.BENCHMARK;
                }
                point_y += constDefine.BENCHMARK;
            }
        }

        /// <summary>
        /// 映射函数  字符串->图片
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public void  Map(string name,MapNode node)
        {
            if (name == "道路")
            {
                node.oth = img_Road;
                node.Node_Type = true;
            }
            if (name == "障碍物")
            {
                node.oth = img_Obstacle;
                node.Node_Type = false;
            }
            if (name == "充电区")
            {
                node.oth = img_ChargeStation;
                node.Node_Type = true;
            }
            if (name == "传送带")
            {
                node.oth = img_Belt;
                node.Node_Type = true;
            }
            if (name == "扫描仪")
            {
                node.oth = img_Scanner;
                node.Node_Type = true;
            }
            if (name == "投送处")
            {
                node.oth=img_Destination ;
                node.Node_Type=false;
            }
            if (name == "隔道")
            {
                node.oth = img_Mid;
                node.Node_Type = true;
            }
        }

        /// <summary>
        /// 初始化电子地图，将所有的object的属性赋值，同时把电子地图中的物体都摆放好
        /// </summary>
        /// <param name="g"></param>

        public void SetObject()
        {
            string path = "../../XMLFile1.xml";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            //获取根节点
            XmlNode root = xmlDoc.SelectSingleNode("config");

            int Node_number = 1;    //节点编号

            //电子地图的长、宽被分割的个数
            heightNum = Height / constDefine.BENCHMARK;
            widthNum = Width / constDefine.BENCHMARK;

            mapnode = new MapNode[heightNum,widthNum];
            TempMapNode =new MapNode [heightNum,widthNum];  
            str=new String[heightNum,widthNum];

            //循环变量
            int i = 0;
            int j = 0;

            //横纵坐标的控制变量
            int point_x, point_y;

            //节点类型
            bool point_type = false;

            point_x = constDefine.BEGIN_X;
            point_y = 0;

            XmlNode xn;
            string Str_Temp;
            string Str_Con;

            for (i = 0; i < heightNum; i++)
            {
                for (j = 0; j < widthNum; j++)
                {
                    Str_Temp = "data";
                    Str_Temp = Str_Temp + (i+1).ToString() +"-"+(j+1).ToString();
                    str[i, j] = Str_Temp;
                }
            }
                for (i = 0; i < heightNum; i++)
                {
                    point_x = constDefine.BEGIN_X;
                    for (j = 0; j < widthNum; j++)
                    {
                        mapnode[i, j] = new MapNode(point_x, point_y, Node_number, point_type);
                        TempMapNode[i, j] = new MapNode(point_x, point_y, Node_number, point_type);
                        mapnode[i, j].another = img_Display;
                        TempMapNode[i, j].another = img_Display;
                        Node_number++;
                        point_x += constDefine.BENCHMARK;
                    }
                    point_y += constDefine.BENCHMARK;
                }
            
            //读XML文件中存的地图
            for (i = 0; i < heightNum; i++)
            {
                point_x = constDefine.BLANK_X;
                for (j = 0; j < widthNum; j++)
                {
                    xn = root.SelectSingleNode(str[i,j]);
                    if (xn == null)
                        break;
                    Str_Con = Convert.ToString(xn.InnerText);
                    Map(Str_Con, mapnode[i, j]);
                    Map(Str_Con, TempMapNode[i, j]);
                }
            }
        }

        /// <summary>
        /// 按比例缩放图片
        /// </summary>
        /// <param name="SourceImage"> 源图片</param>
        /// <param name="TargetWidth">目标宽度</param>
        /// <param name="TargetHeight">目标长度</param>
        /// <returns></returns>
        public Image ChargePicture(Image SourceImage, int TargetWidth, int TargetHeight)
        {
            int IntWidth; //新的图片宽
            int IntHeight; //新的图片高
            try
            {
                System.Drawing.Imaging.ImageFormat format = SourceImage.RawFormat;
                System.Drawing.Bitmap SaveImage = new System.Drawing.Bitmap(TargetWidth,TargetHeight);
                Graphics g = Graphics.FromImage(SaveImage);
                g.Clear(Color.White);

                //计算缩放图片的大小 

                if (SourceImage.Width > TargetWidth && SourceImage.Height <=TargetHeight)//宽度比目的图片宽度大，长度比目的图片长度小
                {
                    IntWidth = TargetWidth;
                    IntHeight = (IntWidth * SourceImage.Height) / SourceImage.Width;
                }
                else if (SourceImage.Width <= TargetWidth && SourceImage.Height >TargetHeight)//宽度比目的图片宽度小，长度比目的图片长度大
                {
                    IntHeight = TargetHeight;
                    IntWidth = (IntHeight * SourceImage.Width) / SourceImage.Height;
                }
                else if (SourceImage.Width <= TargetWidth && SourceImage.Height <=TargetHeight) //长宽比目的图片长宽都小
                {
                    IntHeight = TargetWidth;
                    IntWidth = TargetHeight;
                }
                else//长宽比目的图片的长宽都大
                {
                    IntWidth = TargetWidth;
                    IntHeight = (IntWidth * SourceImage.Height) / SourceImage.Width;
                    if (IntHeight > TargetHeight)//重新计算
                    {
                        IntHeight = TargetHeight;
                        IntWidth = (IntHeight * SourceImage.Width) / SourceImage.Height;
                    }
                }

                g.DrawImage(SourceImage, (TargetWidth - IntWidth) / 2, (TargetHeight -IntHeight) / 2, IntWidth, IntHeight);
                //SourceImage.Dispose();

                return SaveImage;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~ElecMap()
        {
        }

    }
}
