using Domain.Entities;

namespace Bot.Exceptions
{
	public class InvalidOfferException : Exception
	{
        public long OfferId { get; private set; }
        public InvalidOfferException(string message) : base(message) { }

        public InvalidOfferException(string message, Exception innerEx) : base(message, innerEx) { }
        public InvalidOfferException(string message, Exception innerEx, long offerId) : base(message, innerEx)
        {
            OfferId = offerId;
        }
    }
}
