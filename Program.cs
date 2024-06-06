using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using Rungs;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<HttpClient>(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazoredLocalStorageAsSingleton();


builder.Services.AddSingleton<DebugService>();
//builder.Services.AddSingleton<ITransliterator, TransliteratorService>();
builder.Services.AddSingleton<ProfileService>();
builder.Services.AddSingleton<FileService>();

WebAssemblyHost host = builder.Build();

ProfileService profileService = host.Services.GetRequiredService<ProfileService>();
await profileService.InitializeAsync();

FileService fileService = host.Services.GetRequiredService<FileService>();
await fileService.InitializeAsync();

await host.RunAsync();
