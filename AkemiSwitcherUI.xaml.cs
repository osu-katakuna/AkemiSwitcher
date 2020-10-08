
using System.Windows;
using System.Windows.Media;

namespace AkemiSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AkemiSwitcherUI : Window
    {


        public AkemiSwitcherUI()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).PerformSwitcherAction();
        }

        private void languageSelectionBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ((App)App.Current).UpdateLanguageByIndex(languageSelectionBox.SelectedIndex);
        }
    }
}
