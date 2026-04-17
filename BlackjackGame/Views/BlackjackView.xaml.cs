using System.Windows;
using System.Windows.Controls;
using BlackjackGame.ViewModels;


namespace BlackjackGame.Views
{
    /// <summary>
    /// Interaction logic for BlackjackView.xaml        
    /// </summary>
    public partial class BlackjackView : UserControl
    {
        public BlackjackView()
        {
            InitializeComponent();
            DataContext = new BlackjackViewModel();
        }
    }
}
