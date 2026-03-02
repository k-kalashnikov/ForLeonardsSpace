namespace Masofa.Common.Models.SystemCrical
{
    public class CallStack
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public Guid? CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public string CreateUserFullName { get; set; }
    }
}
