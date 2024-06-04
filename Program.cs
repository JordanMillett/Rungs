using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Rungs;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<HttpClient>(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//builder.Services.AddSingleton<ITransliterator, TransliteratorService>();
builder.Services.AddSingleton<ISongService, SongService>();

WebAssemblyHost host = builder.Build();

ISongService songService = host.Services.GetRequiredService<ISongService>();
await songService.InitializeAsync();

await host.RunAsync();
