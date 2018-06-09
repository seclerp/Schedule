using System.Data;
using System.Data.SqlClient;
using Autofac;
using NURE.Schedule.Domain.CistApi.Repositories;
using NURE.Schedule.Domain.CistApi.Repositories.Interfaces;

namespace NURE.Schedule.Api.Configuration
{
  public class AutofacModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      // The generic ILogger<TCategoryName> service was added to the ServiceCollection by ASP.NET Core.
      // It was then registered with Autofac using the Populate method in ConfigureServices.
//      builder.Register(c => new ValuesService(c.Resolve<ILogger<ValuesService>>()))
//        .As<IValuesService>()
//        .InstancePerLifetimeScope();

      builder.RegisterType<CistRepository>()
        .As<ICistRepository>();
      
      builder.RegisterType<SqlConnection>()
        .As<IDbConnection>();
    }
  }
}