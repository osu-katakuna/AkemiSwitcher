
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
            switchOnLoad();
        }

        private void error(string ErrorMessage)
        {
            btnSwitch.Background = this.FindResource("ButtonStateError") as Brush;
            btnSwitch.IsEnabled = true;
            btnSwitch.Content = ErrorMessage;

            versionText.Content = ((App)Application.Current).versionString;
        }

        private void disable(string Message)
        {
            btnSwitch.Background = this.FindResource("ButtonStateDisabled") as Brush;
            btnSwitch.IsEnabled = false;
            btnSwitch.Content = Message;

            versionText.Content = ((App)Application.Current).versionString;
        }

        public void switchOnLoad()
        {
            btnSwitch.Background = ((App)Application.Current).targetServerBrush();
            btnSwitch.Content = ((App)Application.Current).SwitchTo;

            btnSwitch.IsEnabled = true;
            versionText.Content = ((App)Application.Current).versionString;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //disable("lets go");

            //((App)Application.Current).isKatakuna = !((App)Application.Current).isKatakuna;
            //((App)Application.Current).isBancho = !((App)Application.Current).isBancho;

            //switchOnLoad();
            // ((App)Application.Current).JaPierdole();
        }

        private void languageSelectionBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ((App)App.Current).UpdateLanguageByIndex(languageSelectionBox.SelectedIndex);
        }
    }
}
