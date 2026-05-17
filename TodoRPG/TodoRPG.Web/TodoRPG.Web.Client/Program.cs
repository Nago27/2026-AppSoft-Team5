using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;
using TodoRPG.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5187/")
    });

builder.Services.AddScoped<TodoApiService>();

await builder.Build().RunAsync();
