using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControlOfComputerClub.Model
{
    public partial class BookingRequest : ObservableValidator, IValidatableObject
    {
        public int BookingRequestId { get; set; }
        public int EmployeeId { get; set; }
        public int WorkplaceId { get; set; }
        public int ClientId { get; set; }
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