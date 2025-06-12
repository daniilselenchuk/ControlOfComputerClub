using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ControlOfComputerClub.Model;
using ControlOfComputerClub.ViewModel.Messages;

namespace ControlOfComputerClub.ViewModel
{
    public partial class AddClientViewModel : ObservableObject
    {
        [ObservableProperty]
        private Client _client = new Client();

        public AddClientViewModel()
        {
            Client.Validate();
        }

        [RelayCommand]
        private void Save()
        {
            if (!Client.HasErrors)
            {
                WeakReferenceMessenger.Default.Send(Client);
                WeakReferenceMessenger.Default.Send(new CloseAddClientWindowMessage());
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            WeakReferenceMessenger.Default.Send(new CloseAddClientWindowMessage());
        }
    }
}