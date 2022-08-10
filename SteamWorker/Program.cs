using SteamWorker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostc, services) =>
    {
        services.Configure<Settings>(hostc.Configuration.GetSection("Settings"));
        services.AddHostedService<Bot>().AddSingleton<GameClaimerFactory, WebDriverFactory>();
    })
    .Build();
await host.RunAsync();
