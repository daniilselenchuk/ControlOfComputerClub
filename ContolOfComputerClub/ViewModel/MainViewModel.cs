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
        [ObservableProperty]
        private ObservableCollection<Employee>? _employees;

        [ObservableProperty]
        private ObservableCollection<BookingRequest>? _bookingRequests;

        [ObservableProperty]
        private Employee? _selectedEmployee;

        public MainViewModel()
        {
            LoadEmployees();
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

        private void LoadEmployees()
        {
            using (var db = new ApplicationDbContext())
            {
                Employees = new ObservableCollection<Employee>(db.Employees.ToList());
            }
        }

        private void LoadBookingRequests()
        {
            using (var db = new ApplicationDbContext())
            {
                BookingRequests = new ObservableCollection<BookingRequest>(db.BookingRequests.ToList());
            }
        }
    }
}