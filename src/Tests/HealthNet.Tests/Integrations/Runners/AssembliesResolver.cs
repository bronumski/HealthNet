using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace HealthNet.Integrations.Runners
{
  class AssembliesResolver : DefaultAssembliesResolver
  {
    public override ICollection<Assembly> GetAssemblies()
    {
      return new[]
      {
        typeof(HealthCheckController).Assembly
      };
    }
  }
}