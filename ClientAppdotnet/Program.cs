using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ClientAppdotnet;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// During development the API runs on localhost:5034 per launchSettings; use that as the API base.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5034") });
builder.Services.AddScoped<ProductService>();

await builder.Build().RunAsync();
