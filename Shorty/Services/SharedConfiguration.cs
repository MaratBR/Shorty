namespace Shorty.Services
{
    public class SharedConfiguration
    {
        public int PreferredLinkLength { get; set; } = 7;

        public string SecretKey { get; set; } = string.Empty;
    }
}