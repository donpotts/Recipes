using BlazorRecipes.Shared.Models;
namespace BlazorRecipes.Shared.Blazor;

public static class ReviewExtensions
{
    public static decimal AverageRating(this List<Reviews> reviews)
        => Math.Round(reviews.Select(review => review.Rating).DefaultIfEmpty(0).Average(), 1);
}
