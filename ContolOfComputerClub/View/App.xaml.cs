using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using ControlOfComputerClub.ViewModel;
using ControlOfComputerClub.ViewModel.Messages;
using ControlOfComputerClub.View.Dialogs;
using System.IO;
using ControlOfComputerClub.Model;
using System.Collections.ObjectModel;

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
            RegisterErrorHandler();
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
            WeakReferenceMessenger.Default.Register<OpenFileDialogMessage>(this, HandleOpenFileDialogMessage);
            WeakReferenceMessenger.Default.Register<AddClientMessage>(this, HandleAddClientMessage);
            WeakReferenceMessenger.Default.Register<AddWorkplaceMessage>(this, HandleAddWorkplaceMessage);
            WeakReferenceMessenger.Default.Register<AddBookingRequestMessage>(this, HandleAddBookingRequestMessage);
            WeakReferenceMessenger.Default.Register<OpenSelectionDialogMessage>(this, HandleOpenSelectionDialogMessage);
            WeakReferenceMessenger.Default.Register<CloseSelectionDialogMessage>(this, HandleCloseSelectionDialogMessage);
            WeakReferenceMessenger.Default.Register<OpenQueryWindowMessage>(this, HandleOpenQueryWindowMessage);
            WeakReferenceMessenger.Default.Register<OpenWorkplaceSelectionDialogMessage>(this, HandleOpenWorkplaceSelectionMessage);

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

        private void HandleOpenFileDialogMessage(object recipient, OpenFileDialogMessage message)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    byte[] imageBytes = File.ReadAllBytes(dlg.FileName);
                    WeakReferenceMessenger.Default.Send(new FileSelectedMessage(imageBytes));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void HandleAddClientMessage(object recipient, AddClientMessage message)
        {
            AddClientWindow addClientWindow = new AddClientWindow();
            if (addClientWindow.DataContext == null)
            {
                AddClientViewModel addClientViewModel = new AddClientViewModel();
                addClientWindow.DataContext = addClientViewModel;
            }
            RegisterAddClientWindowClose(addClientWindow);
            addClientWindow.ShowDialog();
        }
        private void HandleAddWorkplaceMessage(object recipient, AddWorkplaceMessage message)
        {
            var window = new Dialogs.AddWorkplaceWindow();
            if (window.DataContext == null)
            {
                var vm = new AddWorkplaceViewModel();
                window.DataContext = vm;
            }
            RegisterAddWorkplaceWindowClose(window);
            window.ShowDialog();
        }

        private void HandleAddBookingRequestMessage(object recipient, AddBookingRequestMessage message)
        {
            var window = new Dialogs.AddBookingRequestWindow();
            if (window.DataContext == null)
            {
                var vm = new AddBookingRequestViewModel();
                window.DataContext = vm;
            }
            RegisterAddBookingRequestWindowClose(window);
            window.ShowDialog();
        }

        private void HandleOpenSelectionDialogMessage(object recipient, OpenSelectionDialogMessage message)
        {
            ObservableCollection<object> items;

            using var db = new ApplicationDbContext();
            items = message.SelectionType switch
            {
                "Employee" => new ObservableCollection<object>(db.Employees.ToList()),
                "Workplace" => new ObservableCollection<object>(db.Workplaces.ToList()),
                "Client" => new ObservableCollection<object>(db.Clients.ToList()),
                _ => new ObservableCollection<object>()
            };

            var window = new SelectionDialogWindow
            {
                DataContext = new SelectionDialogViewModel(items)
            };
            window.ShowDialog();
        }

        private void HandleCloseSelectionDialogMessage(object recipient, CloseSelectionDialogMessage message)
        {
            var window = Application.Current.Windows.OfType<SelectionDialogWindow>().FirstOrDefault();
            window?.Close();
        }

        private void HandleOpenQueryWindowMessage(object recipient, OpenQueryWindowMessage message)
        {
            var window = new QueryWindow();
            if (window.DataContext == null)
            {
                var viewModel = new QueryViewModel();
                window.DataContext = viewModel;
            }
            window.ShowDialog();
        }
        private void HandleOpenWorkplaceSelectionMessage(object recipient, OpenWorkplaceSelectionDialogMessage message)
        {
            var workplaces = new ObservableCollection<object>(message.Workplaces.Cast<object>().ToList());
            var window = new SelectionDialogWindow
            {
                DataContext = new SelectionDialogViewModel(workplaces)
            };
            var result = window.ShowDialog();
            if (result == true)
            {
                var viewModel = (SelectionDialogViewModel)window.DataContext;
                if (viewModel.SelectedItem is Workplace selectedWorkplace)
                {
                    WeakReferenceMessenger.Default.Send(new WorkplaceSelectedMessage(selectedWorkplace));
                }
            }
        }

        private void RegisterAddClientWindowClose(Window window)
        {
            WeakReferenceMessenger.Default.Register<CloseAddClientWindowMessage>(window, (r, m) =>
            {
                window.Close();
            });
        }

        private void RegisterAddWorkplaceWindowClose(Window window)
        {
            WeakReferenceMessenger.Default.Register<CloseAddWorkplaceWindowMessage>(window, (r, m) =>
            {
                window.Close();
            });
        }

        private void RegisterErrorHandler()
        {
            WeakReferenceMessenger.Default.Register<ErrorMessage>(this, (recipient, message) =>
            {
                MessageBox.Show(message.Message, message.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        private void RegisterAddBookingRequestWindowClose(Window window)
        {
            WeakReferenceMessenger.Default.Register<CloseAddBookingRequestWindowMessage>(window, (r, m) =>
            {
                window.Close();
            });
        }

    }
}