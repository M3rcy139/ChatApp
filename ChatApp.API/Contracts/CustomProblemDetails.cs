using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Contracts;

public class CustomProblemDetails : ProblemDetails
{
    public IEnumerable<object>? Errors { get; set; }
}
