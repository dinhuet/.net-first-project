using System.ComponentModel.DataAnnotations;

namespace BlogApp.Application.DTO.Request.Category;

public record CreateCategoryRequestDto
( 
    [Required(ErrorMessage = "Category's name is required")]
    [MaxLength(100, ErrorMessage = "Category's name cannot exceed 100 characters")]
    string Name
    );