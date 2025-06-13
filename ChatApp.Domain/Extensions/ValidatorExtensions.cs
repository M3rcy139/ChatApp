namespace ChatApp.Domain.Extensions;

public static class ValidatorExtensions
{
    public static void ValidateEntity<T>(this T entity, string errorMessage)
        where T : class?
    {
        if (entity == null)
        {
            throw new ArgumentException(errorMessage);
        }
    }
    
    public static void ThrowIfTrue<TException>(this bool condition, Func<TException> exceptionFactory) where TException : Exception
    {
        if (condition)
            throw exceptionFactory();
    }

    public static void ThrowIfFalse<TException>(this bool condition, Func<TException> exceptionFactory) where TException : Exception
    {
        if (!condition)
            throw exceptionFactory();
    }
}