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
using Microsoft.Data.SqlClient;
using System.Data;

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
                var employeesList = db.Employees
                    .FromSqlRaw(@"SELECT EmployeeId, Name, PhoneNumber, JobTitle, NumberPassport, Photo FROM Employees")
                    .ToList();
                Employees = new ObservableCollection<Employee>(employeesList);
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
                var existingEmployee = db.Employees
                    .FromSqlRaw(@"SELECT EmployeeId, Name, PhoneNumber, JobTitle, NumberPassport, Photo 
                                  FROM Employees 
                                  WHERE EmployeeId = {0}", CurrentEmployee.EmployeeId)
                    .FirstOrDefault();

                var photoParam = new SqlParameter("@Photo", SqlDbType.VarBinary, -1)
                {
                    Value = CurrentEmployee.Photo ?? (object)DBNull.Value
                };

                if (existingEmployee != null)
                {
                    string updateQuery = 
                        @"UPDATE Employees 
                        SET Name = @Name, PhoneNumber = @PhoneNumber, JobTitle = @JobTitle,
                        NumberPassport = @NumberPassport, Photo = @Photo 
                        WHERE EmployeeId = @EmployeeId";

                    var parameters = new[]
                    {
                        new SqlParameter("@Name", CurrentEmployee.Name ?? string.Empty),
                        new SqlParameter("@PhoneNumber", CurrentEmployee.PhoneNumber ?? string.Empty),
                        new SqlParameter("@JobTitle", CurrentEmployee.JobTitle ?? string.Empty),
                        new SqlParameter("@NumberPassport", CurrentEmployee.NumberPassport ?? string.Empty),
                        photoParam,
                        new SqlParameter("@EmployeeId", CurrentEmployee.EmployeeId)
                    };

                    db.Database.ExecuteSqlRaw(updateQuery, parameters);
                }
                else
                {
                    string insertQuery = @"
                        INSERT INTO Employees 
                        (Name, PhoneNumber, JobTitle, NumberPassport, Photo) 
                        VALUES 
                        (@Name, @PhoneNumber, @JobTitle, @NumberPassport, @Photo)";

                    var parameters = new[]
                    {
                        new SqlParameter("@Name", CurrentEmployee.Name ?? string.Empty),
                        new SqlParameter("@PhoneNumber", CurrentEmployee.PhoneNumber ?? string.Empty),
                        new SqlParameter("@JobTitle", CurrentEmployee.JobTitle ?? string.Empty),
                        new SqlParameter("@NumberPassport", CurrentEmployee.NumberPassport ?? string.Empty),
                        photoParam
                    };

                    db.Database.ExecuteSqlRaw(insertQuery, parameters);
                }
            }
            LoadEmployees();
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
                string deleteQuery = "DELETE FROM Employees WHERE EmployeeId = @EmployeeId";
                var parameter = new SqlParameter("@EmployeeId", CurrentEmployee.EmployeeId);
                int rowsAffected = db.Database.ExecuteSqlRaw(deleteQuery, parameter);
                if (rowsAffected > 0)
                {
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