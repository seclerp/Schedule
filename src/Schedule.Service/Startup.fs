module Startup

open Domain.ScheduleContext
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.EntityFrameworkCore
open Swashbuckle.AspNetCore
open Swashbuckle.AspNetCore.Swagger

type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        // Add framework services.
        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1) |> ignore
        services.AddSwaggerGen(fun c ->
            c.SwaggerDoc("v1", new Info())
        ) |> ignore
        services.AddEntityFrameworkMySql() |> ignore
        services.AddDbContext<ScheduleContext>(
            fun o -> o.UseMySql(this.Configuration.GetValue<string>("ConnectionStrings:Schedule"),
                                fun o -> o.MigrationsAssembly("Schedule.Migrations") |> ignore
                     ) |> ignore
        ) |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =
        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
        else
            app.UseHsts() |> ignore
            app.UseHttpsRedirection() |> ignore

        app.UseSwagger() |> ignore
        app.UseSwaggerUI(fun c -> c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1")) |> ignore
        app.UseMvc() |> ignore

    member val Configuration : IConfiguration = null with get, set