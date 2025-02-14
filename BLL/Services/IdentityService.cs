using BLL.Abstractions;
using DAL.Interfaces;
using DAL.Repositories;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using BLL.Extensions;
using BLL.Exceptions;

namespace BLL.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
		private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly ILogger _logger;
        public IdentityService(IContextManager contextManager,
            IPasswordHasher passwordHasher,
			IJwtTokenProvider jwtTokenProvider)
        {
            _userRepository = new UserRepository(contextManager);
            _passwordHasher = passwordHasher;
			_jwtTokenProvider = jwtTokenProvider;
        }

        public async Task<User> Register(string username, string password, string contact)
        {
			if (contact.IsEmail()) return await RegisterByEmail(username, password, contact);
			else if (contact.IsPhoneNumber()) return await RegisterByPhone(username, password, contact);
			else throw new BllException();
		}
		public async Task<User> RegisterTelegramClient(long telegramId, string username)
		{
			var newUser = new User
			{
				Name = username,
				TelegramId = telegramId
			};
			var result = await _userRepository.Add(newUser);
			return result;
		}
		private async Task<User> RegisterByPhone(string username, string password, string phone)
		{
			var newUser = new User
			{
				Name = username,
				Password = _passwordHasher.Hash(password),
				PhoneNumber = phone
			};
			var result = await _userRepository.Add(newUser);
			return result;
		}
		private async Task<User> RegisterByEmail(string username, string password, string email)
		{
			var newUser = new User
			{
				Name = username,
				Password = _passwordHasher.Hash(password),
				EmailAddress = email
			};
			var result = await _userRepository.Add(newUser);
			return result;
		}
		public async Task<string> Login(string contact, string password)
		{
			if (contact.IsEmail()) return await AuthUserByEmail(contact, password);
			else if (contact.IsPhoneNumber()) return await AuthUserByPhone(contact, password);
			else throw new InvalidContactInputException();
		}
		private async Task<string> AuthUserByEmail(string email, string password)
		{
			if (email.IsNullOrEmpty()) { throw new ArgumentNullException(nameof(email)); }
			if (password.IsNullOrEmpty()) { throw new ArgumentNullException(nameof(password)); }
			var user = await _userRepository.GetByEmail(email);

			if (user is null) throw new InvalidCredentialsException();

			var isPasswordValid = _passwordHasher.Verify(password, user.Password);
			if (isPasswordValid)
			{
				return _jwtTokenProvider.GenerateToken(user);
			}
			else throw new InvalidCredentialsException();
		}
		private async Task<string> AuthUserByPhone(string phone, string password)
		{
			if (phone.IsNullOrEmpty()) { throw new ArgumentNullException(nameof(phone)); }
			if (password.IsNullOrEmpty()) { throw new ArgumentNullException(nameof(password)); }
			var user = await _userRepository.GetByPhone(phone);

			if (user is null) throw new InvalidCredentialsException();

			var isPasswordValid = _passwordHasher.Verify(password, user.Password);
			if (isPasswordValid)
			{
				return _jwtTokenProvider.GenerateToken(user);
			}
			else throw new InvalidCredentialsException();
		}
	}
}
