using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlOfComputerClub.Model
{
    public partial class Client : ObservableValidator
    {
        public int ClientId { get; set; }

        private string _phoneNumber = string.Empty;

        private string _name = string.Empty;

        private decimal _amountSpent;

        private decimal _discount;

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

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal Discount
        {
            get => _discount;
            set => SetProperty(ref _discount, value, false);
        }

        public void Validate()
        {
            ValidateAllProperties();
        }
    }
}