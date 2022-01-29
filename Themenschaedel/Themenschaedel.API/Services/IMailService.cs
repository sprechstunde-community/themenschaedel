namespace Themenschaedel.API.Services
{
    public interface IMailService
    {
        public Task SendMail(string mailTo, string verificationId);
    }
}
