namespace Masofa.Common.Models.Identity
{
    public class OneIdUser
    {
        public int? Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Pinfl { get; set; }
        public string? Position { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Subdivision { get; set; }
        public string? Description { get; set; }
        public string? Base { get; set; }
        public int? Organization { get; set; }
        public DateTime? LastLogin { get; set; }
        public List<OneIdSystemInfo> Systems { get; set; } = [];
        public List<OneIdRoleInfo> Roles { get; set; } = [];
    }
}