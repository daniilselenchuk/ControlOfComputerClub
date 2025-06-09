using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControlOfComputerClub.Model;

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

        public ClientsViewModel()
        {
            LoadClients();
        }

        [RelayCommand]
        private void LoadClients()
        {
            using (var db = new ApplicationDbContext())
            {
                Clients = new ObservableCollection<Client>(db.Clients.ToList());
            }
            if (Clients.Count > 0)
                CurrentClient = Clients[0];
        }

        [RelayCommand]
        private void SaveClient()
        {
            if (CurrentClient == null) return;

            using (var db = new ApplicationDbContext())
            {
                var existingClient = db.Clients.FirstOrDefault(c => c.ClientId == CurrentClient.ClientId);
                if (existingClient != null)
                {
                    existingClient.PhoneNumber = CurrentClient.PhoneNumber;
                    existingClient.Name = CurrentClient.Name;
                    existingClient.AmountSpent = CurrentClient.AmountSpent;
                }
                else
                {
                    db.Clients.Add(CurrentClient);
                }
                db.SaveChanges();
                LoadClients();
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
            if (Clients.Count > 0)
                CurrentClient = Clients[0];
        }

        [RelayCommand]
        private void LastClient()
        {
            if (Clients.Count > 0)
                CurrentClient = Clients[Clients.Count - 1];
        }

        [RelayCommand]
        private void AddClient()
        {
            Client newClient = new Client();
            CurrentClient = newClient;
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
            if (newValue != null)
            {
                _originalClientCopy = new Client
                {
                    ClientId = newValue.ClientId,
                    PhoneNumber = newValue.PhoneNumber,
                    Name = newValue.Name,
                    AmountSpent = newValue.AmountSpent
                };
            }
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