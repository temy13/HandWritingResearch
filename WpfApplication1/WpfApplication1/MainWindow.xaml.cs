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

namespace WpfApplication1
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            EssayWindow lw = new EssayWindow();
            lw.Show();
        }

        private void Q_Window_Button_Click(object sender, RoutedEventArgs e)
        {
            QuestionWindow qw = new QuestionWindow();
            qw.Show();

        }
        private void H_Window_Button_Click(object sender, RoutedEventArgs e)
        {
            HistoryWindow hw = new HistoryWindow();
            hw.Show();
        }
        private void L_Window_Button_Click(object sender, RoutedEventArgs e)
        {
            EssayWindow lw = new EssayWindow();
            lw.Show();
        }
    }
}
