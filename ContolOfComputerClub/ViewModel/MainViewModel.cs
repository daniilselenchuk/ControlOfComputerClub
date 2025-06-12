using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ControlOfComputerClub.Model;
using ControlOfComputerClub.ViewModel.Messages;
using System.Collections.ObjectModel;

namespace ControlOfComputerClub.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        [RelayCommand]
        private void OpenClientsWindow()
        {
            WeakReferenceMessenger.Default.Send(new OpenClientsWindowMessage());
        }

        [RelayCommand]
        private void OpenBookingRequestsWindow()
        {
            WeakReferenceMessenger.Default.Send(new OpenBookingRequestsWindowMessage());
        }

        [RelayCommand]
        private void OpenEmployeesWindow()
        {
            WeakReferenceMessenger.Default.Send(new OpenEmployeesWindowMessage());
        }

        [RelayCommand]
        private void OpenWorkplacesWindow()
        {
            WeakReferenceMessenger.Default.Send(new OpenWorkplacesWindowMessage());
        }

        [RelayCommand]
        private void OpenQueryWindow()
        {
            WeakReferenceMessenger.Default.Send(new OpenQueryWindowMessage());
        }

        [RelayCommand]
        private void Exit()
        {
            WeakReferenceMessenger.Default.Send(new ExitMessage());
        }

        [RelayCommand]
        private void ShowAbout()
        {
            WeakReferenceMessenger.Default.Send(new ShowAboutMessage());
        }
    }
}