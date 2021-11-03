namespace Themenschaedel.Shared.Props
{
    public enum Platforms
    {
        None,
        Spotify,
        Youtube,
        Apple
    }

    public class Settings
    {
        public Platforms Platform { get; set; }
    }
}