using System;
using System.Threading.Tasks;

namespace Nerdshoe.Views
{
    public interface IView
    {
        string Title { get; set; }

        Task ShowErrorMessage(string message, string caption = null);

        Task ShowAlertMessage(string message, string caption = null);

        Task<bool> ShowConfirmPrompt(string message, string caption = null);
    }
}
