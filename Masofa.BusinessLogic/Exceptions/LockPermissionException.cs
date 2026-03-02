namespace Masofa.BusinessLogic.Exceptions
{
    public class LockPermissionException : Exception
    {
        public LockPermissionException()
        {
        }

        public LockPermissionException(string? message) : base(message)
        {
        }
    }
}
