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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        // User writing info
        Stopwatch sw = new Stopwatch();
        private List<Stroke> strokeList = new List<Stroke>();
        private int strokeNumber = 0;
        private List<Question> solvedQuestionList = new List<Question>();
        private Question solvingQuestion;
        private Question[] allQuestionArray = new Question[11];


        RenderTargetBitmap bitmap;
        public MainWindow()
        {
            InitializeComponent();
            allQuestionArraySet();
            QuestionSet();
        }
        private Point ptLast;
        private Brush brush;
        private DrawingVisual drawVisual = new DrawingVisual();
        private DrawingContext drawContext;
        private Random rand = new Random();

        private void QuestionSet()
        {
            bitmap = new RenderTargetBitmap(800, 100, 96, 96, PixelFormats.Default);
            image1.Width = 800;
            image1.Height = 100;
            image1.Source = bitmap;
            QuestionTextBlock.Text = "Q. " + solvingQuestion.question;
            AnswerTextBlock.Text = "A. ";
            AnswerButtons.Visibility = Visibility.Hidden;
            strokeNumber = 0;
            sw.Start();
        }

        private void image1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice != null)
                return; // スタイラス操作／タッチ操作のときはスルー
            Point pt = e.GetPosition(image1);
            DrawStart(pt);
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
            DrawContinue(pt);
        }

        private void image1_StylusDown(object sender, StylusDownEventArgs e)
        {
            Point pt = e.GetPosition(image1);
            DrawStart(pt);
            e.Handled = true;
        }

        private void image1_StylusMove(object sender, StylusEventArgs e)
        {
            Point pt = e.GetPosition(image1);
            DrawContinue(pt);
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

        private void DrawStart(Point pt)
        {
            Stroke st = new Stroke(pt.X, pt.Y, sw.ElapsedMilliseconds, strokeNumber);
            brush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            drawContext = drawVisual.RenderOpen();
            drawContext.DrawEllipse(brush, null, pt, 1, 1);
            drawContext.Close();
            bitmap.Render(drawVisual);
            ptLast = pt;
            strokeList.Add(st);
        }

        private void DrawContinue(Point pt)
        {
            Stroke st = new Stroke(pt.X, pt.Y, sw.ElapsedMilliseconds, strokeNumber);
            drawContext = drawVisual.RenderOpen();
            Pen penDraw = new Pen(brush, 3);
            drawContext.DrawLine(penDraw, ptLast, pt); // 前回の位置からの直線を描画 
            drawContext.DrawEllipse(brush, null, pt, 1, 1); // 最終座標に円を描画 
            drawContext.Close();
            bitmap.Render(drawVisual);
            ptLast = pt;
            strokeList.Add(st);
        }

        private void solving_Button_Click(object sender, RoutedEventArgs e)
        {
            StrokeSave();
            AnswerTextBlock.Text = "A. " + solvingQuestion.answer;
            AnswerButtons.Visibility = Visibility.Visible;
        }

        private void answered_Button_Click(object sender, RoutedEventArgs e)
        {
            Button target = (Button)sender;
            switch (target.Name)
            {
                case "correct":
                    AnswerSave(4);
                        break;
                case "word":
                    AnswerSave(1);
                        break;
                case "grammer":
                    AnswerSave(2);
                    break;
                case "both":
                    AnswerSave(3);
                    break;
                default:
                    AnswerSave(0);
                    break;
            }
            QuestionSet();
        }

        private void StrokeSave()
        {
            sw.Stop();
            sw.Reset();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Stroke[]));
            int solvingN = solvingQuestion.questionNumber;
            string filename = "temy_output" + solvingN.ToString() + ".json";
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

        private void AnswerSave(int solveType)
        {
            int solvingN = solvingQuestion.questionNumber;
            Question solvedQuestion = this.solvingQuestion;
            solvedQuestion.solveType = solveType;
            solvedQuestionList.Add(solvedQuestion);
            if (solvingN < allQuestionArray.Length - 1)
            {
                solvingQuestion = allQuestionArray[solvingN + 1];
            }else
            {
                Finish();
            }
        }

        private void allQuestionArraySet()
        {
            this.allQuestionArray[0] = new Question("これはぺんですか", "Is this a pen?", 0);
            this.allQuestionArray[1] = new Question("花見に行くのを楽しみにしていたのに、雨で台なしになった。", "", 1);
            this.allQuestionArray[2] = new Question("その家は夜とても静かで、なかなか寝つけなかった。", "", 2);
            this.allQuestionArray[3] = new Question("私たちは健康に有害な食品をそれと知らずに口にしていることが多い。", "", 3);
            this.allQuestionArray[4] = new Question("素晴らしい人との出会いが人生を豊かにしてくれる。", "", 4);
            this.allQuestionArray[5] = new Question("忙しくて本が読めないとこぼす人が多いが、その気になれば時間はつくれるものだ。", "", 5);
            this.allQuestionArray[6] = new Question("日本でも、週末に多くの時間を子供たちと過ごす父親が増えてきた。", "", 6);
            this.allQuestionArray[7] = new Question("私たちは家に帰る途中、にわか雨にあってびしょ濡れになったうえに、もう少しで道に迷いそうになった。", "", 7);
            this.allQuestionArray[8] = new Question("こんなに面白い本は読んだことがない。一度読み出したらやめられない。", "", 8);
            this.allQuestionArray[9] = new Question("この新法の目的は、公共の場での受動喫煙を防止することだ。", "", 9);
            this.allQuestionArray[10] = new Question("今日ではありとあらゆる類のマニュアルや手引書が氾濫している。", "", 10);
            solvingQuestion = allQuestionArray[0];
        }

        private void Finish()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Question[]));
            string filename = "temy_output_answer.json";
            FileStream fs = new FileStream(filename, FileMode.Create);
            try
            {
                Question[] queArray = solvedQuestionList.ToArray();
                serializer.WriteObject(fs, queArray);
            }
            finally
            {
                fs.Close();
            }
            MainPanel.Visibility = Visibility.Hidden;
            FinTextBlock.Visibility = Visibility.Visible;
        }

    }
}
