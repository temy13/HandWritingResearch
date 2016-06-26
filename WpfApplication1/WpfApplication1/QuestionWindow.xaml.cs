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
    /// 
    public partial class QuestionWindow : Window
    {
        // User writing info
        Stopwatch sw = new Stopwatch();
        private List<Stroke> strokeList = new List<Stroke>();
        private int strokeNumber = 0;
        private List<Question> solvedQuestionList = new List<Question>();
        private Question solvingQuestion;
        private Question[] allQuestionArray = new Question[11];
        private Boolean isWrite;

        private string userName = "nappaeasy2";
        // 1 2


        RenderTargetBitmap bitmap;
        public QuestionWindow()
        {
            InitializeComponent();
            allQuestionArraySet();
            QuestionSet();
            isWrite = true;
        }
        private Point ptLast;
        private Brush brush;
        private int brushSize = 1;
        private DrawingVisual drawVisual = new DrawingVisual();
        private DrawingContext drawContext;
       

        private void QuestionSet()
        {
            bitmap = new RenderTargetBitmap(800, 500, 96, 96, PixelFormats.Default);
            image1.Width = 800;
            image1.Height = 500;
            image1.Source = bitmap;
            QuestionTextBlock.Text = "Q. " + solvingQuestion.question;
            AnswerTextBlock.Text = "A. ";
            AnswerButtons.Visibility = Visibility.Hidden;
            strokeNumber = 1;
            sw.Start();
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
            st.SetParam(strokeNumber, strokeList.Count() + 1, isWrite);
            if (isWrite)
            {
                brush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                brushSize = 1;
            }else
            {
                brush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                brushSize = 5;
            }
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
            st.SetParam(strokeNumber, strokeList.Count() + 1, isWrite);
            drawContext = drawVisual.RenderOpen();
            Pen penDraw = new Pen(brush, 3);
            drawContext.DrawLine(penDraw, ptLast, pt); // 前回の位置からの直線を描画 
            drawContext.DrawEllipse(brush, null, pt, brushSize, brushSize);// 最終座標に円を描画 
            drawContext.Close();
            bitmap.Render(drawVisual);
            ptLast = pt;
            strokeList.Add(st);
        }

        private void ModeChangeButton_Click(object sender, RoutedEventArgs e)
        {
            isWrite = !isWrite;
            if (isWrite)
            {
                ModeChangeButton.Content = "Normal";
            }else
            {
                ModeChangeButton.Content = "Eraser";
            }
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
            string filename = userName + "_output" + solvingN.ToString() + ".json";
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
            /*this.allQuestionArray[1] = new Question("日本は昔と同じような意味で貧しいのではないということもできるけれど、必ずしも貧困と不平等がなくなったわけではない", "", 1);
            this.allQuestionArray[2] = new Question("友達と一緒にいる時は、スマートフォンで頻繁にメールをチェックすることによって友達の気分を害することがないように気を付けましょう", "", 2);
            this.allQuestionArray[3] = new Question("機械が発達して、その結果人々の仕事を肩代わりすれば人々は自由な時間がもっと手に入ると、多くの人が思っていた", "", 3);
            this.allQuestionArray[4] = new Question("第二言語を習得することが脳の力を高めるという諸研究があるので、私の子供にも外国語を習わせたい", "", 4);
            this.allQuestionArray[5] = new Question("この橋を一緒に渡すカップルについて言われていること、しってる？", "", 5);
            this.allQuestionArray[6] = new Question("子供たちは克服しなければいけない障害をたくさん抱えていますから、ぜひ根気よくやってくださいね。", "", 6);
            this.allQuestionArray[7] = new Question("どんな謎めいた話でも、結末がどうなるかわかってしまうと、あまりわくわくしなくなってしまうわ", "", 7);
            this.allQuestionArray[8] = new Question("名作を読むには長い時間がかかるかもしれないが、その経験からあなたは必ず何か価値あるものを得るだろう", "", 8);
            this.allQuestionArray[9] = new Question("25年後までに沖縄県の平均寿命を日本一に復活させるという目標に向け、息の長い活動が始まっている", "", 9);
            this.allQuestionArray[10] = new Question("2000年、男性の平均寿命が全国1位から26位に転落して以来、県を先頭に多くの団体が県民に健康づくりを呼び掛けた", "", 10);
            this.allQuestionArray[11] = new Question("死亡率の高さは、職の欧米化や車社会の影響で勤労者世代の肥満率が上昇し、それに伴い生活習慣病が増加したことなどが主な原因といわれる", "", 11);
            this.allQuestionArray[12] = new Question("人は自らの体の動きや他者との相互作用を通じて、どのようにして言葉の意味を身につけていくのか", "", 12);
            this.allQuestionArray[13] = new Question("これらはすべて、コミュニケーションについて基本的な問いであり、医療、防災、教育、その他多くの実践に結び付く、極めて重要な問でもある", "", 13);
            this.allQuestionArray[14] = new Question("コミュニケーションとは、人々の共存と情報の共有のために、人間や社会の根本的な基本に他ならない", "", 14);*/
            this.allQuestionArray[1] = new Question("ひかえめにいっても、嘘をつくことは悪い習慣です", "Lying is a bad habit, to say the least of it.", 1);
            this.allQuestionArray[2] = new Question("私の母は私があまりに怠惰だと文句を言う", "My mother complains of my being too lazy.", 2);
            this.allQuestionArray[3] = new Question("私は緊張しています。たくさんの聴衆に話をするのは慣れていないのです。", "I'm nervous. I'm not used to speaking to a large audience.", 3);
            this.allQuestionArray[4] = new Question("彼は歴史上きわめて偉大な学者である。", "He is as great a scholar as ever lived.", 4);
            this.allQuestionArray[5] = new Question("勿論彼はとても素晴らしい作家ですが、学者というよりもジャーナリストです", "Of course he is quite a good write, but he is ajournalist rather than a scholar", 5);
            this.allQuestionArray[6] = new Question("出席者はすぐに彼の提案を受け入れた", "Those present accepted his proposal at once.", 6);
            this.allQuestionArray[7] = new Question("私のカーテンは、この家具にあっている", "My curtains go with this furniture", 7);
            this.allQuestionArray[8] = new Question("彼女は怪我をしたと思ったが、実際は、そうではなかった", "She thought she was hurt, but that wansnt't  really true/the case.", 8);
            this.allQuestionArray[9] = new Question("その国は天然資源に恵まれている", "The country is rich in natural resources.", 9);
            this.allQuestionArray[10] = new Question("私は彼がアメリカに行った理由を知らない", "I am ignorant of his reason for going to America.", 10);
            solvingQuestion = allQuestionArray[0];
        }

        private void Finish()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Question[]));
            string filename = userName + "_output_answer.json";
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
