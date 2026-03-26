using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


//吃蛇游戏
//1.创建一个面板，作为游戏的主界面
//2.在面板上创建一个二维数组的标签，作为游戏的地图
//3.创建一个列表，作为蛇的身体，列表中的每个元素都是一个标签
//4.创建一个标签，作为蛇头
//5.创建一个标签，作为食物
//6.创建一个计时器，控制蛇的移动
//7.创建一个计时器，控制食物的生成
//8.创建一个枚举，表示蛇的移动方向
//9.创建一个结构体，表示标签的坐标
//10.创建一个方法，初始化游戏面板
//11.创建一个方法，初始化蛇
//12.创建一个方法，添加蛇的身体
//13.创建一个方法，移除蛇的身体
//14.创建一个方法，移动蛇
//15.创建一个方法，生成食物
//16.创建一个方法，吃食物
//17.创建一个方法，游戏结束
//18.在窗体加载事件中，初始化游戏面板和蛇，并启动计时器
//19.在计时器事件中，移动蛇
//20.在计时器事件中，如果没有食物，生成食物
//21.在键盘事件中，改变蛇的移动方向
//22.在键盘事件中，按空格键暂停或继续游戏
//23.在菜单栏中，添加一个开始游戏的选项，点击后重新初始化游戏面板和蛇，并启动计时器
//24.在游戏结束时，停止计时器，清空面板上的所有控件，显示一个标签，提示游戏结束
//25.在吃食物的方法中，添加判断，如果蛇头的坐标和食物的坐标一样，就吃掉食物，增加蛇的长度，并生成新的食物
//26.在移动蛇的方法中，添加判断，如果蛇头的坐标超出地图的边界，就游戏结束


//注意，行是rows部分，列是cols部分，行是y轴，列是x轴
//head也有tag是因为前面map赋值给了head




namespace eatsnake
{
    public partial class Form1 : Form
    {
        #region 属性
        Label[,] map;//地图，用于索引标签
        int rows = 0;//标签的行数
        int cols = 0;//标签的列数
        int fwidth = 30;//每个格子的宽度
        int fheight = 30;//每个格子的高度
        Color bc = Color.Black;//背景颜色
        #endregion


        #region 蛇属性
        List<Label> snake;//蛇的身体
        Label head;//蛇头
        Color headcolor = Color.Green;//蛇头的颜色
        Color bodycolor = Color.White;//蛇的颜色
        Direction dir;//蛇的移动方向
        #endregion

        #region 食物属性
        Label food;//食物
        Random rand;//随机数生成器
        Color foodcolor = Color.Yellow;//食物的颜色
        bool isfood;//是否有食物
        #endregion


        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;  // 添加这一行！
            this.KeyDown += Form1_KeyDown;  // 确保事件已订阅（如果设计器没自动加的话）

        }

        void Init_GamePenal()
        {
            rows = panel1.Height / fheight;//计算行数
            cols = panel1.Width / fwidth;//计算列数
            map = new Label[rows, cols];//初始化方法
            
            isfood = false;//初始化没有食物

            for (int i = 0; i < rows; i++)//循环行数
            {
                for (int j = 0; j < cols; j++)//循环列数
                {
                    map[i, j] = new Label();
                    map[i, j].Size = new Size(fwidth, fheight);//设置标签大小
                    map[i, j].Location = new Point(j * fwidth, i * fheight);//设置标签位置
                    map[i, j].BackColor = bc;//设置标签背景颜色
                    map[i, j].BorderStyle = BorderStyle.FixedSingle;//设置标签边框样式
                    map[i, j].Tag = new ZB(i, j);//创建坐标结构体对象，保存标签的行列索引
                   

                    panel1.Controls.Add(map[i, j]);//添加到面板

                }

            }


        }

        void Init_Snake()
        {
            snake = new List<Label>();//初始化蛇的身体集合
            ADDSnake(map[5, 0]);//将蛇的第一个身体放在地图的第一个标签上
            ADDSnake(map[6, 0]);
            ADDSnake(map[7, 0]);
            ADDSnake(map[8, 0]);
            ADDSnake(map[9, 0]);
            dir = Direction.Right;//设置蛇的初始移动方向
        }

        void ADDSnake(Label lbl)
        {
            //如果当前已经有蛇了，把旧的蛇头变成蛇身的颜色
            if (snake.Count > 0) { head.BackColor = bodycolor; }
            head = lbl;
            lbl.BackColor = headcolor;//设置蛇头背景颜色
            snake.Add(lbl);//将标签添加到蛇的身体集合中
        }

        void RemoveSnake()
        {
            snake[0].BackColor = bc;//将蛇尾标签的背景颜色设置为地图背景颜色
            snake.RemoveAt(0);//将蛇尾标签从蛇的身体集合中移除
        }

