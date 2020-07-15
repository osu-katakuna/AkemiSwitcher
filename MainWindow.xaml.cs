
using System.Windows;
using System.Windows.Media;

namespace AkemiSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isSwitching = false;
        private bool isKatakuna = true;
        private bool isBancho = false;

        private string server
        {
            get
            {
                return !isSwitching ? getCurrentServer() : getTargetServer();
            }
        }

        private string SwitchTo
        {
            get
            {
                return string.Format("Switch to {0}", getTargetServer());
            }
        }

        private string getCurrentServer()
        {
            if (isKatakuna && !isBancho) return "Katakuna";
            if (!isKatakuna && isBancho) return "Bancho";

            return "Other server";
        }

        private string getTargetServer()
        {
            if (isKatakuna) return "Bancho";
            if (isBancho) return "Katakuna";

            return "Katakuna";
        }

        private Brush targetServerBrush()
        {
            if (isKatakuna) return this.FindResource("ButtonStateNormal") as Brush;
            if (isBancho) return this.FindResource("ButtonStateOK") as Brush;

            return this.FindResource("ButtonStateNormal") as Brush;
        }

        private string status
        {
            get
            {
                return isSwitching ? string.Format("switching to {0}...", server) : string.Format("playing on {0}.", server);
            }
        }

        private string versionString
        {
            get
            {
                return string.Format("{0} v{1} - {2}", System.Reflection.Assembly.GetEntryAssembly().GetName().Name, System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(), status);
            }
        }


        public MainWindow()
        {
            InitializeComponent();

            switchOnLoad();
        }

        private void error(string ErrorMessage)
        {
            btnSwitch.Background = this.FindResource("ButtonStateError") as Brush;
            btnSwitch.IsEnabled = true;
            btnSwitch.Content = ErrorMessage;

            versionText.Content = versionString;
        }

        private void disable(string Message)
        {
            btnSwitch.Background = this.FindResource("ButtonStateDisabled") as Brush;
            btnSwitch.IsEnabled = false;
            btnSwitch.Content = Message;

            versionText.Content = versionString;
        }

        private void switchOnLoad()
        {
            btnSwitch.Background = targetServerBrush();
            btnSwitch.Content = SwitchTo;

            btnSwitch.IsEnabled = true;
            versionText.Content = versionString;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            disable("lets go");

            isKatakuna = !isKatakuna;
            isBancho = !isBancho;

            switchOnLoad();
        }
    }
}
