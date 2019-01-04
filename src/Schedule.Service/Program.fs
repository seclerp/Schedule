module Program

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Hosting
open System
open System.Diagnostics
open System.IO
open System.Text

open Domain.Models
open Startup

let exitCode = 0

let CreateWebHostBuilder args =
    WebHost
        .CreateDefaultBuilder(args)
        .UseStartup<Startup>();

[<EntryPoint>]
let main argv =
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
    CreateWebHostBuilder(argv).Build().Run()
    
    

    exitCode