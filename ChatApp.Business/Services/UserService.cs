using System.Security.Authentication;
using ChatApp.Business.Interfaces.Authentication;
using ChatApp.Business.Interfaces.Services;
using ChatApp.DataAccess.Interfaces;
using ChatApp.Domain.Constants;
using ChatApp.Domain.Extensions;
using ChatApp.Domain.Models;

namespace ChatApp.Business.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public UserService(
        IUserRepository usersRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }
    
    public async Task Register(string userName, string phoneNumber, string password)
    {
        await UserExistsAsync(userName, phoneNumber);
        
        var hashedPassword = _passwordHasher.Generate(password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            PhoneNumber = phoneNumber,
            PasswordHash = hashedPassword,
        };
        
        await _usersRepository.AddAsync(user);
    }

    public async Task<string> Login(string phoneNumber, string password)
    {
        var user = await _usersRepository.GetByPhoneNumberAsync(phoneNumber);

        var result = _passwordHasher.Verify(password, user.PasswordHash);
        result.ThrowIfFalse(() => new AuthenticationException(ErrorMessages.FailedToLogin));

        var token = _jwtProvider.Generate(user);

        return token;
    }

    private async Task UserExistsAsync(string userName, string phoneNumber)
    {
        var userNameExists = await _usersRepository.UserNameExistsAsync(userName);
        userNameExists.ThrowIfTrue(() => new InvalidOperationException(ErrorMessages.AlreadyExistsUserName));
        
        var phoneNumberExists = await _usersRepository.UserPhoneNumberExistsAsync(phoneNumber);
        phoneNumberExists.ThrowIfTrue(() => new InvalidOperationException(ErrorMessages.AlreadyExistsPhoneNumber));
    }
}