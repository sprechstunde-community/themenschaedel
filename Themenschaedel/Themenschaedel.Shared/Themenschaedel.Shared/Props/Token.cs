namespace Themenschaedel.Shared.Props
{
    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }

        public string GetToken() => $"{token_type} {access_token}";
    }
}