        void MoveSnake()
        {
            //有时间试试加上自身碰撞判定
            if (dir == Direction.Right)
            {
                if (((ZB)head.Tag).y < cols - 1)
                {
                    ADDSnake(map[((ZB)head.Tag).x, ((ZB)head.Tag).y + 1]);
                    if(EatFood()){BiudFood(); }
                    else { RemoveSnake(); }
                   
                }
                else
                { GameOver();
                }
            }

            if (dir == Direction.Left)
            {
                if (((ZB)head.Tag).y > 0)
                {
                    ADDSnake(map[((ZB)head.Tag).x, ((ZB)head.Tag).y - 1]);
                    if (EatFood()) { BiudFood(); }
                    else { RemoveSnake(); }
                }
                else
                {
                    GameOver();
                }
            }
            
            if(dir == Direction.Up)
            {
                if (((ZB)head.Tag).x > 0)
                {
                    ADDSnake(map[((ZB)head.Tag).x - 1, ((ZB)head.Tag).y]);
                    if (EatFood()) { BiudFood(); }
                    else { RemoveSnake(); }
                }
                else
                {
                    GameOver();
                }
            }


            if (dir == Direction.Down)
            {
                if (((ZB)head.Tag).x < rows - 1)
                {
                    ADDSnake(map[((ZB)head.Tag).x+1,((ZB)head.Tag).y]);
                    if (EatFood()) { BiudFood(); }
                    else { RemoveSnake(); }
                }
                else
                {
                    GameOver();
                }
            }   
            
        }

        void BiudFood()//创建食物
        {
            rand = new Random();
            int food_y = rand.Next(0, rows);
            int food_x = rand.Next(0, cols);
            food = map[food_y, food_x];
            food.BackColor = foodcolor;
            isfood = true;//设置有食物了
        }

       bool EatFood()//吃食物
        {
            //也能写if(((ZB)(head.Tag)).x==((ZB)(food.Tag)).x&&((ZB)(head.Tag)).y==((ZB)(food.Tag)).y)
            //)
            //((ZB)(head.Tag)).x;//获取蛇头的行索引
            //((ZB)(head.Tag)).y;//获取蛇头的列索引
            // ((ZB)(food.Tag)).x;//获取食物的行索引
            //((ZB)(food.Tag)).y;//获取食物的列索引
            //food也有tag，因为初始化食物的时候，food = map[food_y, food_x];

            //如果蛇头的坐标和食物的坐标一样，就吃掉食物
            if (head == food)
            {
                isfood = false;//设置没有食物了
                return true;
            }
            return false;
        }


        void GameOver()
        {
            timer1.Enabled = false;//停止计时器

            this.panel1.Controls.Clear();//清空面板上的所有控件
            
            Label lbl = new Label();
            lbl.Text = "Game Over!";//设置标签文本
            lbl.Size = new Size(this.panel1.Width,this.panel1.Height);//设置标签大小
            lbl.TextAlign = ContentAlignment.MiddleCenter;//设置标签文本居中
            lbl.BackColor = Color.Red;//设置标签背景颜色

            panel1.Controls.Add(lbl);//将标签添加到面板
        }





        private void Form1_Load(object sender, EventArgs e)
        {
            Init_GamePenal();
            Init_Snake();
            this.timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MoveSnake();
            
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Interval = 500;
            if (!isfood)
            {
                BiudFood();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string x = ((ZB)head.Tag).x.ToString();
            string y = ((ZB)head.Tag).y.ToString();

            int test = 1;

            rows = panel1.Height / fheight;//计算行数
            cols = panel1.Width / fwidth;//计算列数

            MessageBox.Show(x + "," + y);
            MessageBox.Show("行rows：" + rows + "||" + "列cols：" + cols);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up&&dir != Direction.Down) { dir = Direction.Up; }
            if (e.KeyCode == Keys.Down&&dir!=Direction.Up) { dir = Direction.Down; }
            if (e.KeyCode == Keys.Left&&dir!=Direction.Right) { dir = Direction.Left; }
            if (e.KeyCode == Keys.Right&&dir!=Direction.Left ) { dir = Direction.Right; }
 
        }

        private void 开始ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Init_GamePenal();
            Init_Snake();
            this.timer1.Enabled = true;

        }

        
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Space) { this.timer1.Enabled = !this.timer1.Enabled; }

        }
    } 
}


    public enum Direction { Up, Down, Left, Right }



public struct ZB
    {
        public int x;
        public int y;
        public ZB(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }






/*
for (int i = 0; i < rows; i++)//循环行数
{
    for (int j = 0; j < cols; j++)//循环列数
    {
        //创建标签，设置每一个标签属性，添加到面板
        //i,j，是标签的索引，真正的物理位置是根据i,j计算出来的
        map[i, j] = new Label();
        map[i, j].Width = fwidth;//设置该标签宽度
        map[i, j].Height = fheight;//设置该标签高度
        map[i, j].Left = j * fwidth;//设置左边距,真正的物理位置
        map[i, j].Top = i * fheight;//设置上边距,真正的物理位置
        map[i, j].BackColor = backcolor;//设置背景颜色
        map[i, j].BorderStyle = BorderStyle.FixedSingle;//设置边框样式
        this.panel1.Controls.Add(map[i, j]);//添加到面板

    }
}
*/