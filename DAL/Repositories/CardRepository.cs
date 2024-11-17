using DAL.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
	public class CardRepository : BaseRepository<Card>
	{
		public CardRepository(IContextManager contextManager) : base(contextManager) { }

		public async Task<List<Card>> GetCardsForSearch(Card card)
		{
			using(var context = CreateDatabaseContext())
			{
				if (card.TargetGender == Gender.NotSpecified)
				{
					var result = await context.Cards.Where(x => x.Region == card.Region && x.Id != card.Id).ToListAsync();
					return result;
				}
				else
				{
					var result = await context.Cards.Where(x => (x.Gender != Gender.NotSpecified && x.Gender != card.Gender) && 
						x.Region == card.Region && x.Id != card.Id)
						.ToListAsync();
					return result;
				}
			}
		}
	}
}
