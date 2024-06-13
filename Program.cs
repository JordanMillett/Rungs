using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using Rungs;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<HttpClient>(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazoredLocalStorageAsSingleton();

builder.Services.AddSingleton<ServiceManager>();
builder.Services.AddSingleton<DebugService>();
builder.Services.AddSingleton<ProfileService>();
builder.Services.AddSingleton<FileService>();

WebAssemblyHost host = builder.Build();

ServiceManager Manager = host.Services.GetRequiredService<ServiceManager>();
await Manager.InitializeAsync();

await host.RunAsync();
