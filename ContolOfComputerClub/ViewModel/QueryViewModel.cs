using System.Data;
using System.Data.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using ControlOfComputerClub.Model;
using ControlOfComputerClub.ViewModel.Messages;
using Microsoft.EntityFrameworkCore;

namespace ControlOfComputerClub.ViewModel
{
    public partial class QueryViewModel : ObservableObject
    {
        [ObservableProperty] private DataView? queryResults;

        [ObservableProperty] private bool isEmployeesSelected;
        [ObservableProperty] private bool isWorkplacesSelected;
        [ObservableProperty] private bool isBookingsSelected;

        public QueryViewModel()
        {
            IsEmployeesSelected = true;
        }

        partial void OnIsEmployeesSelectedChanged(bool value)
        {
            if (value) LoadEmployees();
        }

        partial void OnIsWorkplacesSelectedChanged(bool value)
        {
            if (value) LoadWorkplaces();
        }

        partial void OnIsBookingsSelectedChanged(bool value)
        {
            if (value) LoadBookings();
        }

        private void LoadEmployees()
        {
            const string sql = 
                "SELECT EmployeeId AS [Код]," +
                " Name AS [ФИО]," +
                " JobTitle AS [Должность]," +
                " PhoneNumber AS [Телефон]," +
                " NumberPassport AS [Паспорт]" +
                " FROM Employees";
            QueryResults = LoadFromSql(sql);
        }

        private void LoadWorkplaces()
        {
            const string sql = 
                "SELECT WorkplaceId AS [Код]," +
                " Status AS [Статус]," +
                " Tariff AS [Тариф]," +
                " PriceOfWorkplace AS [Цена]," +
                " Configuration AS [Конфигурация] " +
                "FROM Workplaces";
            QueryResults = LoadFromSql(sql);
        }

        private void LoadBookings()
        {
            const string sql = 
                "SELECT BookingRequestId AS [Код]," +
                " EmployeeId AS [Сотрудник]," +
                " WorkplaceId AS [Раб. место]," +
                " ClientId AS [Клиент]," +
                " StartTime AS [Начало]," +
                " EndTime AS [Окончание]," +
                " RequestStatus AS [Статус]" +
                " FROM BookingRequests";
            QueryResults = LoadFromSql(sql);
        }

        private DataView LoadFromSql(string sql)
        {
            try
            {
                using var db = new ApplicationDbContext();
                using var conn = db.Database.GetDbConnection();
                using var cmd = conn.CreateCommand();

                cmd.CommandText = sql;
                if (conn.State != ConnectionState.Open) conn.Open();

                using var reader = cmd.ExecuteReader();
                var table = new DataTable();
                table.Load(reader);

                return table.DefaultView;
            }
            catch (DbException ex)
            {
                WeakReferenceMessenger.Default.Send(new ErrorMessage($"Ошибка SQL-запроса:\n{ex.Message}", "Ошибка SQL"));
                return new DataTable().DefaultView;
            }
        }
    }
}