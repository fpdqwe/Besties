using Domain.Entities;
using Domain.Enums;

namespace BLL.Abstractions
{
    public interface IJwtTokenProvider
    {
        public string GenerateToken(User user);
        public string GenerateToken();
    }
}
