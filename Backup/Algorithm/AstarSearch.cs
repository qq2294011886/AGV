using AGV_V1._0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Astar
{

    class AstarSearch
    {

        public const int MaxLength = 500;   //用于优先队列（Open表）的数组
        public int Height = 15;       //地图高度
        public int Width = 20;       //地图宽度

        public const int Reachable = 0;       //可以到达的结点
        public const int Bar = 1;             //障碍物
        public const int Pass = 2;            //需要走的步数
        public const int Source = 3;          //起点
        public const int Destination = 4;     //终点

        public const int Sequential = 0;    //顺序遍历
        public const int NoSolution = 2;    //无解决方案
        public const int Infinity = 0xfffffff;

        public const int East = (1 << 0);
        public const int South_East = (1 << 1);
        public const int South = (1 << 2);
        public const int South_West = (1 << 3);
        public const int West = (1 << 4);
        public const int North_West = (1 << 5);
        public const int North = (1 << 6);
        public const int North_East = (1 << 7);

        static myPoint[] dir = new myPoint[8]{	
        new myPoint( 0, 1 ),   // East
	    new myPoint( 1, 1 ),   // South_East
	    new myPoint( 1, 0),   // South
	    new myPoint(1, -1 ),  // South_West
	    new myPoint( 0, -1 ),  // West
	    new myPoint( -1, -1 ), // North_West
        new myPoint( -1, 0 ),  // North
	    new myPoint( -1, 1)   // North_East
};

        public bool within(int x, int y)
        {
            if ((x >= 0 && y >= 0
                && x < Height && y < Width))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        MapNode[,] graph = null;

        int srcX, srcY, dstX, dstY;    //起始点、终点
        Close[,] close =null;


        // 优先队列基本操作
        public void initOpen(Open q)    //优先队列初始化
        {
            q.length = 0;        // 队内元素数初始为0
        }

        public void push(Open q, Close[,] cls, int x, int y, float g)
        {    //向优先队列（Open表）中添加元素
            Close t;
            int i, mintag;
            cls[x, y].G = g;    //所添加节点的坐标
            cls[x, y].F = cls[x, y].G + cls[x, y].H;
            q.Array[q.length++] = (cls[x, y]);
            mintag = q.length - 1;
            for (i = 0; i < q.length - 1; i++)
            {
                if (q.Array[i].F < q.Array[mintag].F)
                {
                    mintag = i;
                }
            }
            t = q.Array[q.length - 1];
            q.Array[q.length - 1] = q.Array[mintag];
            q.Array[mintag] = t;    //将评价函数值最小节点置于队头
        }

        public Close shift(Open q)
        {
            return q.Array[--q.length];
        }

        // 地图初始化操作
        public void initClose(Close[,] cls, int sx, int sy, int dx, int dy)
        {    // 地图Close表初始化配置
            int i, j;
            for (i = 0; i < Height; i++)
            {
                for (j = 0; j < Width; j++)
                {
                    cls[i, j] = new Close { };
                    cls[i, j].cur = graph[i, j];        // Close表所指节点
                    cls[i, j].vis = !(graph[i, j].Node_Type);        // 是否被访问
                    cls[i, j].from = null;                // 所来节点
                    cls[i, j].G = cls[i, j].F = 0;
                    cls[i, j].H = Math.Abs(dx - i) + Math.Abs(dy - j);    // 评价函数值
                }
            }
            cls[sx, sy].F = cls[sx, sy].H;            //起始点评价初始值
            //    cls[sy,sy].G = 0;                        //移步花费代价值
            cls[dx, dy].G = Infinity;
        }

        public void initGraph(int[,] map, int sx, int sy, int dx, int dy)
        {    //地图发生变化时重新构造地
            int i, j;
            srcX = sx;    //起点X坐标
            srcY = sy;    //起点Y坐标
            dstX = dx;    //终点X坐标
            dstY = dy;    //终点Y坐标

           graph=new MapNode[Height, Width];

            for (i = 0; i < Height; i++)
            {
                for (j = 0; j < Width; j++)
                {
                    graph[i, j] = new MapNode { };
                    graph[i, j].value = map[i, j];
                    graph[i, j].x = i; //地图坐标X
                    graph[i, j].y = j; //地图坐标Y

                    graph[i, j].Node_Type = (graph[i, j].value == Reachable);    // 节点可到达性
                    graph[i, j].sur = 0; //邻接节点个数
                    if (!graph[i, j].Node_Type)
                    {
                        continue;
                    }
                    if (j > 0)
                    {
                        if (graph[i, j - 1].Node_Type)    // left节点可以到达
                        {
                            graph[i, j].sur |= West;
                            graph[i, j - 1].sur |= East;
                        }
                        //if (i > 0)
                        //{
                        //    if (graph[i - 1, j - 1].Node_Type
                        //        && graph[i - 1, j].Node_Type
                        //        && graph[i, j - 1].Node_Type)    // up-left节点可以到达
                        //    {
                        //        graph[i, j].sur |= North_West;
                        //        graph[i - 1, j - 1].sur |= South_East;
                        //    }
                        //}
                    }
                    if (i > 0)
                    {
                        if (graph[i - 1, j].Node_Type)    // up节点可以到达
                        {
                            graph[i, j].sur |= North;
                            graph[i - 1, j].sur |= South;
                        }
                        //if (j < Width - 1)
                        //{
                        //    if (graph[i - 1, j + 1].Node_Type
                        //        && graph[i - 1, j].Node_Type
                        //        && map[i, j + 1] == Reachable) // up-right节点可以到达
                        //    {
                        //        graph[i, j].sur |= North_East;
                        //        graph[i - 1, j + 1].sur |= South_West;
                        //    }
                        //}
                    }
                }
            }
        }

        public int astar()
        {    // A*算法遍历
            //int times = 0;
            int i, curX, curY, surX, surY;
            float surG;
            Open q = new Open(); //Open表
            Close p = new Close();

            close = new Close[Height, Width];

            initOpen(q);
            initClose(close, srcX, srcY, dstX, dstY);
            close[srcX, srcY].vis = true;
            push(q, close, srcX, srcY, 0);

            while (q.length > 0)
            {    //times++;
                p = shift(q);
                curX = p.cur.x;
                curY = p.cur.y;
                if (p.H == 0)
                {
                    return Sequential;
                }
                for (i = 0; i < 8; i++)
                {
                    if ((p.cur.sur & (1 << i)) == 0)
                    {
                        continue;
                    }
                    surX = curX + (int)dir[i].x;
                    surY = curY + (int)dir[i].y;
                    if (!close[surX, surY].vis)
                    {
                        close[surX, surY].vis = true;
                        close[surX, surY].from = p;
                        surG = p.G + (float)(Math.Abs(curX - surX) + Math.Abs(curY - surY));
                        push(q, close, surX, surY, surG);
                    }
                }
            }
            //System.Console.Write("times: %d\n", times);
            return NoSolution; //无结果
        }



        string[] Symbol = new string[5] { "□", "▓", "▽", "☆", "◎" };

        public void printMap()
        {
            int i, j;
            for (i = 0; i < Height; i++)
            {
                for (j = 0; j < Width; j++)
                {
                    System.Console.Write(Symbol[graph[i, j].value]);
                }
                System.Console.WriteLine("");
            }
            System.Console.WriteLine("");
        }
        public Close getShortest()
        {    // 获取最短路径


            int result = astar();
            Close p, t, q = null;
            switch (result)
            {
                case Sequential:  //顺序最近
                    p = (close[dstX, dstY]);
                    while (p != null)    //转置路径
                    {
                        t = p.from;
                        p.from = q;
                        q = p;
                        p = t;
                    }
                    close[srcX, srcY].from = q.from;
                    return (close[srcX, srcY]);
                case NoSolution:
                    return null;
            }
            return null;
        }

        Close start;
        int m = 0;
        //int shortestep;
        public int printShortest(List<myPoint> route)
        {
            Close p;
            int step = 0;

            p = getShortest();
            start = p;
            if (p == null)
            {
                return 0;
            }
            else
            {
                while (p.from != null)
                {
                    graph[p.cur.x, p.cur.y].value = Pass;
                    System.Console.WriteLine("({0}，{1}）→", p.cur.x, p.cur.y);
                    route.Add(new myPoint(p.cur.x, p.cur.y));
                    m++;
                    p = p.from;
                    step++;
                }
                System.Console.WriteLine("→（{0}，{1}）", p.cur.x, p.cur.y);
                route.Add(new myPoint(p.cur.x, p.cur.y));
                m++;
                graph[srcX, srcY].value = Source;
                graph[dstX, dstY].value = Destination;
                return step;
            }
        }

        public void clearMap()
        {    // Clear Map Marks of Steps
            Close p = start;
            while (p != null)
            {
                graph[p.cur.x, p.cur.y].value = Reachable;
                p = p.from;
            }
            graph[srcX, srcY].value = map[srcX, srcY];
            graph[dstX, dstY].value = map[dstX, dstY];
        }

        public void printDepth()
        {
            int i, j;
            for (i = 0; i < Height; i++)
            {
                for (j = 0; j < Width; j++)
                {
                    if (map[i, j] > 0)
                    {
                        System.Console.Write(Symbol[graph[i, j].value]);
                    }
                    else
                    {
                        System.Console.Write(close[i, j].G);
                    }
                }
                System.Console.WriteLine("");
            }
            System.Console.WriteLine("");
        }

        public void printSur()
        {
            int i, j;
            for (i = 0; i < Height; i++)
            {
                for (j = 0; j < Width; j++)
                {
                    System.Console.Write("%02x ", graph[i, j].sur);
                }
                System.Console.WriteLine("");
            }
            System.Console.WriteLine("");
        }

        public void printH()
        {
            int i, j;
            for (i = 0; i < Height; i++)
            {
                for (j = 0; j < Width; j++)
                {
                    System.Console.Write("%02d ", close[i, j].H);
                }
                System.Console.WriteLine("");
            }
            System.Console.WriteLine("");
        }
        public int bfs()
        {
            int times = 0;
            int i, curX, curY, surX, surY;
            int f = 0, r = 1;
            Close p = new Close();
            Close[] q = new Close[Height * Width];
            int w = 0;
            for (int m = 0; m < Height; m++)
            {
                for (int n = 0; n < Width; n++)
                {
                    q[w] = close[m, n];
                    w++;
                }
            }

            initClose(close, srcX, srcY, dstX, dstY);
            close[srcX, srcY].vis = true;

            while (r != f)
            {
                p = q[f];
                f = (f + 1) % MaxLength;
                curX = p.cur.x;
                curY = p.cur.y;
                for (i = 0; i < 8; i++)
                {
                    if ((p.cur.sur & (1 << i)) == 0)
                    {
                        continue;
                    }
                    surX = curX + (int)dir[i].x;
                    surY = curY + (int)dir[i].y;
                    if (!close[surX, surY].vis)
                    {
                        close[surX, surY].from = p;
                        close[surX, surY].vis = true;
                        close[surX, surY].G = p.G + 1;
                        q[r] = close[surX, surY];
                        r = (r + 1) % MaxLength;
                    }
                }
                times++;
            }
            return times;
        }
        static int[,] map = null;
        public void ChangeMap(ElecMap elc, int width, int height)
        {
            //电子地图的长、宽被分割的个数
            Height = elc.heightNum;
            Width = elc.widthNum;
            //Width = width;
            //Height = height;
            map = new int[Height, Width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (elc.TempMapNode[i, j].Node_Type == true&&elc.TempMapNode[i,j].nodeCanUsed==true)
                    {
                        map[i, j] = 0;
                    }
                    else
                    {
                        map[i, j] = 1;
                    }

                }
            }
        }
        public static int[,] mapString()
        {
            return map;
        }
        public List< myPoint> search(ElecMap elc, int width, int height, int firstX, int firstY, int endX, int endY)
        {

            ChangeMap(elc, width, height);  // 转换寻找路径的可达还是不可达
            initGraph(map, firstX, firstY, endX, endY);
            List<myPoint> route = new List<myPoint>();
            printShortest(route);
            

            //printMap();

            //while (true)
            //{
            //    Console.WriteLine("请输入起始地点坐标和目标地点坐标，中间以空格隔开 如1 1 3 4");
            //    string[] values = Console.ReadLine().Split(' ');
            //    srcX = int.Parse(values[0]);
            //    srcY = int.Parse(values[1]);
            //    dstX = int.Parse(values[2]);
            //    dstY = int.Parse(values[3]);
            //    if (within(srcX, srcY) && within(dstX, dstY))
            //    {
            //        if ((shortestep = printShortest()) > 0)
            //        {
            //            Console.WriteLine("从（{0}，{1}）到（{2}，{3}）的最短步数是: {4}",
            //                srcX, srcY, dstX, dstY, shortestep);
            //            printMap();
            //            clearMap();
            //          // int times= bfs();
            //            //printDepth();
            //          //  Console.WriteLine("shortestep={0}, close[dstX, dstY].G={1}",times,close[dstX, dstY].G);
            //          //  Console.WriteLine((shortestep == close[dstX, dstY].G) ? "正确" : "错误");
            //          //  clearMap();
            //        }
            //        else
            //        {
            //            Console.WriteLine("从（{0}，{1}）不可到达（{2}，{3}",
            //                srcX, srcY, dstX, dstY);
            //        }
            //    }
            //    else
            //    {
            //        Console.Write("输入错误！");
            //    }
            //
            return route;
            //  }
        }


    
    }
}
