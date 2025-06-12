using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ControlOfComputerClub.Model;
using ControlOfComputerClub.ViewModel.Messages;
using Microsoft.EntityFrameworkCore;

namespace ControlOfComputerClub.ViewModel
{
    public partial class BookingRequestsViewModel : ObservableObject
    {
        private BookingRequest? _originalBookingRequestCopy;

        /// <summary>
        /// Текущая выбранная заявка.
        /// </summary>
        [ObservableProperty]
        private BookingRequest? _currentBookingRequest;

        /// <summary>
        /// Список заявок, загруженных из базы данных.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<BookingRequest> _bookingRequests = new();

        public bool HasErrors => CurrentBookingRequest?.HasErrors ?? false;

        public BookingRequestsViewModel()
        {
            LoadBookingRequests();
            WeakReferenceMessenger.Default.Register<BookingRequest>(this, (r, newBookingRequest) =>
            {
                using var db = new ApplicationDbContext();
                db.BookingRequests.Add(newBookingRequest);
                db.SaveChanges();
                LoadBookingRequests();
                CurrentBookingRequest = BookingRequests.FirstOrDefault(x => x.BookingRequestId == newBookingRequest.BookingRequestId);
            });

        }

        [RelayCommand]
        private void LoadBookingRequests()
        {
            using (var db = new ApplicationDbContext())
            {
                BookingRequests = new ObservableCollection<BookingRequest>(db.BookingRequests.ToList());
            }
            if (BookingRequests.Count > 0)
                CurrentBookingRequest = BookingRequests[0];
        }

        [RelayCommand]
        private void SaveBookingRequest()
        {
            if (CurrentBookingRequest == null)
                return;

            using (var db = new ApplicationDbContext())
            {
                var existing = db.BookingRequests.FirstOrDefault(b => b.BookingRequestId == CurrentBookingRequest.BookingRequestId);
                if (existing != null)
                {
                    existing.EmployeeId = CurrentBookingRequest.EmployeeId;
                    existing.WorkplaceId = CurrentBookingRequest.WorkplaceId;
                    existing.ClientId = CurrentBookingRequest.ClientId;
                    existing.StartTime = CurrentBookingRequest.StartTime;
                    existing.EndTime = CurrentBookingRequest.EndTime;
                    existing.RequestStatus = CurrentBookingRequest.RequestStatus;
                }
                else
                {
                    db.BookingRequests.Add(CurrentBookingRequest);
                }
                try
                {
                    db.SaveChanges();
                    LoadBookingRequests();
                }
                catch (DbUpdateException ex)
                {
                    WeakReferenceMessenger.Default.Send(new ErrorMessage("Ошибка базы данных", ex.InnerException?.Message ?? "Неизвестная ошибка"));
                }
                catch (FormatException ex)
                {
                    WeakReferenceMessenger.Default.Send(new ErrorMessage("Ошибка формата данных", ex.Message));
                }
                catch (Exception ex)
                {
                    WeakReferenceMessenger.Default.Send(new ErrorMessage("Неизвестная ошибка", ex.Message));
                }
            }
        }

        [RelayCommand]
        private void NextBookingRequest()
        {
            if (CurrentBookingRequest == null || BookingRequests.Count == 0)
                return;
            CancelChanges();
            int index = BookingRequests.IndexOf(CurrentBookingRequest);
            if (index < BookingRequests.Count - 1)
                CurrentBookingRequest = BookingRequests[index + 1];
        }

        [RelayCommand]
        private void PreviousBookingRequest()
        {
            if (CurrentBookingRequest == null || BookingRequests.Count == 0)
                return;
            CancelChanges();
            int index = BookingRequests.IndexOf(CurrentBookingRequest);
            if (index > 0)
                CurrentBookingRequest = BookingRequests[index - 1];
        }

        [RelayCommand]
        private void FirstBookingRequest()
        {
            CancelChanges();
            if (BookingRequests.Count > 0)
                CurrentBookingRequest = BookingRequests[0];
        }

        [RelayCommand]
        private void LastBookingRequest()
        {
            CancelChanges();
            if (BookingRequests.Count > 0)
                CurrentBookingRequest = BookingRequests[BookingRequests.Count - 1];
        }

        [RelayCommand]
        private void AddBookingRequest()
        {
            WeakReferenceMessenger.Default.Send(new AddBookingRequestMessage());
            //BookingRequest newRequest = new BookingRequest();
            //CurrentBookingRequest = newRequest;
        }

        [RelayCommand]
        private void DeleteBookingRequest()
        {
            if (CurrentBookingRequest == null)
                return;

            using (var db = new ApplicationDbContext())
            {
                var requestToDelete = db.BookingRequests.FirstOrDefault(b => b.BookingRequestId == CurrentBookingRequest.BookingRequestId);
                if (requestToDelete != null)
                {
                    db.BookingRequests.Remove(requestToDelete);
                    db.SaveChanges();
                    BookingRequests.Remove(CurrentBookingRequest);
                    CurrentBookingRequest = BookingRequests.Count > 0 ? BookingRequests[0] : null;
                }
            }
        }

        partial void OnCurrentBookingRequestChanged(BookingRequest? oldValue, BookingRequest? newValue)
        {
            if (oldValue != null)
                oldValue.ErrorsChanged -= OnErrorsChanged;
            if (newValue != null)
            {
                _originalBookingRequestCopy = new BookingRequest
                {
                    BookingRequestId = newValue.BookingRequestId,
                    EmployeeId = newValue.EmployeeId,
                    WorkplaceId = newValue.WorkplaceId,
                    ClientId = newValue.ClientId,
                    StartTime = newValue.StartTime,
                    EndTime = newValue.EndTime,
                    RequestStatus = newValue.RequestStatus
                };
                newValue.ErrorsChanged += OnErrorsChanged;
            }
            OnPropertyChanged(nameof(HasErrors));
        }

        private void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasErrors));
        }

        private void CancelChanges()
        {
            if (_originalBookingRequestCopy != null && CurrentBookingRequest != null)
            {
                CurrentBookingRequest.EmployeeId = _originalBookingRequestCopy.EmployeeId;
                CurrentBookingRequest.WorkplaceId = _originalBookingRequestCopy.WorkplaceId;
                CurrentBookingRequest.ClientId = _originalBookingRequestCopy.ClientId;
                CurrentBookingRequest.StartTime = _originalBookingRequestCopy.StartTime;
                CurrentBookingRequest.EndTime = _originalBookingRequestCopy.EndTime;
                CurrentBookingRequest.RequestStatus = _originalBookingRequestCopy.RequestStatus;
            }
        }
    }
}