using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlOfComputerClub.Model
{
    public partial class BookingRequest : ObservableValidator, IValidatableObject
    {
        public int BookingRequestId { get; set; }

        [ObservableProperty]
        private int _employeeId;

        [ObservableProperty]
        public int _workplaceId;

        [ObservableProperty]
        public int _clientId;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        private string _requestStatus = string.Empty;

        [Required(ErrorMessage = "Статус заявки обязателен")]
        [RegularExpression(@"^(Создана|Завершена|Отменена)$",
            ErrorMessage = "Статус заявки должен быть 'Создана', 'Завершена' или 'Отменена'")]
        public string RequestStatus
        {
            get => _requestStatus;
            set => SetProperty(ref _requestStatus, value, true);
        }


        [NotMapped]
        public decimal SessionPrice
        {
            get
            {
                using var db = new ApplicationDbContext();

                decimal tariff = db.Workplaces
                    .Where(w => w.WorkplaceId == WorkplaceId)
                    .Select(w => w.Tariff)
                    .FirstOrDefault();

                decimal discountRaw = db.Clients
                    .Where(c => c.ClientId == ClientId)
                    .Select(c => c.Discount)
                    .FirstOrDefault();

                decimal discountMultiplier = 1 - (discountRaw / 100m);
                decimal durationHours = (decimal)(EndTime - StartTime).TotalHours;
                return Math.Round(durationHours * tariff * discountMultiplier, 2);
            }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartTime >= EndTime)
            {
                yield return new ValidationResult("Время начала должно быть меньше времени окончания",
                                                   new[] { nameof(StartTime), nameof(EndTime) });
            }
        }
    }
}