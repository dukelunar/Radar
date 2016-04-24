using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ArcheBuddy.Bot.Classes;
using System.Threading;
using DefaultNameSpace;


namespace BasicRadar
{
    public partial class Form1 : Form
    {
        Self me;
        DefaultClass main;
        Pen p3 = new Pen(Color.LightGray, 2f);

        public Form1(DefaultClass main, Self me)
        {
            this.me = me;
            this.main = main;
            InitializeComponent();
        }

        //Draw Line Pointing North
        public void drawSelf()
        {
            int halfX = pictureBox1.ClientRectangle.Width / 2;
            int halfY = pictureBox1.ClientRectangle.Height / 2;

            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            g.DrawLine(p3, new Point(halfX + 0, halfY + 0), new Point(halfX + 0, halfY - 40));
        }


        //Figure out enemy point relative to center of pictureBox1
        private Tuple<int, int> relativeVector(int aX, int aY, int bX, int bY)
        {
            int cX = (bX - aX);
            int cY = (bY - aY);

            return Tuple.Create(cX, cY);
        }


        //rotate point
        private Tuple<int, int> rotate_point(int px, int py, int cx, int cy, double angle)
        {

            angle = ((450 - angle) * (Math.PI / 180f));

            int halfX = pictureBox1.ClientRectangle.Width / 2;
            int halfY = pictureBox1.ClientRectangle.Height / 2;
            int x = (int)(Math.Cos(angle) * (px - cx) - Math.Sin(angle) * (py - cy) + cx);
            int y = (int)(Math.Sin(angle) * (px - cx) + Math.Cos(angle) * (py - cy) + cy);

            return Tuple.Create(x, y);
        }


