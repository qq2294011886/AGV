using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


namespace Astar
{
     static class constDefine
    {
        public static int BLANK_X = 5;                                   //最左侧空白处图例开始的横坐标
        public static int BLANK_Y = 350;                              //最左侧空白处图例开始的纵坐标
        public static int SPACE = 40;                                 //每个图例的间距

        public static int WIDTH;
        public static int HEIGHT;
        public static int WIDTHPANEL2=0;
        public static int HEIGHTPANEL2 = 800;
        public static int BENCHMARK = 25;
        public static int BEGIN_X = 0;
        public static int VEHICL_COUNT = 2;

        public static int PANEL_X = 100;

        public static int Form_Height;                           //框体的长度
        public static int Form_Width;                            //框体的宽度

        public static int STOP_TIME = 3; //设置等待时长 0422 

    }
    //小车状态
     enum v_state { normal, needCharge, breakdown, cannotToDestination };
}
