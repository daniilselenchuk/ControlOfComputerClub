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
                "SELECT e.EmployeeId AS [ID]," +
                " e.Name AS [ФИО]," +
                " e.JobTitle AS [Должность]," +
                " e.PhoneNumber AS [Телефон]," +
                " e.NumberPassport AS [Паспорт]," +
                " COUNT(br.BookingRequestId) AS [Заявок]" +
                " FROM Employees e" +
                " JOIN BookingRequests br " +
                "ON br.EmployeeId = e.EmployeeId" +
                " GROUP BY e.EmployeeId, e.Name, e.JobTitle, e.PhoneNumber, e.NumberPassport";
            QueryResults = LoadFromSql(sql);
        }

        private void LoadWorkplaces()
        {
            const string sql = 
                "SELECT WorkplaceId AS [ID]," +
                " Status AS [Статус]," +
                " Tariff AS [Тариф]," +
                " PriceOfWorkplace AS [Цена]," +
                " Configuration AS [Конфигурация], " +
                "ROUND(CAST(PriceOfWorkplace AS float) / CAST(Tariff AS float), 2) AS [Окупаемость (часов)]"+
                "FROM Workplaces";
            QueryResults = LoadFromSql(sql);
        }

        private void LoadBookings()
        {
            const string sql = 
                " SELECT BookingRequestId AS [ID]," +
                " EmployeeId AS [Сотрудник]," +
                " WorkplaceId As [Рабочее место]," +
                " ClientId AS [Клиент]," +
                " StartTime AS [Начало]," +
                " EndTime AS [Окончание]," +
                " RequestStatus AS [Статус] " +
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