namespace GMS.Domain.Enums;

public enum ComplaintStatus
{
    Pending = 1,     // Created by resident, awaiting admin review
    Approved = 2,    // Verified by municipal admin
    Rejected = 3,    // Invalid / duplicate / out of scope
    Assigned = 4,    // Collector assigned
    InProgress = 5,  // Collector working on it
    Resolved = 6,    // Collector marked done (proof uploaded)
    Closed = 7       // Confirmed closed by admin or resident
}
