using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ControlOfComputerClub.Model
{
    public partial class Employee : ObservableValidator
    {
        public int EmployeeId { get; set; }

        private string _phoneNumber = string.Empty;
        private string _name = string.Empty;
        private string _jobTitle = string.Empty;
        private string _numberPassport = string.Empty;
        private byte[]? _photo;

        [Required(ErrorMessage = "Телефон обязателен")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Телефон должен содержать ровно 11 цифр")]
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value, true);
        }

        [Required(ErrorMessage = "Имя обязательно")]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, true);
        }

        [Required(ErrorMessage = "Должность обязательна")]
        [RegularExpression(@"^(Администратор|Кассир)$", ErrorMessage = "Должность должна быть либо 'Администратор', либо 'Кассир'")]
        public string JobTitle
        {
            get => _jobTitle;
            set => SetProperty(ref _jobTitle, value, true);
        }

        [Required(ErrorMessage = "Номер паспорта обязателен")]
        public string NumberPassport
        {
            get => _numberPassport;
            set => SetProperty(ref _numberPassport, value, true);
        }

        public byte[]? Photo
        {
            get => _photo;
            set => SetProperty(ref _photo, value, true);
        }

        public void Validate() => ValidateAllProperties();
    }
}