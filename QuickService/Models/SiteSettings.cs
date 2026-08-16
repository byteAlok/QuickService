namespace QuickService.Models
{
    public class SiteSettings
    {
        public string SiteName { get; set; } = string.Empty;
        public string SiteUrl { get; set; } = string.Empty;
        public string Facebook { get; set; } = string.Empty;
        public string Instagram { get; set; } = string.Empty;
        public string YouTube { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string EmailMessage { get; set; } = string.Empty; 
        public string EmailAppPassword { get; set; } = string.Empty;

        public List<string> SameAs => new List<string>
        {
            Facebook,
            Instagram,
            YouTube
        }.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
    }
}