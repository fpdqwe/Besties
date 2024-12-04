using Bot.Exceptions;
using Domain.Entities;
using Telegram.Bot.Types;

namespace Bot.Types
{
    public class SearchScopes
    {
        private List<long> _archivedRelatedIds;
        private List<Offer> _incomingOffers;
		private List<Offer> _checkedOffers;
		private Offer _currentOffer;
        public int IncomingOffersCount { get => _incomingOffers.Where(x => x.RecipientApproval == null).Count(); }
		public List<long> SkippedIds
        {
            get
            {
                var range = _checkedOffers.OrderByDescending(x => x.DateCreated)
                    .Select(x => x.RecipientId).ToList();
                range.AddRange(_incomingOffers.OrderByDescending(x => x.DateCreated)
                    .Select(x => x.SenderId).ToList());
                range.AddRange(_archivedRelatedIds);
                return range;
            }
        }
        public Card CurrentOfferCard { get; private set; }
        public Offer CurrentIncomingOffer { get; private set; }
        public SearchScopes()
        {
            _checkedOffers = new List<Offer>(1);
            _incomingOffers = new List<Offer>(1);
            _archivedRelatedIds = new List<long>(1);
        }
        public void SetIncomingOffers(List<Offer> offers)
        {
			_archivedRelatedIds.AddRange(offers.Where(x => x.SenderApproval == false).Select(x => x.SenderId).ToList());
			_incomingOffers = offers.Where(x => x.SenderApproval == true).ToList();
            if (_incomingOffers != null)
            {
                if (_incomingOffers.Count > 0)
                {
					
					CurrentIncomingOffer = _incomingOffers[0];
                }
            }
        }
        public void SetOutgoingOffers(IList<Offer> offers)
        {
            _checkedOffers = offers.Where(x => x.SenderApproval == true).ToList();
        }
        public List<Offer> GetOutgoingOffers()
        {
            return new List<Offer>(_checkedOffers);
        }
        public List<Offer> GetIncomingOffers()
        {
            return new List<Offer>(_incomingOffers);
        }
		public async void SkipIncomingOffer(long senderId, bool approve)
        {
            var offer = _incomingOffers.FirstOrDefault(x => x.SenderId == senderId);
            if (offer == null) return;
            offer.RecipientApproval = approve;
            offer.RecipientApprovalDate = DateTime.UtcNow;
            await BotService.ChatManager.SaveOrUpdateOffer(offer);
            if (IncomingOffersCount > 0)
            {
                var uncheckedOffers = _incomingOffers.Where(x => x.RecipientApproval == null).ToList();
                
                CurrentIncomingOffer = uncheckedOffers[0];
            }
        }
		public Offer GetCurrentOffer(Chat chat)
        {
            return _currentOffer;
        }

        // Returns new offer card if succeed
        public async Task<Card> SkipCurrentOffer(Chat sender, bool senderApprove)
        {
            if (_currentOffer != null)
            {
				_currentOffer.SenderApproval = senderApprove;
				_checkedOffers.Add(_currentOffer);
                var result = await BotService.ChatManager.SaveOrUpdateOffer(_currentOffer);
                Console.WriteLine(result);
			}
            await GetNewOffer(sender);
            return CurrentOfferCard;
        }
		private async Task<Offer> GetNewOffer(Chat chat)
		{
			var newOffer = await BotService.ChatManager.GetOffer(chat);
            if (newOffer == null)
            {
                CurrentOfferCard = null;
                _currentOffer = null;
                return null;
            }
			var newCurrent = new Offer()
			{
				DateCreated = DateTime.UtcNow,
				SenderId = chat.Id,
				RecipientId = newOffer.Id,
                SenderApproval = false,
			};
            newCurrent = await BotService.ChatManager.SaveOrUpdateOffer(newCurrent);
			_currentOffer = newCurrent;
			CurrentOfferCard = newOffer;
			return _currentOffer;
		}
	}
}
