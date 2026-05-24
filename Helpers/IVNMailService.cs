namespace EcommerceMVC.Helpers
{
    public interface IVNMailService
    {
        Task<int> SendMail(string toMail, string subject, string message);
    }
}
