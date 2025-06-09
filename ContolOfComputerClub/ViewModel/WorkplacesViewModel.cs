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
        private Workplace? _originalWorkplaceCopy;

        /// <summary>
        /// Текущее выбранное рабочее место.
        /// </summary>
        [ObservableProperty]
        private Workplace? _currentWorkplace;

        /// <summary>
        /// Список рабочих мест из базы данных.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Workplace> _workplaces = new();

        public bool HasErrors => CurrentWorkplace?.HasErrors ?? false;

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
            if (CurrentWorkplace == null) return;

            using (var db = new ApplicationDbContext())
            {
                var existingWorkplace = db.Workplaces.FirstOrDefault(w => w.WorkplaceId == CurrentWorkplace.WorkplaceId);
                if (existingWorkplace != null)
                {
                    existingWorkplace.Status = CurrentWorkplace.Status;
                    existingWorkplace.Tariff = CurrentWorkplace.Tariff;
                    existingWorkplace.PriceOfWorkplace = CurrentWorkplace.PriceOfWorkplace;
                    existingWorkplace.Configuration = CurrentWorkplace.Configuration;
                }
                else
                {
                    db.Workplaces.Add(CurrentWorkplace);
                }
                db.SaveChanges();
                LoadWorkplaces();
            }
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
            Workplace newWorkplace = new Workplace();
            CurrentWorkplace = newWorkplace;
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
            {
                oldValue.ErrorsChanged -= OnErrorsChanged;
            }
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