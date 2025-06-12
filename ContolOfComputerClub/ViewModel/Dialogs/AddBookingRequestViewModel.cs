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
    }
}