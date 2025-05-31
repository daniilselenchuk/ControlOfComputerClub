using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ControlOfComputerClub.ViewModel;
using ViewModel.Messages;

namespace ControlOfComputerClub.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {

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