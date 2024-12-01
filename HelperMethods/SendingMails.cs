namespace Expense_App.HelperMethods
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email,string subject,string message );
    }
}
