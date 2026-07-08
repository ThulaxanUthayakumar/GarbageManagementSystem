using GMS.Domain.Common;
using GMS.Domain.Enums;
using GMS.Domain.Identity;

namespace GMS.Domain.Entities;

/// <summary>
/// Uploaded image metadata (the binary lives on disk / blob storage at
/// StoragePath). Attached to a complaint (evidence), a collection record
/// (proof) or a user profile, depending on Type.
/// </summary>
public class Image : BaseEntity
{
    public string FileName { get; set; } = string.Empty;     // original client file name
    public string StoragePath { get; set; } = string.Empty;  // relative path or blob URL
    public string ContentType { get; set; } = string.Empty;  // image/jpeg, image/png, image/webp
    public long FileSizeBytes { get; set; }

    public ImageType Type { get; set; }

    public int UploadedByUserId { get; set; }   // FK -> AspNetUsers
    public int? ComplaintId { get; set; }       // FK -> Complaints (nullable)
    public int? CollectionRecordId { get; set; } // FK -> CollectionRecords (nullable)

    // Navigation
    public ApplicationUser UploadedByUser { get; set; } = null!;
    public Complaint? Complaint { get; set; }
    public CollectionRecord? CollectionRecord { get; set; }
}
