using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ControlOfComputerClub.Model
{
    public partial class Client : ObservableValidator
    {
        public int ClientId { get; set; }

        private string _phoneNumber = string.Empty;

        private string _name = string.Empty;

        private decimal _amountSpent;

        [Required(ErrorMessage = "Телефон обязателен")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Номер телефона должен содержать ровно 11 цифр")]
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

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Сумма не может быть отрицательной")]
        public decimal AmountSpent
        {
            get => _amountSpent;
            set => SetProperty(ref _amountSpent, value, true);
        }

        public decimal Discount { get; }

        public void Validate()
        {
            ValidateAllProperties();
        }
    }
}