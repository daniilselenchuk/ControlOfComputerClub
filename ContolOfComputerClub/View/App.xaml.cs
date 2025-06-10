using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using ControlOfComputerClub.ViewModel;
using ControlOfComputerClub.ViewModel.Messages;

namespace ControlOfComputerClub.View
{
    /// <summary>
    /// Логика взаимодействия для App.xaml.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mainWindow = new MainWindow();
            MainViewModel mainViewModel = new MainViewModel();
            mainWindow.DataContext = mainViewModel;
            ProcessMessages();
            mainWindow.Show();
        }
        private void ProcessMessages()
        {
            WeakReferenceMessenger.Default.Register<ExitMessage>(this, HandleExitMessage);
            WeakReferenceMessenger.Default.Register<ShowAboutMessage>(this, HandleShowAboutMessage);
            WeakReferenceMessenger.Default.Register<OpenClientsWindowMessage>(this, HandleOpenClientsWindowMessage);
            WeakReferenceMessenger.Default.Register<OpenBookingRequestsWindowMessage>(this, HandleOpenBookingRequestsWindowMessage);
            WeakReferenceMessenger.Default.Register<OpenEmployeesWindowMessage>(this, HandleOpenEmployeesWindowMessage);
            WeakReferenceMessenger.Default.Register<OpenWorkplacesWindowMessage>(this, HandleOpenWorkplacesWindowMessage);
        }

        private void HandleExitMessage(object recipient, ExitMessage message)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти?", "Выход",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) Application.Current.Shutdown();

        }

        private void HandleShowAboutMessage(object recipient, ShowAboutMessage message)
        {
            MessageBox.Show("(C)ТУСУР, КСУП, Селенчук Даниил Олегович, группа 573-2, 2025","О программе", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void HandleOpenClientsWindowMessage(object recipient, OpenClientsWindowMessage message)
        {
            ClientsWindow clientsWindow = ClientsWindow.GetInstance();
            if (clientsWindow.DataContext == null)
            {
                ClientsViewModel clientsViewModel = new ClientsViewModel();
                clientsWindow.DataContext = clientsViewModel;
            }
            clientsWindow.Show();
            clientsWindow.Activate();
        }

        private void HandleOpenBookingRequestsWindowMessage(object recipient, OpenBookingRequestsWindowMessage message)
        {
            BookingRequestsWindow bookingRequestsWindow = BookingRequestsWindow.GetInstance();
            if (bookingRequestsWindow.DataContext == null)
            {
                BookingRequestsViewModel bookingRequestsViewModel = new BookingRequestsViewModel();
                bookingRequestsWindow.DataContext = bookingRequestsViewModel;
            }
            bookingRequestsWindow.Show();
            bookingRequestsWindow.Activate();
        }

        private void HandleOpenEmployeesWindowMessage(object recipient, OpenEmployeesWindowMessage message)
        {
            EmployeesWindow employeesWindow = EmployeesWindow.GetInstance();
            if (employeesWindow.DataContext == null)
            {
                EmployeesViewModel employeesViewModel = new EmployeesViewModel();
                employeesWindow.DataContext = employeesViewModel;
            }
            employeesWindow.Show();
            employeesWindow.Activate();
        }
        private void HandleOpenWorkplacesWindowMessage(object recipient, OpenWorkplacesWindowMessage message)
        {
            WorkplacesWindow workplacesWindow = WorkplacesWindow.GetInstance();
            if (workplacesWindow.DataContext == null)
            {
                WorkplacesViewModel workplacesViewModel = new WorkplacesViewModel();
                workplacesWindow.DataContext = workplacesViewModel;
            }
            workplacesWindow.Show();
            workplacesWindow.Activate();
        }
    }

}
