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

namespace WpfApplication1
{
    /// <summary>
    /// HistoryWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class HistoryWindow : Window
    {
        RenderTargetBitmap bitmap;
        private Brush brush;
        private int brushSize = 1;
        private float beforeTime;
        private DrawingVisual drawVisual = new DrawingVisual();
        private DrawingContext drawContext;
        private int beforeStrokeNumber;
        private Point ptLast;

        public HistoryWindow()
        {
            InitializeComponent();
            bitmap = new RenderTargetBitmap(800, 500, 96, 96, PixelFormats.Default);
            image1.Width = 800;
            image1.Height = 500;
            image1.Source = bitmap;
            getFile("nappa_output3.json");
        }

        private void getFile(string fileName)
        {
            try
            {   // Open the text file using a stream reader.
                StreamReader sr = new StreamReader(fileName);
                string json = sr.ReadToEnd();
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
                var serializer = new DataContractJsonSerializer(typeof(List<Stroke>));
                List<Stroke> StrokeList = (List <Stroke>)serializer.ReadObject(stream);
                foreach(Stroke st in StrokeList)
                {
                    StrokeDraw(st);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        private void StrokeDraw(Stroke st)
        {
            Point pt = new Point(st.x, st.y);
            Boolean sameStroke = beforeStrokeNumber == st.strokeNumber;
            if (!sameStroke)
            {
                if (st.isWrite)
                {
                    /*brush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    if (st.time - beforeTime > 200)
                    {*/
                        System.Int32 diff = (int)(st.time - beforeTime);
                        brush = new SolidColorBrush(Color.FromRgb(255, 0, (byte)(diff/2)));
                    //}
                    brushSize = 1;
                }
                else
                {
                    brush = new SolidColorBrush(Color.FromRgb(220, 220, 220));
                    brushSize = 5;
                }
            }
            drawContext = drawVisual.RenderOpen();
            if(sameStroke)
            {
                Pen penDraw = new Pen(brush, 3);
                drawContext.DrawLine(penDraw, ptLast, pt); // 前回の位置からの直線を描画 
            }
            drawContext.DrawEllipse(brush, null, pt, brushSize, brushSize);
            drawContext.Close();
            bitmap.Render(drawVisual);
            ptLast = pt;
            beforeStrokeNumber = st.strokeNumber;
            beforeTime = st.time;
        }
    }
}
