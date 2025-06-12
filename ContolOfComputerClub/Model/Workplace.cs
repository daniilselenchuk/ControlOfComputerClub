using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ControlOfComputerClub.Model
{
    public partial class Workplace : ObservableValidator
    {
        private bool _status;
        private decimal _tariff;
        private decimal? _priceOfWorkplace;
        private string? _configuration;

        public int WorkplaceId { get; set; }

        [Required(ErrorMessage = "Статус обязателен")]
        public bool Status
        {
            get => _status;
            set => SetProperty(ref _status, value, true);
        }

        [Required(ErrorMessage = "Тариф обязателен")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Тариф должен быть больше 0")]
        public decimal Tariff
        {
            get => _tariff;
            set => SetProperty(ref _tariff, value, true);
        }

        [Required(ErrorMessage = "Цена рабочего места обязательна")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Цена рабочего места должна быть больше 0")]
        public decimal? PriceOfWorkplace
        {
            get => _priceOfWorkplace;
            set => SetProperty(ref _priceOfWorkplace, value, true);
        }

        public string? Configuration
        {
            get => _configuration;
            set => SetProperty(ref _configuration, value, true);
        }

        public void Validate() => ValidateAllProperties();

        public override string ToString()
        {
            return $"{WorkplaceId} - {Tariff}";
        }
    }
}