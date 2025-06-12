using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ControlOfComputerClub.Model;
using ControlOfComputerClub.ViewModel.Messages;

namespace ControlOfComputerClub.ViewModel
{
    public partial class EmployeesViewModel : ObservableObject
    {
        private Employee? _originalEmployeeCopy;

        /// <summary>
        /// Текущий выбранный сотрудник.
        /// </summary>
        [ObservableProperty]
        private Employee? _currentEmployee;

        /// <summary>
        /// Список сотрудников, загруженных из базы данных.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Employee> _employees = new();

        public bool HasErrors => CurrentEmployee?.HasErrors ?? false;

        public EmployeesViewModel()
        {
            LoadEmployees();
            WeakReferenceMessenger.Default.Register<FileSelectedMessage>(this, (_, message) =>
            {
                if (CurrentEmployee != null)
                {
                    CurrentEmployee.Photo = message.Value;
                }
            });

        }

        [RelayCommand]
        private void LoadEmployees()
        {
            using (var db = new ApplicationDbContext())
            {
                Employees = new ObservableCollection<Employee>(db.Employees.ToList());
            }
            if (Employees.Count > 0)
                CurrentEmployee = Employees[0];
        }

        [RelayCommand]
        private void SaveEmployee()
        {
            if (CurrentEmployee == null) return;

            using (var db = new ApplicationDbContext())
            {
                var existingEmployee = db.Employees.FirstOrDefault(e => e.EmployeeId == CurrentEmployee.EmployeeId);
                if (existingEmployee != null)
                {
                    existingEmployee.Name = CurrentEmployee.Name;
                    existingEmployee.PhoneNumber = CurrentEmployee.PhoneNumber;
                    existingEmployee.JobTitle = CurrentEmployee.JobTitle;
                    existingEmployee.NumberPassport = CurrentEmployee.NumberPassport;
                    existingEmployee.Photo = CurrentEmployee.Photo;
                }
                else
                {
                    db.Employees.Add(CurrentEmployee);
                }
                db.SaveChanges();
                LoadEmployees();
            }
        }

        [RelayCommand]
        private void NextEmployee()
        {
            if (CurrentEmployee == null || Employees.Count == 0) return;
            CancelChanges();
            int index = Employees.IndexOf(CurrentEmployee);
            if (index < Employees.Count - 1)
                CurrentEmployee = Employees[index + 1];
        }

        [RelayCommand]
        private void PreviousEmployee()
        {
            if (CurrentEmployee == null || Employees.Count == 0) return;
            CancelChanges();
            int index = Employees.IndexOf(CurrentEmployee);
            if (index > 0)
                CurrentEmployee = Employees[index - 1];
        }

        [RelayCommand]
        private void FirstEmployee()
        {
            CancelChanges();
            if (Employees.Count > 0)
                CurrentEmployee = Employees[0];
        }

        [RelayCommand]
        private void LastEmployee()
        {
            CancelChanges();
            if (Employees.Count > 0)
                CurrentEmployee = Employees[Employees.Count - 1];
        }

        [RelayCommand]
        private void AddEmployee()
        {
            Employee newEmployee = new Employee();
            CurrentEmployee = newEmployee;
            CurrentEmployee.Validate();
        }

        [RelayCommand]
        private void DeleteEmployee()
        {
            if (CurrentEmployee == null) return;
            using (var db = new ApplicationDbContext())
            {
                var employeeToDelete = db.Employees.FirstOrDefault(e => e.EmployeeId == CurrentEmployee.EmployeeId);
                if (employeeToDelete != null)
                {
                    db.Employees.Remove(employeeToDelete);
                    db.SaveChanges();
                    Employees.Remove(CurrentEmployee);
                    CurrentEmployee = Employees.Count > 0 ? Employees[0] : null;
                }
            }
        }

        [RelayCommand]
        private void LoadEmployeePhoto()
        {
            WeakReferenceMessenger.Default.Send(new OpenFileDialogMessage());
        }


        partial void OnCurrentEmployeeChanged(Employee? oldValue, Employee? newValue)
        {
            if (oldValue != null)
            {
                oldValue.ErrorsChanged -= OnErrorsChanged;
            }
            if (newValue != null)
            {
                _originalEmployeeCopy = new Employee
                {
                    EmployeeId = newValue.EmployeeId,
                    Name = newValue.Name,
                    PhoneNumber = newValue.PhoneNumber,
                    JobTitle = newValue.JobTitle,
                    NumberPassport = newValue.NumberPassport,
                    Photo = newValue.Photo
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
            if (_originalEmployeeCopy != null && CurrentEmployee != null)
            {
                CurrentEmployee.Name = _originalEmployeeCopy.Name;
                CurrentEmployee.PhoneNumber = _originalEmployeeCopy.PhoneNumber;
                CurrentEmployee.JobTitle = _originalEmployeeCopy.JobTitle;
                CurrentEmployee.NumberPassport = _originalEmployeeCopy.NumberPassport;
                CurrentEmployee.Photo = _originalEmployeeCopy.Photo;
            }
        }
    }
}