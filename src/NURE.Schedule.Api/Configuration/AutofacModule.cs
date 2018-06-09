using System.Data;
using System.Data.SqlClient;
using Autofac;
using NURE.Schedule.Domain.CistApi.Repositories;
using NURE.Schedule.Domain.CistApi.Repositories.Interfaces;
using NURE.Schedule.Domain.Repositories;
using NURE.Schedule.Domain.Repositories.Interfaces;

namespace NURE.Schedule.Api.Configuration
{
  public class AutofacModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      RegisterRepositories(builder);
    }

    private void RegisterRepositories(ContainerBuilder builder)
    {
      builder.RegisterType<CistRepository>()
        .As<ICistRepository>();
      
      builder.RegisterType<TeachersRepository>()
        .As<ITeachersRepository>();
      
      builder.RegisterType<GroupsRepository>()
        .As<IGroupsRepository>();
      
      builder.RegisterType<LastUpdateRepository>()
        .As<ILastUpdateRepository>();
      
      builder.RegisterType<EventsRepository>()
        .As<IEventsRepository>();
    }
  }
}