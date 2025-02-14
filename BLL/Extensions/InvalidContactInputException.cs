namespace BLL.Extensions
{
    [Serializable]
    internal class InvalidContactInputException : Exception
    {
        public InvalidContactInputException()
        {
        }

        public InvalidContactInputException(string? message) : base(message)
        {
        }

        public InvalidContactInputException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}