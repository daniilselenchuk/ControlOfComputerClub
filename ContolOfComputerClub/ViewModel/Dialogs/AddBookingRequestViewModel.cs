using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ControlOfComputerClub.Model;
using ControlOfComputerClub.ViewModel.Messages;

namespace ControlOfComputerClub.ViewModel
{
    public partial class AddBookingRequestViewModel : ObservableObject
    {
        [ObservableProperty]
        private BookingRequest _bookingRequest = new BookingRequest();

        public AddBookingRequestViewModel()
        {
            BookingRequest = new BookingRequest
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1) 
            };
            WeakReferenceMessenger.Default.Register<SelectionChosenMessage>(this, HandleSelectionChosen);
        }

        [RelayCommand]
        private void OpenEmployeeSelection()
        {
            WeakReferenceMessenger.Default.Send(new OpenSelectionDialogMessage("Employee"));
        }

        [RelayCommand]
        private void OpenWorkplaceSelection()
        {
            WeakReferenceMessenger.Default.Send(new OpenSelectionDialogMessage("Workplace"));
        }

        [RelayCommand]
        private void OpenClientSelection()
        { 
            WeakReferenceMessenger.Default.Send(new OpenSelectionDialogMessage("Client"));
        }

        [RelayCommand]
        private void Save()
        {
            if (!BookingRequest.HasErrors)
            {
                WeakReferenceMessenger.Default.Send(BookingRequest);
                WeakReferenceMessenger.Default.Send(new CloseAddBookingRequestWindowMessage());
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            WeakReferenceMessenger.Default.Send(new CloseAddBookingRequestWindowMessage());
        }

        private void HandleSelectionChosen(object recipient, SelectionChosenMessage message)
        {
            switch (message.SelectedItem)
            {
                case Employee employee:
                    BookingRequest.EmployeeId = employee.EmployeeId;
                    OnPropertyChanged(nameof(BookingRequest));
                    break;
                case Workplace workplace:
                    BookingRequest.WorkplaceId = workplace.WorkplaceId;
                    OnPropertyChanged(nameof(BookingRequest));
                    break;
                case Client client:
                    BookingRequest.ClientId = client.ClientId;
                    OnPropertyChanged(nameof(BookingRequest));
                    break;
            }
        }
    }
}