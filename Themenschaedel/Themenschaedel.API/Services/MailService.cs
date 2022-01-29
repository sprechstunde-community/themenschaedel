using System.Text;

namespace Themenschaedel.API.Services
{
    public class MailService : IMailService
    {
        private readonly ILogger<MailService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private readonly string AuthToken;
        private readonly string BaseURL;
        private readonly string MailjetAPIUrl;

        public MailService(ILogger<MailService> logger, HttpClient httpClient, IConfiguration config)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = config;

            BaseURL = _configuration["BaseURL"];
            MailjetAPIUrl = _configuration["MailjetURL"];

            // Get mailjet credentials from secrets.json (VS feature) and set them as default headers
            AuthToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(_configuration["MailjetCredentials"]));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", AuthToken);
        }

        public async Task SendMail(string mailTo, string verificationId)
        {
            string mailJson = "{\"Messages\":[{\"From\":{\"Email\":\"mailjet@yiuo.me\",\"Name\":\"Themenschaedel Projekt\"},\"To\":[{\"Email\":\"" + mailTo + "\",\"Name\":\"User\"}],\"Subject\":\"Themenschaedel Verification\",\"TextPart\":\"Themenschaedel Verification\",\"HTMLPart\":\"<p>Hi, here is the verification email for the Themenschaedel Project.<br/><br/><br/>Please click on this link to verify your email:<br/><a href=\'" + BaseURL + "api/auth/verify/" + verificationId +"\'>Verification link</a></p>\",\"CustomID\":\"VerificationEmail\"}]}";
            var response = await _httpClient.PostAsync(MailjetAPIUrl, new StringContent(mailJson));
        }
    }
}
