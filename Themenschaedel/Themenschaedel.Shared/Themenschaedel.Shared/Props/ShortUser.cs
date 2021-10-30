namespace Themenschaedel.Shared.Props
{
    public class ShortUserWrapper
    {
        public ShortUser data { get; set; }
    }
    
    public class ShortUser
    {
        public string username { get; set; }
        public string description { get; set; }
        public string created_at { get; set; }
        public string profile_picture { get; set; }
    }
}