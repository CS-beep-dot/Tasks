namespace TaskManagementApi.Application.Exceptions
{
    public class InvalidReferenceException : Exception
    {
        public InvalidReferenceException(string message) : base(message)
        {
        }
    }
}
