namespace ADManager.Models
{
    public class User
    {
        public string? SAMAccountName { get; set; }
        public string? GivenName { get; set; }
        public string? Surname { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? Office { get; set; }
        public string? TelephoneNumber { get; set; }
        public string? Email { get; set; }
        public string? WebPage { get; set; }
        public string? DistinguishedName { get; set; }
        public string? Initials { get; set; }
        public string? Title { get; set; } // Поле "Должность"
    }
}