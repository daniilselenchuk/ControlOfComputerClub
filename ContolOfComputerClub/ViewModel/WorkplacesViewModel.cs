using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControlOfComputerClub.Model;

namespace ControlOfComputerClub.ViewModel
{
    public partial class WorkplacesViewModel : ObservableObject
    {
        private Workplace? _workplaceCopy;

        [ObservableProperty]
        private Workplace? _currentWorkplace;

        [ObservableProperty]
        private ObservableCollection<Workplace> _workplaces = new();

        public bool HasErrors => _workplaceCopy?.HasErrors ?? false;

        public WorkplacesViewModel()
        {
            LoadWorkplaces();
        }

        [RelayCommand]
        private void LoadWorkplaces()
        {
            using (var db = new ApplicationDbContext())
            {
                Workplaces = new ObservableCollection<Workplace>(db.Workplaces.ToList());
            }
            if (Workplaces.Count > 0)
                CurrentWorkplace = Workplaces[0];
        }

        [RelayCommand]
        private void SaveWorkplace()
        {
            if (_workplaceCopy == null) return;

            using (var db = new ApplicationDbContext())
            {
                var existingWorkplace = db.Workplaces.FirstOrDefault(w => w.WorkplaceId == _workplaceCopy.WorkplaceId);
                if (existingWorkplace != null)
                {
                    existingWorkplace.Status = _workplaceCopy.Status;
                    existingWorkplace.Tariff = _workplaceCopy.Tariff;
                    existingWorkplace.PriceOfWorkplace = _workplaceCopy.PriceOfWorkplace;
                    existingWorkplace.Configuration = _workplaceCopy.Configuration;
                }
                else
                {
                    db.Workplaces.Add(_workplaceCopy);
                }
                db.SaveChanges();
            }

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
            _workplaceCopy = new Workplace();
            CurrentWorkplace = _workplaceCopy;
            CurrentWorkplace.Validate();
        }

        [RelayCommand]
        private void DeleteWorkplace()
        {
            if (CurrentWorkplace == null) return;
            using (var db = new ApplicationDbContext())
            {
                var workplaceToDelete = db.Workplaces.FirstOrDefault(w => w.WorkplaceId == CurrentWorkplace.WorkplaceId);
                if (workplaceToDelete != null)
                {
                    db.Workplaces.Remove(workplaceToDelete);
                    db.SaveChanges();
                    Workplaces.Remove(CurrentWorkplace);
                    CurrentWorkplace = Workplaces.Count > 0 ? Workplaces[0] : null;
                }
            }
        }

        partial void OnCurrentWorkplaceChanged(Workplace? oldValue, Workplace? newValue)
        {
            if (oldValue != null)
                oldValue.ErrorsChanged -= OnErrorsChanged;

            if (newValue != null)
            {
                _workplaceCopy = new Workplace
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
            if (_workplaceCopy != null && CurrentWorkplace != null)
            {
                _workplaceCopy.Status = CurrentWorkplace.Status;
                _workplaceCopy.Tariff = CurrentWorkplace.Tariff;
                _workplaceCopy.PriceOfWorkplace = CurrentWorkplace.PriceOfWorkplace;
                _workplaceCopy.Configuration = CurrentWorkplace.Configuration;
            }
        }
    }
}