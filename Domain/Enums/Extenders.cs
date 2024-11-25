namespace Domain.Enums
{
	public static class Extenders
	{
		public static string ToString(this string text, Gender gender)
		{
			return gender switch
			{
				Gender.Male => "Мужской",
				Gender.Female => "Женский",
				Gender.NotSpecified => "Не указан",
				_ => "Не указан",
			};
		}
	}
}
