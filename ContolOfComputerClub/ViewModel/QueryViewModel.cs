using System.Data;
using System.Data.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ControlOfComputerClub.Model;
using ControlOfComputerClub.ViewModel.Messages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ControlOfComputerClub.ViewModel;

public partial class QueryViewModel : ObservableObject
{
    [ObservableProperty] 
    private string? _searchName;

    [ObservableProperty]
    private DataView? _queryResults;

    [ObservableProperty] 
    private bool _isEmployeesSelected = true;

    [ObservableProperty]
    private bool _isWorkplacesSelected;

    [ObservableProperty] 
    private bool _isBookingsSelected;

    [ObservableProperty] 
    private bool _isBasic = true;

    [ObservableProperty] 
    private bool _isWhere;

    [ObservableProperty] 
    private bool _isJoin;

    [ObservableProperty] 
    private bool _isAggregate;

    [ObservableProperty]
    private bool _isFull;

    public QueryViewModel()
    {
        Refresh();
    }

    partial void OnIsEmployeesSelectedChanged(bool value) => Refresh();
    partial void OnIsWorkplacesSelectedChanged(bool value) => Refresh();
    partial void OnIsBookingsSelectedChanged(bool value) => Refresh();

    partial void OnIsBasicChanged(bool value) => Refresh();
    partial void OnIsWhereChanged(bool value) => Refresh();
    partial void OnIsJoinChanged(bool value) => Refresh();
    partial void OnIsAggregateChanged(bool value) => Refresh();
    partial void OnIsFullChanged(bool value) => Refresh();

    private void Refresh()
    {
        string sqlQuery = GetSqlQuery();
        QueryResults = Load(sqlQuery);
    }

    private string GetSqlQuery()
    {
        if (IsEmployeesSelected) return GetEmployeesSqlQuery();
        if (IsWorkplacesSelected) return GetWorkplacesSqlQuery();
        return GetBookingsRequestsSqlQuery();
    }

    private string GetEmployeesSqlQuery()
    {
        // Корелированный.
        if (IsBasic)
            return "SELECT * FROM Employees WHERE JobTitle = 'Администратор' " +
                "AND EmployeeId " +
                "IN (SELECT DISTINCT EmployeeId FROM BookingRequests)";

        if (IsWhere)
            return "SELECT * FROM Employees WHERE JobTitle = 'Администратор'";

        if (IsJoin)
            return "SELECT e.Name AS [ФИО], br.BookingRequestId AS [Заявка] " +
                   "FROM Employees e " +
                   "JOIN BookingRequests br ON br.EmployeeId = e.EmployeeId";

        if (IsAggregate)
            return "SELECT e.EmployeeId AS [ID], COUNT(*) AS [Заявок] " +
                   "FROM Employees e " +
                   "JOIN BookingRequests br ON br.EmployeeId = e.EmployeeId " +
                   "GROUP BY e.EmployeeId";

        return "SELECT e.EmployeeId AS [ID], e.Name AS [ФИО], COUNT(br.BookingRequestId) AS [Заявок] " +
               "FROM Employees e " +
               "LEFT JOIN BookingRequests br ON br.EmployeeId = e.EmployeeId " +
               "WHERE e.JobTitle = 'Администратор' " +
               "GROUP BY e.EmployeeId, e.Name " +
               "HAVING COUNT(br.BookingRequestId) >= 2 " +
               "ORDER BY [Заявок] DESC";
    }

    private string GetWorkplacesSqlQuery()
    {
        /// Корелированный запрос.
        if (IsBasic)
            return 
                "SELECT * FROM Workplaces " + 
                "WHERE WorkplaceId IN " +
                "(SELECT DISTINCT WorkplaceId FROM BookingRequests)";

        if (IsWhere)
            return "SELECT * FROM Workplaces WHERE Tariff > 250";

        if (IsJoin)
            return "SELECT w.Configuration, COUNT(br.BookingRequestId) AS [Заявок] " +
                   "FROM Workplaces w " +
                   "JOIN BookingRequests br ON br.WorkplaceId = w.WorkplaceId " +
                   "GROUP BY w.Configuration";

        if (IsAggregate)
            return "SELECT COUNT (*) AS [Всего мест], " +
                   "SUM(CAST(PriceOfWorkplace AS decimal(18,2))) AS [Инвестиции], " +
                   "AVG(CAST(Tariff AS decimal(18,2))) AS [Средний тариф], " +
                   "MIN(CAST(Tariff AS decimal(18,2))) AS [Мин тариф], " +
                   "MAX(CAST(Tariff AS decimal(18,2))) AS [Макс тариф] " +
                   "FROM Workplaces";

        return "SELECT WorkplaceId AS [ID], " +
               "Status AS [Статус], " +
               "Tariff AS [Тариф], " +
               "PriceOfWorkplace AS [Цена], " +
               "Configuration AS [Конфигурация], " +
               "ROUND(CAST(PriceOfWorkplace AS float) / Tariff, 2) AS [Окупаемость (часов)] " +
               "FROM Workplaces " +
               "WHERE Tariff > 200 " +
               "ORDER BY [Окупаемость (часов)] DESC";
    }

