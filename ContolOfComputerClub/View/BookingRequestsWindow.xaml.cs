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

namespace ControlOfComputerClub.View
{
    /// <summary>
    /// Логика взаимодействия для BookingRequestsWindow.xaml
    /// </summary>
    public partial class BookingRequestsWindow : Window
    {
        private static BookingRequestsWindow? _instance;

        private BookingRequestsWindow()
        {
            InitializeComponent();
            this.Left = Properties.Settings.Default.WindowLeft;
            this.Top = Properties.Settings.Default.WindowTop;
        }

        public static BookingRequestsWindow GetInstance()
        {
            if (_instance == null)
                _instance = new BookingRequestsWindow();
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
