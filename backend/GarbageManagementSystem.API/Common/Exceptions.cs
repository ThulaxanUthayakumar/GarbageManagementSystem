namespace GarbageManagementSystem.API.Common;

/// <summary>
/// Thrown when a requested entity does not exist. Translated to HTTP 404 by
/// the global exception handling middleware.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} with id '{key}' was not found.")
    {
    }
}

/// <summary>
/// Thrown for business-rule violations that are the caller's fault.
/// Translated to HTTP 400 by the global exception handling middleware.
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }
}

/// <summary>
/// Thrown when an authenticated user tries to access a resource they do not own
/// and are not privileged enough to see. Translated to HTTP 403.
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "You do not have permission to perform this action.") : base(message)
    {
    }
}
