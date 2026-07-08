namespace GarbageManagementSystem.API.DTOs.WasteCategories;

public class WasteCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconName { get; set; }
}

public class CreateWasteCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconName { get; set; }
}

public class UpdateWasteCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconName { get; set; }
}
