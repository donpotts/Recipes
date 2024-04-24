using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BlazorRecipes.Shared.Models;

[DataContract]
public class RecipeIngredients
{
    [Key]
    [DataMember]
    public long? Id { get; set; }

    [DataMember]
    public long? Quantity { get; set; }

    [DataMember]
    public long? RecipesId { get; set; }

    [DataMember]
    public long? IngredientsId { get; set; }

    [DataMember]
    public long? UnitsId { get; set; }

    [DataMember]
    public Recipes? Recipes { get; set; }

    [DataMember]
    public Ingredients? Ingredients { get; set; }

    [DataMember]
    public Units? Units { get; set; }
}
