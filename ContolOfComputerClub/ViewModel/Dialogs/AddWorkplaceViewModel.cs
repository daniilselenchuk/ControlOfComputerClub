using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ControlOfComputerClub.Model;
using ControlOfComputerClub.ViewModel.Messages;

namespace ControlOfComputerClub.ViewModel
{
    public partial class AddWorkplaceViewModel : ObservableObject
    {
        [ObservableProperty]
        private Workplace _workplace = new Workplace();

        public AddWorkplaceViewModel()
        {
            Workplace.Validate();
        }

        [RelayCommand]
        private void Save()
        {
            if (!Workplace.HasErrors)
            {
                WeakReferenceMessenger.Default.Send(Workplace);
                WeakReferenceMessenger.Default.Send(new CloseAddWorkplaceWindowMessage());
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            WeakReferenceMessenger.Default.Send(new CloseAddWorkplaceWindowMessage());
        }
    }
}