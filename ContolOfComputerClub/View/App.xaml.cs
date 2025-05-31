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
        }

        private void HandleExitMessage(object recipient, ExitMessage message)
        {
            Application.Current.Shutdown();
        }

        private void HandleShowAboutMessage(object recipient, ShowAboutMessage message)
        {
            MessageBox.Show("(C)ТУСУР, КСУП, Селенчук Даниил Олегович, группа 573-2, 2025","О программе", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

}
