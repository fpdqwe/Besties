using DAL.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
	public class OfferRepository : BaseRepository<Offer>
	{
		public OfferRepository(IContextManager contextManager) : base(contextManager)
		{
			
		}

		public async Task<List<Offer>> GetOffersByRecepientId(long id)
		{
			using (var context = CreateDatabaseContext())
			{
				return await context.Offers.Where(x => x.RecipientId == id && x.RecipientApprovalDate != null)
					.ToListAsync();
			}
		}

		public async Task<List<Offer>> GetOffersBySenderId(long id)
		{
			using (var context = CreateDatabaseContext())
			{
				return await context.Offers.Where(x => x.SenderId == id)
					.ToListAsync();
			}
		}
	}
}
