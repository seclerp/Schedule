cd ..\Schedule.Migrations
dotnet ef --startup-project=..\Schedule.Api database update
cd ..\scripts