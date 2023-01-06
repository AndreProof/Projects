using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectWindow
{
    public partial class Form1 : Form
    {
        public Graphics gr;
        public List<Window> Windows;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Windows = new List<Window>();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            gr = Graphics.FromImage(pictureBox1.Image);
            gr.SmoothingMode = SmoothingMode.HighQuality;
            
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            gr = Graphics.FromImage(pictureBox1.Image);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Window wind = new Window(new PointF(100, 100), (float)numericUpDown5.Value, (float)numericUpDown6.Value, (float)numericUpDown1.Value, (float)numericUpDown2.Value, (float)numericUpDown3.Value, (float)numericUpDown4.Value);
            Windows.Add(wind);
            string add = "Width: " + wind.Width + " Height: " + wind.Height + " Up: " + wind.Up + " Down: " + wind.Down + " Left: " + wind.Left + " Right: " + wind.Right;
            listBox1.Items.Add(add);
        }

        private void Place()
        {
            Windows.Sort((x,y) => y.Square.CompareTo(x.Square));
            int countList = 0;
            List<List<Window>> listWithWindows = new List<List<Window>>();
            listWithWindows.Add(new List<Window>());
            PointF BP = new PointF();
            float BR = float.MaxValue;
            bool Rotate = false;
            bool find = false;
            for(int i = 0; i < Windows.Count;)
            {
                Window tmp = Windows[i].Copy();
                PointF pointTmp;
                foreach(Window winIn in listWithWindows[countList])
                {
                    pointTmp = FindPlace(winIn.OutPoint1, winIn.Width, winIn.Height, listWithWindows[countList], tmp);
                    if (pointTmp.X != float.MaxValue)
                    {
                        tmp.MoveTo(pointTmp);
                        if (tmp.OutPoint2.X < BR)
                        {
                            BP.X = pointTmp.X; BP.Y = pointTmp.Y;
                            BR = tmp.OutPoint2.X;
                        }
                        find = true;
                    }
                }
                tmp.Rotate();
                foreach (Window winIn in listWithWindows[countList])
                {
                    pointTmp = FindPlace(winIn.OutPoint1, winIn.Width, winIn.Height, listWithWindows[countList], tmp);
                    if (pointTmp.X != float.MaxValue)
                    {
                        tmp.MoveTo(pointTmp);
                        if (tmp.OutPoint2.X < BR)
                        {
                            BP.X = pointTmp.X; BP.Y = pointTmp.Y;
                            BR = tmp.OutPoint2.X;
                            Rotate = true;
                        }
                        find = true;
                    }
                }
                tmp.Rotate();
                if (find)
                {
                    if(Rotate)
                        tmp.Rotate();
                    tmp.MoveTo(BP);
                    listWithWindows[countList].Add(tmp);
                    find = false; Rotate = false; BR = float.MaxValue;
                    i++;
                    continue;
                }
                else
                {
                    pointTmp = FindPlace(new PointF(0,0), (float)numericUpDown7.Value, (float)numericUpDown8.Value, listWithWindows[countList], tmp);
                    if (pointTmp.X != float.MaxValue)
                    {
                        tmp.MoveTo(pointTmp);
                        if (tmp.OutPoint2.X < BR)
                        {
                            BP.X = pointTmp.X; BP.Y = pointTmp.Y;
                            BR = tmp.OutPoint2.X;
                        }
                        find = true;
                    }
                    tmp.Rotate();
                    pointTmp = FindPlace(new PointF(0, 0), (float)numericUpDown7.Value, (float)numericUpDown8.Value, listWithWindows[countList], tmp);
                    if (pointTmp.X != float.MaxValue)
                    {
                        tmp.MoveTo(pointTmp);
                        if (tmp.OutPoint2.X < BR)
                        {
                            BP.X = pointTmp.X; BP.Y = pointTmp.Y;
                            BR = tmp.OutPoint2.X;
                            Rotate = true;
                        }
                        find = true;
                    }
                    tmp.Rotate();
                }
                if (find)
                {
                    if (Rotate)
                        tmp.Rotate();
                    tmp.MoveTo(BP);
                    listWithWindows[countList].Add(tmp);
                    find = false; Rotate = false; BR = float.MaxValue;
                    i++;
                    continue;
                }
                else
                {
                    listWithWindows.Add(new List<Window>());
                    countList++;
                }
            }
            PaintAll(listWithWindows);
            MessageBox.Show("Для того, чтобы вырезать " + listBox1.Items.Count + " оконных рам на листах размером " + numericUpDown7.Value + "x" + numericUpDown8.Value + " необходимое количество листов: " + (countList+1));
        }

        private PointF FindPlace(PointF begin, float width, float height, List<Window> wind, Window win)
        {
            bool cross = false;
            for(int i = (int)begin.X; i <= begin.X + width - win.Width; i++)
            {
                for (int j = (int)begin.Y; j <= begin.Y + height - win.Height; j++)
                {
                    cross = false;
                    win.MoveTo(new PointF(i,j));
                    foreach(Window w in wind)
                    {
                        if (win.Intersect(w))
                        {
                            cross = true;
                        }
                    }
                    if(!cross)
                        return new PointF(i, j);
                }
            }
            return new PointF(float.MaxValue, float.MaxValue);
        }

        private void PaintAll(List<List<Window>> listWith)
        {
            float width = (float)numericUpDown7.Value;
            float height = (float)numericUpDown8.Value;
            gr.Clear(Color.FromName("Control"));
            Pen pen = new Pen(Color.Black, 1);
            for (int i = 0; i < listWith.Count; i++)
            {
                gr.DrawRectangle(pen, 0, i * height, width, height);
                for(int j = 0; j < listWith[i].Count; j++)
                {
                    listWith[i][j].MoveTo(new PointF(listWith[i][j].OutPoint1.X, listWith[i][j].OutPoint1.Y + i * height));
                    listWith[i][j].DrawWindow(gr);
                }
            }
            pictureBox1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Place();
        }

        private void удалитьЭлементToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string str = listBox1.SelectedItem.ToString();
            string[] data = str.Split(' ');
            int index = Windows.FindIndex(x => x.Width == Convert.ToSingle(data[1]) && x.Height == Convert.ToSingle(data[3]) && x.Up == Convert.ToSingle(data[5]) && x.Down == Convert.ToSingle(data[7]) && x.Left == Convert.ToSingle(data[9]) && x.Right == Convert.ToSingle(data[11]));
            Windows.RemoveAt(index);
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
        }

        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            Windows.Clear();
        }
    }
}
