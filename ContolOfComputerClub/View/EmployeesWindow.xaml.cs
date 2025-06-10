using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ControlOfComputerClub.View
{
    /// <summary>
    /// Логика взаимодействия для EmployeesWindow.xaml
    /// </summary>
    public partial class EmployeesWindow : Window
    {
        private static EmployeesWindow? _instance;

        private EmployeesWindow()
        {
            InitializeComponent();
            this.Left = Properties.Settings.Default.WindowLeft;
            this.Top = Properties.Settings.Default.WindowTop;
        }

        public static EmployeesWindow GetInstance()
        {
            if (_instance == null)
                _instance = new EmployeesWindow();
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
