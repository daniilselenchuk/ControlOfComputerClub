using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ControlOfComputerClub.ViewModel.Messages;

namespace ControlOfComputerClub.ViewModel
{
    public partial class SelectionDialogViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<object> _availableItems = new();

        [ObservableProperty]
        private object _selectedItem;

        public SelectionDialogViewModel(ObservableCollection<object> items)
        {
            AvailableItems = items;
        }


        [RelayCommand]
        private void ConfirmSelection()
        {
            if (SelectedItem != null)
            {
                WeakReferenceMessenger.Default.Send(new SelectionChosenMessage(SelectedItem));
            }
            WeakReferenceMessenger.Default.Send(new CloseSelectionDialogMessage());
        }

        [RelayCommand]
        private void CloseWindow()
        {
            WeakReferenceMessenger.Default.Send(new CloseSelectionDialogMessage());
        }

        public static object SelectionResult { get; private set; }
    }
}