using Orleans;

await Host.CreateDefaultBuilder(args)
    .UseOrleans(
        (context, builder) =>
        {
            if (context.HostingEnvironment.IsDevelopment())
            {
                builder.UseLocalhostClustering()
                    .AddMemoryGrainStorage("shopping-cart")
                    .AddStartupTask<SeedProductStoreTask>();
            }
            else
            {
                var connectionString = context.Configuration["ORLEANS_AZURE_STORAGE_CONNECTION_STRING"];
                builder.Configure<ClusterOptions>(options =>
                 {
                     options.ClusterId = "shopping-cart";
                     options.ServiceId = "shopping-cart";
                 })
                .Configure<SiloOptions>(options =>
                {
                    options.SiloName = "shopping-cart";
                })
               .ConfigureEndpoints(siloPort: 11_111, gatewayPort: 30_000)
               .UseAzureStorageClustering(
                    options => options.ConfigureTableServiceClient(connectionString));
                builder.AddAzureTableGrainStorage(
                    "shopping-cart",
                    options => options.ConfigureTableServiceClient(connectionString));
            }
        })
    .ConfigureWebHostDefaults(
        webBuilder => webBuilder.UseStartup<Startup>())
    .RunConsoleAsync();
