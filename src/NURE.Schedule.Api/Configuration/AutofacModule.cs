using System.Data;
using System.Data.SqlClient;
using Autofac;
using NURE.Schedule.Domain.CistApi.Repositories;
using NURE.Schedule.Domain.CistApi.Repositories.Interfaces;
using NURE.Schedule.Domain.Repositories;
using NURE.Schedule.Domain.Repositories.Interfaces;
using NURE.Schedule.Services;
using NURE.Schedule.Services.Interfaces;

namespace NURE.Schedule.Api.Configuration
{
  public class AutofacModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      RegisterRepositories(builder);
      RegisterServices(builder);
    }

    private void RegisterRepositories(ContainerBuilder builder)
    {
      builder.RegisterType<CistRepository>()
        .As<ICistRepository>();

      builder.RegisterType<SearchItemRepository>()
        .As<ISearchItemRepository>();

      builder.RegisterType<LastUpdateRepository>()
        .As<ILastUpdateRepository>();

      builder.RegisterType<EventsRepository>()
        .As<IEventsRepository>();
    }

    private void RegisterServices(ContainerBuilder builder)
    {
      builder.RegisterType<SearchService>()
        .As<ISearchService>();

      builder.RegisterType<CistService>()
        .As<ICistService>();

      builder.RegisterType<RelevanceService>()
        .As<IRelevanceService>();
    }
  }
}