        //Get enemies and draw points after rotating
        private void getEnemy()
        {
            foreach (var obj in main.getCreatures())
                if (main.IsSafeForRadar(obj) == true)
                {
                    if (obj.type == BotTypes.Player)
                    {
                        if (obj.name != me.name)
                        {
                            int halfX = pictureBox1.ClientRectangle.Width / 2;
                            int halfY = pictureBox1.ClientRectangle.Height / 2;
                            string pName = obj.name;
                            string pDist = ((int)main.dist(obj)).ToString();
                            uint pfactionID = obj.factionId;
                            int aX = (int)me.X;
                            int aY = (int)me.Y;
                            int bX = (int)obj.X;
                            int bY = (int)obj.Y;
                            double angle = findAngle(me.turnAngle);
                            Tuple<int, int> vector = relativeVector(aX, aY, bX, bY);
                            Tuple<int, int> vectorRotate = rotate_point(vector.Item1, vector.Item2, 0, 0, angle);
                            
                            
                            if (main.isEnemy(obj) == true && main.isStealth(pName) != true)
                            {  
                                drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.Red);
                                drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.RosyBrown);  
                            }

                            if (main.isStealth(pName) == true && main.isEnemy(obj) == true)
                            {
                                drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.Purple);
                                drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.MediumPurple);
                            }

                            
                        }
                    }

                }

        }

        //get friendly players, rotate points, draw points
        private void getFriendly()
        {
            foreach (var obj in main.getCreatures())
                if (main.IsSafeForRadar(obj) == true) //check if safe for radar
                {
                    if (obj.type == BotTypes.Player)
                    {
                        if (obj.name != me.name)
                        {
                            int halfX = pictureBox1.ClientRectangle.Width / 2;
                            int halfY = pictureBox1.ClientRectangle.Height / 2;
                            string pName = obj.name;
                            string pDist = ((int)main.dist(obj)).ToString();
                            uint pfactionID = obj.factionId;
                            int aX = (int)me.X;
                            int aY = (int)me.Y;
                            int bX = (int)obj.X;
                            int bY = (int)obj.Y;
                            double angle = findAngle(me.turnAngle);
                            Tuple<int, int> vector = relativeVector(aX, aY, bX, bY);
                            Tuple<int, int> vectorRotate = rotate_point(vector.Item1, vector.Item2, 0, 0, angle);
                            

                            if (main.isEnemy(obj) == false && main.isStealth(pName) != true)
                            {
                                drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.Green);
                                drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.DarkGreen);
                            }
                        }
                    }
                }
        }


        //very horrible way to see if a creature is a world boss
        private void getBosses()
        {
            foreach (var obj in main.getCreatures())
                if (main.IsSafeForRadar(obj) == true) //check if safe for radar
                {
                    if (obj.type == BotTypes.Creature)
                    {
                        if (obj.name != me.name)
                        {
                            int halfX = pictureBox1.ClientRectangle.Width / 2;
                            int halfY = pictureBox1.ClientRectangle.Height / 2;
                            string pName = obj.name;
                            string pDist = ((int)main.dist(obj)).ToString();
                            uint pfactionID = obj.factionId;
                            int aX = (int)me.X;
                            int aY = (int)me.Y;
                            int bX = (int)obj.X;
                            int bY = (int)obj.Y;
                            double angle = findAngle(me.turnAngle);
                            Tuple<int, int> vector = relativeVector(aX, aY, bX, bY);
                            Tuple<int, int> vectorRotate = rotate_point(vector.Item1, vector.Item2, 0, 0, angle);

                            if (obj.hp <= 600000)
                            {
                                drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.Blue);
                                drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.DarkBlue);
                            }



                        }
                    }

                }

        }

        //get vehicles, rotate points, draw points
        private void getVehicles()
        {
            foreach (var obj in main.getCreatures()) //check if safe for radar
                if (main.IsSafeForRadar(obj) == true)
                {
                    if (obj.type == BotTypes.Slave)
                    {
                        if (obj.name != me.name)
                        {
                            int halfX = pictureBox1.ClientRectangle.Width / 2;
                            int halfY = pictureBox1.ClientRectangle.Height / 2;
                            string pName = obj.name;
                            string pDist = ((int)main.dist(obj)).ToString();
                            uint pfactionID = obj.factionId;
                            int aX = (int)me.X;
                            int aY = (int)me.Y;
                            int bX = (int)obj.X;
                            int bY = (int)obj.Y;
                            double angle = findAngle(me.turnAngle);
                            Tuple<int, int> vector = relativeVector(aX, aY, bX, bY);
                            Tuple<int, int> vectorRotate = rotate_point(vector.Item1, vector.Item2, 0, 0, angle);

                            if (main.isEnemy(obj) == false)
                            {
                                drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.Teal);
                                drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.SkyBlue);
                            }

                            if (main.isEnemy(obj) == true)
                            {
                                drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.OrangeRed);
                                drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.OrangeRed);
                            }


                        }
                    }

                }

        }

        private void getPlants()
        {
            foreach (var obj in main.getDoodads())
                if (main.IsSafeForRadar(obj) == true)
                {
                    if (obj.type == BotTypes.DoodadObject)
                    {
                        if (obj.name != me.name)
                        {
                            int halfX = pictureBox1.ClientRectangle.Width / 2;
                            int halfY = pictureBox1.ClientRectangle.Height / 2;
                            string pName = obj.name;
                            string pDist = ((int)main.dist(obj)).ToString();
                            int aX = (int)me.X;
                            int aY = (int)me.Y;
                            int bX = (int)obj.X;
                            int bY = (int)obj.Y;
                            double angle = findAngle(me.turnAngle);
                            Tuple<int, int> vector = relativeVector(aX, aY, bX, bY);
                            Tuple<int, int> vectorRotate = rotate_point(vector.Item1, vector.Item2, 0, 0, angle);

                            if (pName == "Wild Ginseng")
                            {
                                drawPoint(halfX + vectorRotate.Item1, halfY + vectorRotate.Item2, Color.Pink);
                                drawString(pName + " " + pDist + "m", (halfX + (int)(vectorRotate.Item1 - (8 * (pName.Count() / 2)))), (halfY + (vectorRotate.Item2 + 15)), Brushes.DeepPink);
                            }



                        }
                    }

                }

        }


        private double findAngle(int angle)
         {

             //COUNTERCLOCKWISE
             //TEMPangle -= 43;
             double newAngle = angle * 2.8125;
             /*angle = (int)(angle * 2.8125);

             if (angle > 360)
                 angle = 360;

             angle = angle - 127;

             if (angle > 360)
             {
                 int theDiff = angle - 360;
                 angle = 0 + theDiff;
             }
             if (angle < 0)
                 angle = 360 + angle;

             return angle;*/
             //newAngle -= 120.9375;
             newAngle -= 30;
             if (newAngle < 0.0)
                 return newAngle + 360.0;
             else
                 return newAngle;
         }

        public void drawPoint(int x, int y, Color c)
        {
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);
            SolidBrush brush = new SolidBrush(c);
            Point dPoint = new Point(x, (pictureBox1.Height - y));
            dPoint.X = dPoint.X - 2;
            dPoint.Y = dPoint.Y - 2;
            Rectangle rect = new Rectangle(dPoint, new Size(4, 4));
            g.FillRectangle(brush, rect);
            g.Dispose();
        }

        private void drawString(string y, int a, int b, Brush c)
        {
            Graphics g = Graphics.FromHwnd(pictureBox1.Handle);

            using (Font myFont = new Font("Arial", 8))
            {
                Point dPoint = new Point(a, (pictureBox1.Height - b));
                dPoint.X = dPoint.X - 2;
                dPoint.Y = dPoint.Y - 2;
                g.DrawString(y, myFont, c, dPoint);
            }
        }

        /*
        private bool figFaction()
        {
            if (me.factionId == 109 || me.factionId == 113) //east
            {

            }

            if (me.factionId == 101 || me.factionId == 103)
            {

            }

        }
        */

        //Allows borderless windows to be moved
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x84:
                    base.WndProc(ref m);
                    if ((int)m.Result == 0x1)
                        m.Result = (IntPtr)0x2;
                    return;
            }

            base.WndProc(ref m);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            
        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
        }



        private void timer1_Tick_1(object sender, EventArgs e)
        {
            int halfX = pictureBox1.ClientRectangle.Width / 2;
            int halfY = pictureBox1.ClientRectangle.Height / 2;

            pictureBox1.Invalidate();

            drawSelf();
            drawPoint(halfX + 0, halfY + 0, Color.White);

            if (checkBox1.Checked)
            {
                getEnemy();
            }
            if (checkBox2.Checked)
            {
                getFriendly();
            }
            if (checkBox3.Checked)
            {
                getVehicles();
            }
            if (checkBox4.Checked)
            {
                getPlants();
            }
            if (checkBox4.Checked)
            {
                getBosses();
            }


            Thread.Sleep(300);
        }
    }
}
