using BlackjackGame.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlackjackGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new BlackjackViewModel();
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var viewModel = DataContext as BlackjackViewModel;
            if (viewModel == null)
                return;

            switch (e.Key)
            {
                case Key.H:
                    if (viewModel.HitCommand.CanExecute(null))
                    {
                        viewModel.HitCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;

                case Key.S:
                    if (viewModel.StandCommand.CanExecute(null))
                    {
                        viewModel.StandCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;
            }
        }
    }
}