// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Mvc;

namespace Orleans.ShoppingCart.Silo;

public sealed class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages()
            .AddRazorPagesOptions(options =>
            {
                options.Conventions
                       .ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
            });
        services.AddHttpContextAccessor();
        services.AddTransient<ShoppingCartService, ShoppingCartService>();
        services.AddTransient<InventoryService, InventoryService>();
        services.AddTransient<ProductService, ProductService>();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
        });
    }
}