namespace GarbageManagementSystem.API.DTOs.Residents;

/// <summary>Full resident record returned by the API, joined with identity fields for display.</summary>
public class ResidentDto
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime DateJoined { get; set; }
    public int TotalRequests { get; set; }
    public int TotalComplaints { get; set; }
}

/// <summary>Used by an Admin to add a resident (creates the login account + profile together).</summary>
public class CreateResidentDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
}

public class UpdateResidentDto
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
