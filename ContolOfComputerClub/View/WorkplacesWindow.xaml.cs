using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ControlOfComputerClub.View
{
    /// <summary>
    /// Логика взаимодействия для WorkplacesWindow.xaml
    /// </summary>
    public partial class WorkplacesWindow : Window
    {
        private static WorkplacesWindow? _instance;
        private WorkplacesWindow()
        {
            InitializeComponent();
            this.Left = Properties.Settings.Default.WindowLeft;
            this.Top = Properties.Settings.Default.WindowTop;
        }

        public static WorkplacesWindow GetInstance()
        {
            if (_instance == null)
                _instance = new WorkplacesWindow();
            return _instance;
        }

        protected override void OnClosed(EventArgs e)
        {
            Properties.Settings.Default.WindowLeft = this.Left;
            Properties.Settings.Default.WindowTop = this.Top;
            Properties.Settings.Default.Save();
            base.OnClosed(e);
            _instance = null;
        }
    }
}
