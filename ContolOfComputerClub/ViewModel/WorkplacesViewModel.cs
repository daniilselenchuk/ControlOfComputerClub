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
    public partial class WorkplacesViewModel : ObservableObject
    {
        private Workplace? _originalWorkplaceCopy;

        [ObservableProperty]
        private Workplace? _currentWorkplace;

        [ObservableProperty]
        private ObservableCollection<Workplace> _workplaces = new();

        public bool HasErrors => CurrentWorkplace?.HasErrors ?? false;

        public WorkplacesViewModel()
        {
            LoadWorkplaces();
            WeakReferenceMessenger.Default.Register<Workplace>(this, (r, newWorkplace) =>
            {
                using var db = new ApplicationDbContext();
                db.Workplaces.Add(newWorkplace);
                db.SaveChanges();
                LoadWorkplaces();
                CurrentWorkplace = Workplaces.FirstOrDefault(x => x.WorkplaceId == newWorkplace.WorkplaceId);
            });
        }

        [RelayCommand]
        private void LoadWorkplaces()
        {
            using var db = new ApplicationDbContext();
            Workplaces = new ObservableCollection<Workplace>(db.Workplaces.ToList());
            CurrentWorkplace = Workplaces.FirstOrDefault();
        }

        [RelayCommand]
        private void SaveWorkplace()
        {
            if (CurrentWorkplace == null) return;

            using var db = new ApplicationDbContext();
            var existing = db.Workplaces.FirstOrDefault(w => w.WorkplaceId == CurrentWorkplace.WorkplaceId);

            if (existing != null)
            {
                existing.Status = CurrentWorkplace.Status;
                existing.Tariff = CurrentWorkplace.Tariff;
                existing.PriceOfWorkplace = CurrentWorkplace.PriceOfWorkplace;
                existing.Configuration = CurrentWorkplace.Configuration;
            }
            else
            {
                db.Workplaces.Add(CurrentWorkplace);
            }

            db.SaveChanges();
            LoadWorkplaces();
        }

        [RelayCommand]
        private void NextWorkplace()
        {
            if (CurrentWorkplace == null || Workplaces.Count == 0) return;
            CancelChanges();
            int index = Workplaces.IndexOf(CurrentWorkplace);
            if (index < Workplaces.Count - 1)
                CurrentWorkplace = Workplaces[index + 1];
        }

        [RelayCommand]
        private void PreviousWorkplace()
        {
            if (CurrentWorkplace == null || Workplaces.Count == 0) return;
            CancelChanges();
            int index = Workplaces.IndexOf(CurrentWorkplace);
            if (index > 0)
                CurrentWorkplace = Workplaces[index - 1];
        }

        [RelayCommand]
        private void FirstWorkplace()
        {
            CancelChanges();
            if (Workplaces.Count > 0)
                CurrentWorkplace = Workplaces[0];
        }

        [RelayCommand]
        private void LastWorkplace()
        {
            CancelChanges();
            if (Workplaces.Count > 0)
                CurrentWorkplace = Workplaces[Workplaces.Count - 1];
        }

        [RelayCommand]
        private void AddWorkplace()
        {
            WeakReferenceMessenger.Default.Send(new AddWorkplaceMessage());
        }

        [RelayCommand]
        private void DeleteWorkplace()
        {
            if (CurrentWorkplace == null) return;
            using var db = new ApplicationDbContext();
            var workplaceToDelete = db.Workplaces.FirstOrDefault(w => w.WorkplaceId == CurrentWorkplace.WorkplaceId);
            if (workplaceToDelete != null)
            {
                db.Workplaces.Remove(workplaceToDelete);
                db.SaveChanges();
                Workplaces.Remove(CurrentWorkplace);
                CurrentWorkplace = Workplaces.Count > 0 ? Workplaces[0] : null;
            }
        }

        partial void OnCurrentWorkplaceChanged(Workplace? oldValue, Workplace? newValue)
        {
            if (oldValue != null)
                oldValue.ErrorsChanged -= OnErrorsChanged;

            if (newValue != null)
            {
                _originalWorkplaceCopy = new Workplace
                {
                    WorkplaceId = newValue.WorkplaceId,
                    Status = newValue.Status,
                    Tariff = newValue.Tariff,
                    PriceOfWorkplace = newValue.PriceOfWorkplace,
                    Configuration = newValue.Configuration
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
            if (_originalWorkplaceCopy != null && CurrentWorkplace != null)
            {
                CurrentWorkplace.Status = _originalWorkplaceCopy.Status;
                CurrentWorkplace.Tariff = _originalWorkplaceCopy.Tariff;
                CurrentWorkplace.PriceOfWorkplace = _originalWorkplaceCopy.PriceOfWorkplace;
                CurrentWorkplace.Configuration = _originalWorkplaceCopy.Configuration;
            }
        }
    }
}