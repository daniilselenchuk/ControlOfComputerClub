using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ControlOfComputerClub.ViewModel.Messages
{
    /// <summary>
    /// Сообщение для выхода из приложения.
    /// </summary>
    public class ExitMessage { }

    /// <summary>
    /// Сообщение для отображения информации о программе.
    /// </summary>
    public class ShowAboutMessage { }

    /// <summary>
    /// Сообщение для открытия окна клиентов.
    /// </summary>
    public class OpenClientsWindowMessage { }

    /// <summary>
    /// Сообщение для открытия окна сотрудников.
    /// </summary>
    public class OpenEmployeesWindowMessage { }

    /// <summary>
    /// Сообщение для открытия окна рабочих мест.
    /// </summary>
    public class OpenWorkplacesWindowMessage { }

    /// <summary>
    /// Сообщение для открытия окна заявок на бронирование.
    /// </summary>
    public class  OpenBookingRequestsWindowMessage { }

    /// <summary>
    /// Сообщение для открытия окна для добавления фото.
    /// </summary>
    public class OpenFileDialogMessage { }

    /// <summary>
    /// Сообщение, которое отправляется при выборе файла.
    /// </summary>
    public class FileSelectedMessage : ValueChangedMessage<byte[]>
    {
        public FileSelectedMessage(byte[] imageData) : base(imageData) { }
    }
}
