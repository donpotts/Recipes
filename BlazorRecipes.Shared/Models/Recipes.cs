using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BlazorRecipes.Shared.Models;

[DataContract]
public class Recipes
{
    [Key]
    [DataMember]
    public long? Id { get; set; }

    [DataMember]
    public string? Name { get; set; }

    [DataMember]
    public string? Source { get; set; }

    [IgnoreDataMember]
    public string SourceShort => string.IsNullOrEmpty(Source) ? "My Kitchen" : Uri.TryCreate(Source, UriKind.Absolute, out var sourceUri) ? sourceUri.Authority : Source;

    [DataMember]
    public string? PrepTime { get; set; }

    [DataMember]
    public string? WaitTime { get; set; }

    [DataMember]
    public string? CookTime { get; set; }

    [DataMember]
    public long? Servings { get; set; }

    [DataMember]
    public string? RecipeIngredients { get; set; }

    [DataMember]
    public string? Comments { get; set; }

    [DataMember]
    public string? Instructions { get; set; }

    [DataMember]
    public string? Photo { get; set; }

    [DataMember]
    public List<Tags>? Tags { get; set; }

    [DataMember]
    public List<Ingredients>? Ingredients { get; set; }

    [DataMember]
    public List<Units>? Units { get; set; }

    [DataMember]
    public List<Reviews>? Reviews { get; set; }
}
