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
    public partial class ClientsViewModel : ObservableObject
    {
        private Client? _originalClientCopy;

        /// <summary>
        /// Показывает текущего выбранного клиента.
        /// </summary>
        [ObservableProperty]
        private Client? _currentClient;

        /// <summary>
        /// Список клиентов, в котором хранятся данные из базы данных.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Client> _clients = new();

        public bool HasErrors => CurrentClient?.HasErrors ?? false;

        public ClientsViewModel()
        {
            LoadClients();
        }

        [RelayCommand]
        private void LoadClients()
        {
            using var db = new ApplicationDbContext();

            var list = db.Clients
                .Select(c => new Client
                {
                    ClientId = c.ClientId,
                    Name = c.Name,
                    PhoneNumber = c.PhoneNumber,
                    AmountSpent = db.V_ClientAmountSpent
                                    .Where(v => v.ClientId == c.ClientId)
                                    .Select(v => v.AmountSpent)
                                    .FirstOrDefault(),
                    Discount = db.Clients
                                 .Where(x => x.ClientId == c.ClientId)
                                 .Select(x => x.Discount)
                                 .FirstOrDefault()
                })
                .ToList();

            Clients = new ObservableCollection<Client>(list);
            CurrentClient = Clients.FirstOrDefault();
        }




        [RelayCommand]
        private void SaveClient()
        {
            if (CurrentClient == null) return;

            using var db = new ApplicationDbContext();
            var existing = db.Clients.FirstOrDefault(c => c.ClientId == CurrentClient!.ClientId);

            if (existing != null)
            {
                existing.Name = CurrentClient.Name;
                existing.PhoneNumber = CurrentClient.PhoneNumber;
                // НЕ ТРОГАТЬ existing.AmountSpent — он пересчитается триггером на стороне БД
            }
            else
            {
                db.Clients.Add(CurrentClient);
            }
            try
            {
                db.SaveChanges();
                LoadClients();
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


        [RelayCommand]
        private void NextClient()
        {
            if (CurrentClient == null || Clients.Count == 0) return;
            CancelChanges();
            int index = Clients.IndexOf(CurrentClient);
            if (index < Clients.Count - 1)
                CurrentClient = Clients[index + 1];
        }

        [RelayCommand]
        private void PreviousClient()
        {
            if (CurrentClient == null || Clients.Count == 0) return;
            CancelChanges();
            int index = Clients.IndexOf(CurrentClient);
            if (index > 0)
                CurrentClient = Clients[index - 1];
        }

        [RelayCommand]
        private void FirstClient()
        {
            CancelChanges();
            if (Clients.Count > 0)
                CurrentClient = Clients[0];
        }

        [RelayCommand]
        private void LastClient()
        {
            CancelChanges();
            if (Clients.Count > 0)
                CurrentClient = Clients[Clients.Count - 1];
        }

        [RelayCommand]
        private void AddClient()
        {
            Client newClient = new Client();
            CurrentClient = newClient;
            CurrentClient.Validate();
        }

        [RelayCommand]
        private void DeleteClient()
        {
            if (CurrentClient == null) return;
            using (var db = new ApplicationDbContext())
            {
                var clientToDelete = db.Clients.FirstOrDefault(c => c.ClientId == CurrentClient.ClientId);
                if (clientToDelete != null)
                {
                    db.Clients.Remove(clientToDelete);
                    db.SaveChanges();
                    Clients.Remove(CurrentClient);
                    CurrentClient = Clients.Count > 0 ? Clients[0] : null;
                }
            }
        }

        partial void OnCurrentClientChanged(Client? oldValue, Client? newValue)
        {
            if (oldValue != null)
            {
                oldValue.ErrorsChanged -= OnErrorsChanged;
            }
            if (newValue != null)
            {
                _originalClientCopy = new Client
                {
                    ClientId = newValue.ClientId,
                    PhoneNumber = newValue.PhoneNumber,
                    Name = newValue.Name,
                    AmountSpent = newValue.AmountSpent
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
            if (_originalClientCopy != null && CurrentClient != null)
            {
                CurrentClient.PhoneNumber = _originalClientCopy.PhoneNumber;
                CurrentClient.Name = _originalClientCopy.Name;
                CurrentClient.AmountSpent = _originalClientCopy.AmountSpent;
            }
        }
    }
}