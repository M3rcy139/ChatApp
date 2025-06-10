using ChatApp.Domain.Models;

namespace ChatApp.Business.Interfaces.Authentication;

public interface IJwtProvider
{
    string Generate(User user);
}