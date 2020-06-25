using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Icon.Infrastructure.Aggregate;
using Icon.Infrastructure.Query;
using CancellationToken = System.Threading.CancellationToken;

namespace Icon.Handlers
{
    public sealed class GetModelsForTimestampedIdsHandler<M, A>
      : IQueryHandler<Queries.GetModelsForTimestampedIds<M>, IEnumerable<Result<M, Errors>>>,
        IGetModelsForTimestampedIdsHandler
      where M : Models.IModel
      where A : class, IEventSourcedAggregate, IConvertible<M>, new()
    {
        private readonly IAggregateRepository _repository;

        public GetModelsForTimestampedIdsHandler(IAggregateRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Result<M, Errors>>> Handle(
            Queries.GetModelsForTimestampedIds<M> query,
            CancellationToken cancellationToken
            )
        {
            using (var session = _repository.OpenReadOnlySession())
            {
                return await Handle(session, query.TimestampedIds, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Result<M, Errors>>> Handle(
            IAggregateRepositoryReadOnlySession session,
            IEnumerable<ValueObjects.TimestampedId> timestampedIds,
            CancellationToken cancellationToken
            )
        {
            return
              (await session
               .LoadAll<A>(
                 timestampedIds,
                 cancellationToken: cancellationToken
                 )
               .ConfigureAwait(false)
              )
              .Select(result =>
                result.Bind(a => a.ToModel())
                );
        }

        public async Task<IEnumerable<Result<Models.IModel, Errors>>> HandleX(
            IAggregateRepositoryReadOnlySession session,
            IEnumerable<ValueObjects.TimestampedId> timestampedIds,
            CancellationToken cancellationToken
            )
        {
            return
              (await Handle(session, timestampedIds, cancellationToken)
               .ConfigureAwait(false)
               )
              .Select(r => r.Map(m => (Models.IModel)m));
        }
    }
}