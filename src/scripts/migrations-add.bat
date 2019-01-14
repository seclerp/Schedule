cd ..\Schedule.Migrations
dotnet ef --startup-project=..\Schedule.Api migrations add %1
cd ..\scripts