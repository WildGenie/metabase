using Guid = System.Guid;
using System.Collections.Generic;
using System.Threading.Tasks;
using CancellationToken = System.Threading.CancellationToken;
using Icon.Infrastructure.Command;
using Icon.Events;
using Icon.Infrastructure.Aggregate;
using Icon.Infrastructure.Query;
using Models = Icon.Models;
using Queries = Icon.Queries;
using Events = Icon.Events;
using Aggregates = Icon.Aggregates;
using System.Linq;
using Marten;
using Marten.Linq.MatchesSql;
using IError = HotChocolate.IError;
using CSharpFunctionalExtensions;

namespace Icon.Handlers
{
    public sealed class GetComponentsAtTimestampsHandler
      : GetModelsAtTimestampsHandler<Models.Component, Aggregates.ComponentAggregate>
    {
        public GetComponentsAtTimestampsHandler(IAggregateRepository repository)
          : base(repository)
        {
        }

        protected override async Task<IEnumerable<ValueObjects.Id>> QueryModelIds(
            IAggregateRepositoryReadOnlySession session,
            CancellationToken cancellationToken
            )
        {
            return (await session.Query<Events.ComponentCreated>()
              .Select(e => e.ComponentId)
              .ToListAsync(cancellationToken))
              .Select(id => (ValueObjects.Id)id);
        }
    }
}