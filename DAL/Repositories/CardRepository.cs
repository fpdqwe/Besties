using DAL.Interfaces;
using Domain.Entities;
using Domain.Enums;
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
					var result = await context.Cards
						.Where(x => x.IsActive == true)
						.OrderBy(x => x.LastActivationDate)
						.Where(x => x.Region == card.Region && x.Id != card.Id)
						.Where(x => x.TargetGender == Gender.NotSpecified || x.TargetGender == card.Gender)
						.ToListAsync();
					return result;
				}
				else if (card.TargetGender == Gender.Male)
				{
					var result = await context.Cards
						.Where(x => (x.IsActive == true))
						.OrderBy(x => x.LastActivationDate)
						.Where(x => x.Gender == Gender.Male)
						.Where(x => x.TargetGender == Gender.NotSpecified || x.TargetGender == card.Gender)
						.Where(x => x.Region == card.Region && x.Id != card.Id)
						.ToListAsync();
					return result;
				}
				else // (card.TargetGender == Gender.Female)
				{
					var result = await context.Cards
						.Where(x => (x.IsActive == true))
						.OrderBy(x => x.LastActivationDate)
						.Where(x => x.Gender == Gender.Female)
						.Where(x => x.TargetGender == Gender.NotSpecified || x.TargetGender == card.Gender)
						.Where(x => x.Region == card.Region && x.Id != card.Id)
						.ToListAsync();
					return result;
				}
			}
		}
	}
}
