using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace WpfApplication1
{
    /// <summary>
    /// HistoryWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListenWindow : Window
    {
        RenderTargetBitmap bitmap;
        private Brush brush;
        private int brushSize = 1;
        private float beforeTime;
        private DrawingVisual drawVisual = new DrawingVisual();
        private DrawingContext drawContext;
        private int beforeStrokeNumber;
        private Point ptLast;

        Stopwatch sw = new Stopwatch();
        private List<Stroke> strokeList = new List<Stroke>();

        int width = 1500;
        int height = 1000;

        private int strokeNumber = 0;

        public ListenWindow()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
            image1.Width = width;
            image1.Height = height;
            image1.Source = bitmap;
            image_border.Width = width;
            image_border.Height = height;
            sw.Start();
        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            sw.Stop();
            sw.Reset();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Stroke[]));
            string filename = "tmp_Listen.json";
            FileStream fs = new FileStream(filename, FileMode.Create);
            try
            {
                Stroke[] strArray = strokeList.ToArray();
                serializer.WriteObject(fs, strArray);
            }
            finally
            {
                fs.Close();
            }
            strokeList.Clear();
        }

        private void image1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice != null)
                return; // スタイラス操作／タッチ操作のときはスルー
            Point pt = e.GetPosition(image1);
            DrawStart(pt, 0);
        }

        private void image1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice != null)
                return; // スタイラス操作／タッチ操作のときはスルー
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            Point pt = e.GetPosition(image1);
            DrawContinue(pt, 0);
        }

        private void image1_StylusDown(object sender, StylusDownEventArgs e)
        {
            Point pt = e.GetPosition(image1);
            StylusPointCollection sp = e.GetStylusPoints(image1);
            DrawStart(pt, sp[0].PressureFactor);
            e.Handled = true;
        }

        private void image1_StylusMove(object sender, StylusEventArgs e)
        {
            Point pt = e.GetPosition(image1);
            StylusPointCollection sp = e.GetStylusPoints(image1);
            DrawContinue(pt, sp[0].PressureFactor);
            e.Handled = true;
        }

        private void image1_StylusUp(object sender, StylusEventArgs e)
        {
            e.Handled = true;
            strokeNumber += 1;
        }

        private void image1_MouseUp(object sender, MouseEventArgs e)
        {
            strokeNumber += 1;
        }

        private void DrawStart(Point pt, float pressure)
        {
            Stroke st = new Stroke(pt.X, pt.Y, sw.ElapsedMilliseconds, pressure);
            st.SetParam(strokeNumber, strokeList.Count() + 1, true);
                brush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                brushSize = 1;

            drawContext = drawVisual.RenderOpen();
            drawContext.DrawEllipse(brush, null, pt, brushSize, brushSize);
            drawContext.Close();
            bitmap.Render(drawVisual);
            ptLast = pt;
            strokeList.Add(st);
        }

        private void DrawContinue(Point pt, float pressure)
        {
            Stroke st = new Stroke(pt.X, pt.Y, sw.ElapsedMilliseconds, pressure);
            st.SetParam(strokeNumber, strokeList.Count() + 1, true);
            drawContext = drawVisual.RenderOpen();
            Pen penDraw = new Pen(brush, 3);
            drawContext.DrawLine(penDraw, ptLast, pt); // 前回の位置からの直線を描画 
            drawContext.DrawEllipse(brush, null, pt, brushSize, brushSize);// 最終座標に円を描画 
            drawContext.Close();
            bitmap.Render(drawVisual);
            ptLast = pt;
            strokeList.Add(st);
        }

        private void image1_StylusButtonDown(object sender, StylusButtonEventArgs e)
        {
            Console.WriteLine("aa");
        }
    }
}
