using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BlazorRecipes.Shared.Models;

[DataContract]
public class Reviews
{
    [Key]
    [DataMember]
    public long? Id { get; set; }

    [DataMember]
    public decimal Rating { get; set; }

    [StringLength(50, ErrorMessage = "Text must be no more than 50 characters.")]
    [DataMember]
    public string? Comment { get; set; }

    [DataMember]
    public string? User { get; set; }
}
