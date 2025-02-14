namespace BLL.Abstractions
{
	public interface IPasswordHasher
	{
		public string Hash(string password);
		public bool Verify(string value, string password);
	}
}