    private string GetBookingsRequestsSqlQuery()
    {
        /// Некорелированный запрос.
        if (IsBasic)
            return "SELECT TOP 20 * FROM BookingRequests " +
                   "WHERE WorkplaceId IN (SELECT WorkplaceId FROM Workplaces WHERE Tariff > (SELECT AVG(Tariff) FROM Workplaces))";

        if (IsWhere)
            return "SELECT * FROM BookingRequests WHERE RequestStatus = 'Подтверждено'";

        if (IsJoin)
            return "SELECT br.BookingRequestId AS [ID], c.Name AS [Клиент], w.Configuration AS [Место], " +
                   "br.StartTime AS [Начало], br.EndTime AS [Окончание] " +
                   "FROM BookingRequests br " +
                   "JOIN Clients c ON c.ClientId = br.ClientId " +
                   "JOIN Workplaces w ON w.WorkplaceId = br.WorkplaceId";

        if (IsAggregate)
            return "SELECT RequestStatus AS [Статус], COUNT(*) AS [Кол-во], " +
                   "SUM(CAST(w.Tariff AS decimal(18,2))) AS [Сумма тарифов], " +
                   "AVG(CAST(w.Tariff AS decimal(18,2))) AS [Средний тариф] " +
                   "FROM BookingRequests br " +
                   "JOIN Workplaces w ON w.WorkplaceId = br.WorkplaceId " +
                   "GROUP BY RequestStatus";

        return "SELECT c.Name AS [Клиент], COUNT(br.BookingRequestId) AS [Заявок], " +
               "SUM(DATEDIFF(minute, br.StartTime, br.EndTime))/60.0 AS [Часы], " +
               "SUM(DATEDIFF(minute, br.StartTime, br.EndTime)/60.0 * w.Tariff) AS [Выручка] " +
               "FROM BookingRequests br " +
               "JOIN Clients c ON c.ClientId = br.ClientId " +
               "JOIN Workplaces w ON w.WorkplaceId = br.WorkplaceId " +
               "WHERE br.RequestStatus = 'Подтверждено' " +
               "GROUP BY c.Name " +
               "HAVING SUM(DATEDIFF(minute, br.StartTime, br.EndTime)/60.0 * w.Tariff) > 300 " +
               "ORDER BY [Выручка] DESC";
    }

    [RelayCommand]
    private void SearchClient()
    {
        if (string.IsNullOrWhiteSpace(SearchName))
        {
            WeakReferenceMessenger.Default.Send(
                new ErrorMessage("Введите имя клиента для поиска.", "Пустой ввод"));
            return;
        }
        const string sql =
            "SELECT ClientId AS [ID], Name AS [Клиент] " +
            "FROM Clients WHERE Name LIKE @name + '%'";

        SqlParameter param = new SqlParameter("@name", SqlDbType.NVarChar) { Value = SearchName };
        QueryResults = LoadWithParams(sql, param);
    }

    private static DataView Load(string sql)
    {
        try
        {
            using var db = new ApplicationDbContext();
            using var conn = db.Database.GetDbConnection();
            using var cmd = (SqlCommand)conn.CreateCommand();
            cmd.CommandText = sql;
            if (conn.State != ConnectionState.Open) conn.Open();
            using var reader = cmd.ExecuteReader();
            DataTable? table = new DataTable();
            table.Load(reader);
            return table.DefaultView;
        }
        catch (DbException ex)
        {
            WeakReferenceMessenger.Default.Send(new ErrorMessage($"Ошибка SQL:\n{ex.Message}", "SQL"));
            return new DataTable().DefaultView;
        }
    }

    private static DataView LoadWithParams(string sql, params SqlParameter[] parameters)
    {
        try
        {
            using var db = new ApplicationDbContext();
            using var conn = db.Database.GetDbConnection();
            using var cmd = (SqlCommand)conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(parameters);
            if (conn.State != ConnectionState.Open) conn.Open();
            using var reader = cmd.ExecuteReader();
            var table = new DataTable();
            table.Load(reader);
            return table.DefaultView;
        }
        catch (DbException ex)
        {
            WeakReferenceMessenger.Default.Send(new ErrorMessage($"Ошибка SQL:\n{ex.Message}", "SQL"));
            return new DataTable().DefaultView;
        }
    }
}