namespace BLL.Exceptions
{
	public class BllException : Exception
	{
        public BllException() : base() { }
		public BllException(string message) : base(message) { }
    }
}
