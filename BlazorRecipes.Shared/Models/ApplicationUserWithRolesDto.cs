using System.ComponentModel.DataAnnotations;

namespace BlazorRecipes.Shared.Models;

public class ApplicationUserWithRolesDto : ApplicationUserDto
{
    public List<string>? Roles { get; set; }
}
