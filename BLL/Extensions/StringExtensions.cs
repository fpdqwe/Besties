using System.Text.RegularExpressions;

namespace BLL.Extensions
{
	public static class StringExtensions
	{
		public static bool IsEmail(this string value)
		{
			string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
			return Regex.IsMatch(value, emailPattern);
		}

		public static bool IsPhoneNumber(this string value)
		{
			string phonePattern = @"^\+?\d{10,15}$";
			return Regex.IsMatch(value, phonePattern);
		}
		public static bool IsNullOrEmpty(this string value)
		{
			return value == null || value.Length == 0;
		}
	}
}
