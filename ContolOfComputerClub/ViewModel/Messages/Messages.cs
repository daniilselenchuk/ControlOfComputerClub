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

    /// <summary>
    /// Сообщения об ошибке.
    /// </summary>
    public class ErrorMessage
    {
        public string Title { get; }
        public string Message { get; }

        public ErrorMessage(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }

    /// <summary>
    /// Сообщение для открытия окна для добавления клиента.
    /// </summary>
    public class AddClientMessage { }

    /// <summary>
    /// Сообщение для закрытия окна добавления клиента.
    /// </summary>
    public class CloseAddClientWindowMessage { }

    /// <summary>
    /// Сообщение для открытия окна для добавления сотрудника.
    /// </summary>
    public class AddWorkplaceMessage { }

    /// <summary>
    /// Сообщение для закрытия окна добавления рабочего места.
    /// </summary>
    public class CloseAddWorkplaceWindowMessage { }

    /// <summary>
    /// Сообщение для открытия окна для добавления заявки на бронирование.
    /// </summary>
    public class AddBookingRequestMessage { }

    /// <summary>
    /// Сообщение для закрытия окна добавления заявки на бронирование.
    /// </summary>
    public class CloseAddBookingRequestWindowMessage { }

    /// <summary>
    /// Сообщение для открытия диалога выбора.
    /// </summary>
    public class OpenSelectionDialogMessage
    {
        public string SelectionType { get; }

        public OpenSelectionDialogMessage(string selectionType)
        {
            SelectionType = selectionType;
        }
    }

    /// <summary>
    /// Сообщение для закрытия диалога выбора.
    /// </summary>
    public class CloseSelectionDialogMessage { }

    /// <summary>
    /// Сообщение, которое отправляется при выборе элемента в диалоге выбора.
    /// </summary>
    public class SelectionChosenMessage
    {
        public object SelectedItem { get; }

        public SelectionChosenMessage(object selectedItem)
        {
            SelectedItem = selectedItem;
        }
    }

}
