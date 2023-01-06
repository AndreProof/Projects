using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace TPRLab_6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DrawGraph();
        }

        List<XDate> date;
        List<double> high;
        List<double> low;
        List<double> open;
        List<double> close;
        List<double> vol;

        public void DrawGraph()
        {
            //DATE, TIME, OPEN, HIGH, LOW, CLOSE, VOL
            GraphPane pane = zedGraph.GraphPane;
            pane.Title.Text = "MTS";
            pane.XAxis.Title.Text = "Date";
            pane.XAxis.Type = AxisType.Date;
            pane.XAxis.Scale.MajorStep = 1;
            pane.XAxis.Scale.MinorStep = 1;
            LoadFromFile();
        }

        public void EoM()
        {
            richTextBox1.Text = Properties.Resources.EMV;
            List<double> EMV = new List<double>();
            double MM, BR;
            for (int i = 1; i<high.Count; i++)
            {
                MM = (high[i] - low[i]) / 2 - (high[i - 1] - low[i - 1]) / 2;
                BR = vol[i]/10000 / (high[i] - low[i]);
                EMV.Add(MM / BR);
            }
            PointPairList list1 = new PointPairList();
            GraphPane pane = zedGraph.GraphPane;
            pane.CurveList.Clear();
            pane.GraphObjList.Clear();
            pane.YAxisList.Clear();

            double X;
            for (int x = 1; x < date.Count; x++)
            {
                if (x >= 2)
                {
                    if (EMV[x - 1] >= 0 && EMV[x - 2] < 0)
                    {
                        X = -EMV[x - 2] / (EMV[x - 1] - EMV[x - 2]) * (date[x] - date[x - 1]) + date[x - 1];
                        ArrowObj arrow = new ArrowObj(X, -EMV.Average()*3, X, 0);
                        arrow.Line.Color = Color.Green;
                        arrow.Size -= 5;
                        pane.GraphObjList.Add(arrow);
                    }
                    else if(EMV[x-1] <= 0 && EMV[x - 2] > 0)
                    {
                        X = -EMV[x - 2] / (EMV[x - 1] - EMV[x - 2]) * (date[x] - date[x - 1]) + date[x - 1];
                        ArrowObj arrow = new ArrowObj(X, EMV.Average() * 3, X, 0);
                        arrow.Line.Color = Color.Red;
                        arrow.Size -= 5;
                        pane.GraphObjList.Add(arrow);
                    }
                }
                list1.Add(date[x], EMV[x - 1]);
            }
            
            int axis1 = pane.AddYAxis("EMV");
            LineItem myCurve1 = pane.AddCurve("EMV", list1, Color.Purple, SymbolType.None);
            myCurve1.YAxisIndex = axis1;
            pane.YAxisList[axis1].Title.FontSpec.FontColor = Color.Purple;
        }

        public void oBV()
        {
            richTextBox1.Text = Properties.Resources.OBV;
            List<double> OBV = new List<double>();
            OBV.Add(0);
            for (int i = 1; i < high.Count; i++)
            {
                if (close[i] > close[i - 1])
                    OBV.Add(OBV[i - 1] + vol[i]);
                else if (close[i] < close[i - 1])
                    OBV.Add(OBV[i - 1] - vol[i]);
                else
                    OBV.Add(OBV[i - 1]);
            }

            PointPairList list1 = new PointPairList();
            GraphPane pane = zedGraph.GraphPane;
            pane.CurveList.Clear();
            pane.GraphObjList.Clear();
            pane.YAxisList.Clear();
            bool down = false;
            for (int x = 0; x < date.Count; x++)
            {
                if (x >= 2)
                {
                    if (OBV[x-1] > OBV[x] && !down)
                    {
                        ArrowObj arrow = new ArrowObj(date[x-1], OBV[x-1]+vol.Average(), date[x-1], OBV[x-1]);
                        arrow.Line.Color = Color.Red;
                        arrow.Size -= 5;
                        pane.GraphObjList.Add(arrow);
                        down = true;
                    }
                    else if (OBV[x -1] < OBV[x] && down)
                    {
                        ArrowObj arrow = new ArrowObj(date[x-1], OBV[x-1] - vol.Average(), date[x-1], OBV[x-1]);
                        arrow.Line.Color = Color.Green;
                        arrow.Size -= 5;
                        pane.GraphObjList.Add(arrow);
                        down = false;
                    }
                }
                list1.Add(date[x], OBV[x]);
            }

            int axis1 = pane.AddYAxis("OBV");
            LineItem myCurve1 = pane.AddCurve("OBV", list1, Color.Purple, SymbolType.None);
            myCurve1.YAxisIndex = axis1;
            pane.YAxisList[axis1].Title.FontSpec.FontColor = Color.Purple;
        }

        public void PAVT()
        {
            richTextBox1.Text = Properties.Resources.PVT;
            List<double> PVT = new List<double>();
            PVT.Add(0);
            for (int i = 1; i < high.Count; i++)
            {
                PVT.Add(PVT[i-1] + (close[i] - close[i-1])/close[i-1]*vol[i]);
            }

            PointPairList list1 = new PointPairList();
            GraphPane pane = zedGraph.GraphPane;
            pane.CurveList.Clear();
            pane.GraphObjList.Clear();
            pane.YAxisList.Clear();
            bool down = false;
            for (int x = 0; x < date.Count; x++)
            {
                if (x >= 2)
                {
                    if (PVT[x - 1] > PVT[x] && !down)
                    {
                        ArrowObj arrow = new ArrowObj(date[x - 1], PVT[x - 1] + Math.Abs(PVT.Average()/2), date[x - 1], PVT[x - 1]);
                        arrow.Line.Color = Color.Red;
                        arrow.Size -= 5;
                        pane.GraphObjList.Add(arrow);
                        down = true;
                    }
                    else if (PVT[x - 1] < PVT[x] && down)
                    {
                        ArrowObj arrow = new ArrowObj(date[x - 1], PVT[x - 1] - Math.Abs(PVT.Average() / 2), date[x - 1], PVT[x - 1]);
                        arrow.Line.Color = Color.Green;
                        arrow.Size -= 5;
                        pane.GraphObjList.Add(arrow);
                        down = false;
                    }
                }
                list1.Add(date[x], PVT[x]);
            }

            int axis1 = pane.AddYAxis("PVT");
            LineItem myCurve1 = pane.AddCurve("PVT", list1, Color.Purple, SymbolType.None);
            myCurve1.YAxisIndex = axis1;
            pane.YAxisList[axis1].Title.FontSpec.FontColor = Color.Purple;
        }

        public void Williams()
        {
            richTextBox1.Text = Properties.Resources.Williams;
            List<double> R = new List<double>();
            double max = high.Max(); double min = low.Min();
            for (int i = 0; i < high.Count; i++)
            {
                R.Add((max-close[i])/(max - min));
            }

            PointPairList list1 = new PointPairList();
            GraphPane pane = zedGraph.GraphPane;
            pane.CurveList.Clear();
            pane.GraphObjList.Clear();
            pane.YAxisList.Clear();

            max = R.Min() + (R.Max() - R.Min()) / 100 * 80;
            min = R.Min() + (R.Max() - R.Min()) / 100 * 20;

            LineObj line1 = new LineObj(pane.XAxis.Scale.Min, max, pane.XAxis.Scale.Max, max);
            LineObj line2 = new LineObj(pane.XAxis.Scale.Min, min, pane.XAxis.Scale.Max, min);
            line1.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            line2.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            pane.GraphObjList.Add(line1);
            pane.GraphObjList.Add(line2);
            bool maximum = true;
            if (R[0]>max)
                maximum = false;
            for (int x = 0; x < date.Count; x++)
            {
                if(R[x] > max && maximum)
                {
                    ArrowObj arrow = new ArrowObj(date[x], R[x] + Math.Abs(R.Average() / 5), date[x], R[x]);
                    arrow.Line.Color = Color.Red;
                    arrow.Size -= 5;
                    pane.GraphObjList.Add(arrow);
                    maximum = false;
                }
                else if(R[x] < min && maximum)
                {
                    ArrowObj arrow = new ArrowObj(date[x], R[x] - Math.Abs(R.Average() / 5), date[x], R[x]);
                    arrow.Line.Color = Color.Green;
                    arrow.Size -= 5;
                    pane.GraphObjList.Add(arrow);
                    maximum = false;
                }
                if (R[x] < max && R[x] > min)
                    maximum = true;
                list1.Add(date[x], R[x]);
            }

            int axis1 = pane.AddYAxis("Williams");
            LineItem myCurve1 = pane.AddCurve("Williams", list1, Color.Purple, SymbolType.None);
            myCurve1.YAxisIndex = axis1;
            pane.YAxisList[axis1].Title.FontSpec.FontColor = Color.Purple;
        }

        public void PRC()
        {
            richTextBox1.Text = Properties.Resources.ROC;
            List<double> ROC = new List<double>();
            double max , min;
            for (int i = 3; i < high.Count; i++)
            {
                ROC.Add(100*(close[i]-close[i-3])/close[i-3]);
            }

            PointPairList list1 = new PointPairList();
            GraphPane pane = zedGraph.GraphPane;
            pane.CurveList.Clear();
            pane.GraphObjList.Clear();
            pane.YAxisList.Clear();

            max = ROC.Min() + (ROC.Max() - ROC.Min()) / 100 * 80;
            min = ROC.Min() + (ROC.Max() - ROC.Min()) / 100 * 20;

            LineObj line1 = new LineObj(pane.XAxis.Scale.Min, max, pane.XAxis.Scale.Max, max);
            LineObj line2 = new LineObj(pane.XAxis.Scale.Min, min, pane.XAxis.Scale.Max, min);
            line1.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            line2.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            pane.GraphObjList.Add(line1);
            pane.GraphObjList.Add(line2);
            bool maximum = true;
            if (ROC[0] > max)
                maximum = false;
            for (int x = 3; x < date.Count; x++)
            {
                if (ROC[x-3] > max && maximum)
                {
                    ArrowObj arrow = new ArrowObj(date[x], ROC[x-3] + (ROC.Max()-ROC.Min())/8, date[x], ROC[x-3]);
                    arrow.Line.Color = Color.Red;
                    arrow.Size -= 5;
                    pane.GraphObjList.Add(arrow);
                    maximum = false;
                }
                else if (ROC[x-3] < min && maximum)
                {
                    ArrowObj arrow = new ArrowObj(date[x], ROC[x-3] - (ROC.Max() - ROC.Min()) /8, date[x], ROC[x-3]);
                    arrow.Line.Color = Color.Green;
                    arrow.Size -= 5;
                    pane.GraphObjList.Add(arrow);
                    maximum = false;
                }
                if (ROC[x-3] < max && ROC[x-3] > min)
                    maximum = true;
                list1.Add(date[x], ROC[x-3]);
            }

            int axis1 = pane.AddYAxis("PRC");
            LineItem myCurve1 = pane.AddCurve("PRC", list1, Color.Purple, SymbolType.None);
            myCurve1.YAxisIndex = axis1;
            pane.YAxisList[axis1].Title.FontSpec.FontColor = Color.Purple;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                EoM();
                zedGraph.AxisChange();
                zedGraph.Invalidate();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                oBV();
                zedGraph.AxisChange();
                zedGraph.Invalidate();
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                PAVT();
                zedGraph.AxisChange();
                zedGraph.Invalidate();
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                Williams();
                zedGraph.AxisChange();
                zedGraph.Invalidate();
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                PRC();
                zedGraph.AxisChange();
                zedGraph.Invalidate();
            }
        }

        public void LoadFromFile()
        {
            try
            {
                dataGridView1.Rows.Clear();
                StreamReader sr;
                if (radioButton15.Checked)
                {
                    Stream path;
                    if (radioButton10.Checked)
                        path = Assembly.GetExecutingAssembly().GetManifestResourceStream("TPRLab_6.MTS.txt");
                    else if (radioButton9.Checked)
                        path = Assembly.GetExecutingAssembly().GetManifestResourceStream("TPRLab_6.AEROFLOT.txt");
                    else if (radioButton8.Checked)
                        path = Assembly.GetExecutingAssembly().GetManifestResourceStream("TPRLab_6.MEGAFON.txt");
                    else if (radioButton7.Checked)
                        path = Assembly.GetExecutingAssembly().GetManifestResourceStream("TPRLab_6.LENTA.txt");
                    else if (radioButton6.Checked)
                        path = Assembly.GetExecutingAssembly().GetManifestResourceStream("TPRLab_6.ROSNEFT.txt");
                    else
                        path = Assembly.GetExecutingAssembly().GetManifestResourceStream("TPRLab_6.LUKOIL.txt");
                    sr = new StreamReader(path, Encoding.Default);
                }
                else
                {
                    string path;
                    int year = dateTimePicker1.Value.Year % 100;
                    int month = dateTimePicker1.Value.Month;
                    int day = dateTimePicker1.Value.Day;
                    int year2 = dateTimePicker2.Value.Year % 100;
                    int month2 = dateTimePicker2.Value.Month;
                    int day2 = dateTimePicker2.Value.Day;
                    if (radioButton10.Checked)
                        path = "http://export.finam.ru/MTSS_" + year + month + day + "_" + year + month + day + ".txt?market=1&em=15523&code=MTSS&apply=0&df=" + day + "&mf=" + (month - 1) + "&yf=20" + year + "&from=" + day + "." + month + ".20" + year + "&dt=" + day2 + "&mt=" + (month2 - 1) + "&yt=20" + year2 + "&to=" + day2 + "." + month2 + ".20" + year2 + "&p=7&f=MTSS_180601_181031&e=.txt&cn=MTSS&dtf=4&tmf=4&MSOR=0&mstime=on&mstimever=1&sep=5&sep2=1&datf=5";
                    else if (radioButton9.Checked)
                        path = "http://export.finam.ru/AFLT_" + year + month + day + "_" + year + month + day + ".txt?market=1&em=29&code=AFLT&apply=0&df=" + day + "&mf=" + (month - 1) + "&yf=20" + year + "&from=" + day + "." + month + ".20" + year + "&dt=" + day2 + "&mt=" + (month2 - 1) + "&yt=20" + year2 + "&to=" + day2 + "." + month2 + ".20" + year2 + "&p=7&f=AFLT_180601_181031&e=.txt&cn=AFLT&dtf=4&tmf=4&MSOR=0&mstime=on&mstimever=1&sep=5&sep2=1&datf=5";
                    else if (radioButton8.Checked)
                        path = "http://export.finam.ru/MFON_" + year + month + day + "_" + year + month + day + ".txt?market=1&em=152516&code=MFON&apply=0&df=" + day + "&mf=" + (month - 1) + "&yf=20" + year + "&from=" + day + "." + month + ".20" + year + "&dt=" + day2 + "&mt=" + (month2 - 1) + "&yt=20" + year2 + "&to=" + day2 + "." + month2 + ".20" + year2 + "&p=7&f=AFLT_180601_181031&e=.txt&cn=MFON&dtf=4&tmf=4&MSOR=0&mstime=on&mstimever=1&sep=5&sep2=1&datf=5";
                    else if (radioButton7.Checked)
                        path = "http://export.finam.ru/LNTA_" + year + month + day + "_" + year + month + day + ".txt?market=1&em=385792&code=LNTA&apply=0&df=" + day + "&mf=" + (month - 1) + "&yf=20" + year + "&from=" + day + "." + month + ".20" + year + "&dt=" + day2 + "&mt=" + (month2 - 1) + "&yt=20" + year2 + "&to=" + day2 + "." + month2 + ".20" + year2 + "&p=7&f=AFLT_180601_181031&e=.txt&cn=LNTA&dtf=4&tmf=4&MSOR=0&mstime=on&mstimever=1&sep=5&sep2=1&datf=5";
                    else if (radioButton6.Checked)
                        path = "http://export.finam.ru/ROSN_" + year + month + day + "_" + year + month + day + ".txt?market=1&em=17273&code=ROSN&apply=0&df=" + day + "&mf=" + (month - 1) + "&yf=20" + year + "&from=" + day + "." + month + ".20" + year + "&dt=" + day2 + "&mt=" + (month2 - 1) + "&yt=20" + year2 + "&to=" + day2 + "." + month2 + ".20" + year2 + "&p=7&f=AFLT_180601_181031&e=.txt&cn=ROSN&dtf=4&tmf=4&MSOR=0&mstime=on&mstimever=1&sep=5&sep2=1&datf=5";
                    else
                        path = "http://export.finam.ru/LKON_" + year + month + day + "_" + year + month + day + ".txt?market=1&em=8&code=LKOH&apply=0&df=" + day + "&mf=" + (month - 1) + "&yf=20" + year + "&from=" + day + "." + month + ".20" + year + "&dt=" + day2 + "&mt=" + (month2 - 1) + "&yt=20" + year2 + "&to=" + day2 + "." + month2 + ".20" + year2 + "&p=7&f=AFLT_180601_181031&e=.txt&cn=LKOH&dtf=4&tmf=4&MSOR=0&mstime=on&mstimever=1&sep=5&sep2=1&datf=5";

                    sr = new StreamReader(new WebClient().OpenRead(path));
                }
                date = new List<XDate>();
                high = new List<double>();
                low = new List<double>();
                open = new List<double>();
                close = new List<double>();
                vol = new List<double>();
                string[] data; int i = 0;
                while (!sr.EndOfStream)
                {
                    data = sr.ReadLine().Split(' ');
                    date.Add((Convert.ToDateTime(data[0] + " " + data[1])));
                    high.Add(Convert.ToDouble(data[3].Replace('.', ',')));
                    low.Add(Convert.ToDouble(data[4].Replace('.', ',')));
                    open.Add(Convert.ToDouble(data[2].Replace('.', ',')));
                    close.Add(Convert.ToDouble(data[5].Replace('.', ',')));
                    vol.Add(Convert.ToDouble(data[6].Replace('.', ',')));
                    dataGridView1.Rows.Add();
                    i = dataGridView1.RowCount;
                    dataGridView1.Rows[i - 1].HeaderCell.Value = i.ToString();
                    dataGridView1.Rows[i - 1].Cells[0].Value = date[i - 1];
                    dataGridView1.Rows[i - 1].Cells[1].Value = high[i - 1];
                    dataGridView1.Rows[i - 1].Cells[2].Value = low[i - 1];
                    dataGridView1.Rows[i - 1].Cells[3].Value = open[i - 1];
                    dataGridView1.Rows[i - 1].Cells[4].Value = close[i - 1];
                    dataGridView1.Rows[i - 1].Cells[5].Value = vol[i - 1];

                }
                if (radioButton1.Checked)
                    EoM();
                else if (radioButton2.Checked)
                    oBV();
                else if (radioButton3.Checked)
                    PAVT();
                else if (radioButton4.Checked)
                    Williams();
                else
                    PRC();
                if (radioButton13.Checked)
                    CandleStick();
                else
                    OHLC();
                zedGraph.AxisChange();
                zedGraph.Invalidate();
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                radioButton15.Checked = true;
            }
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            zedGraph.GraphPane.Title.Text = "МТС";
            LoadFromFile();
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            zedGraph.GraphPane.Title.Text = "Аэрофлот";
            LoadFromFile();
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            zedGraph.GraphPane.Title.Text = "Мегафон";
            LoadFromFile();
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            zedGraph.GraphPane.Title.Text = "Лента";
            LoadFromFile();
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            zedGraph.GraphPane.Title.Text = "Роснефть";
            LoadFromFile();
        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {
            zedGraph.GraphPane.Title.Text = "Лукоил";
            LoadFromFile();
        }

        public void CandleStick()
        {

            GraphPane myPane = zedGraphControl1.GraphPane;
            myPane.CurveList.Clear();
            myPane.Title.Text = "Свечи";
            myPane.XAxis.Title.Text = "Date";
            myPane.YAxis.Title.Text = "Price";

            StockPointList spl = new StockPointList();

            for (int i = 0; i < date.Count; i++)
            {
                StockPt pt = new StockPt(date[i], high[i], low[i], open[i], close[i], vol[i]);
                spl.Add(pt);
            }

            JapaneseCandleStickItem myCurve = myPane.AddJapaneseCandleStick("CandleStick", spl);
            myCurve.Stick.IsAutoSize = true;
            myCurve.Stick.Color = Color.Blue;

            myPane.XAxis.Type = AxisType.DateAsOrdinal;
        }

        public void OHLC()
        {

            GraphPane myPane = zedGraphControl1.GraphPane;
            myPane.CurveList.Clear();
            myPane.Title.Text = "Бары";
            myPane.XAxis.Title.Text = "Date";
            myPane.YAxis.Title.Text = "Price";

            StockPointList spl = new StockPointList();

            for (int i = 0; i < date.Count; i++)
            {
                StockPt pt = new StockPt(date[i], high[i], low[i], open[i], close[i], vol[i]);
                spl.Add(pt);
            }

            OHLCBarItem myCurve = myPane.AddOHLCBar("OHLC", spl, Color.Black);

            myPane.XAxis.Type = AxisType.DateAsOrdinal;
        }

        private void radioButton13_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton13.Checked)
                CandleStick();
            else
                OHLC();
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            LoadFromFile();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            LoadFromFile();
        }

        private void radioButton15_CheckedChanged(object sender, EventArgs e)
        {
            LoadFromFile();
        }
    }
}
