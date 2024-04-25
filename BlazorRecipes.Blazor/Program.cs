using BlazorRecipes.Shared.Blazor;
using BlazorRecipes.Shared.Blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Main>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazorServices(builder.HostEnvironment.BaseAddress);
builder.Services.AddBrowserStorageService();
builder.Services.AddSingleton<TagService>();

await builder.Build().RunAsync();
