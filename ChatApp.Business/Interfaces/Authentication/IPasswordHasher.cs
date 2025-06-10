namespace ChatApp.Business.Interfaces.Authentication;

public interface IPasswordHasher
{
    byte[] Generate(string password);
    bool Verify(string password, byte[] hashedPassword);
}