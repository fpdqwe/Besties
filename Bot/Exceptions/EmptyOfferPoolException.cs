using Bot.Resources;

namespace Bot.Exceptions
{
	public class EmptyOfferPoolException : Exception
	{
		private static readonly string _defaultMessage = strings.EmptyCardPoolError;

        public EmptyOfferPoolException() : base(_defaultMessage)
        {
            
        }
    }
}
