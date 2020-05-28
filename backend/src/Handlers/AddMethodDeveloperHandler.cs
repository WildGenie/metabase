using System.Threading.Tasks;
using System.Linq; // Enumerable.Empty
using System; // Func
using CancellationToken = System.Threading.CancellationToken;
using Icon.Infrastructure.Command;
using Icon.Infrastructure.Aggregate;
using Events = Icon.Events;
using Commands = Icon.Commands;
using Aggregates = Icon.Aggregates;
using CSharpFunctionalExtensions;

namespace Icon.Handlers
{
    public sealed class AddMethodDeveloperHandler
      : ICommandHandler<Commands.AddAssociation<ValueObjects.AddMethodDeveloperInput>, Result<ValueObjects.TimestampedId, Errors>>
    {
        private readonly IAggregateRepository _repository;
        private readonly CreateModelHandler<Commands.AddAssociation<ValueObjects.AddMethodDeveloperInput>, Aggregates.InstitutionMethodDeveloperAggregate> _addInstitutionMethodDeveloperHandler;
        private readonly CreateModelHandler<Commands.AddAssociation<ValueObjects.AddMethodDeveloperInput>, Aggregates.PersonMethodDeveloperAggregate> _addPersonMethodDeveloperHandler;

        public AddMethodDeveloperHandler(IAggregateRepository repository)
        {
            _repository = repository;
            _addInstitutionMethodDeveloperHandler =
              new CreateModelHandler<Commands.AddAssociation<ValueObjects.AddMethodDeveloperInput>, Aggregates.InstitutionMethodDeveloperAggregate>(
                  repository,
                  Events.InstitutionMethodDeveloperAdded.From,
                  Enumerable.Empty<Func<IAggregateRepositorySession, Commands.AddAssociation<ValueObjects.AddMethodDeveloperInput>, CancellationToken, Task<Result<bool, Errors>>>>()
                  );
            _addPersonMethodDeveloperHandler =
              new CreateModelHandler<Commands.AddAssociation<ValueObjects.AddMethodDeveloperInput>, Aggregates.PersonMethodDeveloperAggregate>(
                  repository,
                  Events.PersonMethodDeveloperAdded.From,
                  Enumerable.Empty<Func<IAggregateRepositorySession, Commands.AddAssociation<ValueObjects.AddMethodDeveloperInput>, CancellationToken, Task<Result<bool, Errors>>>>()
                  );
        }

        public async Task<Result<ValueObjects.TimestampedId, Errors>> Handle(
            Commands.AddAssociation<ValueObjects.AddMethodDeveloperInput> command,
            CancellationToken cancellationToken
            )
        {
            using (var session = _repository.OpenSession())
            {
                return await
                  (await session.FetchAggregateType(
                      command.Input.StakeholderId,
                      cancellationToken
                      )
                    .ConfigureAwait(false)
                    )
                  .Bind(async aggregateType =>
                    {
                        if (aggregateType == typeof(Aggregates.InstitutionAggregate))
                            return await _addInstitutionMethodDeveloperHandler.Handle(command, session, cancellationToken)
                            .ConfigureAwait(false);
                        if (aggregateType == typeof(Aggregates.PersonAggregate))
                            return await _addPersonMethodDeveloperHandler.Handle(command, session, cancellationToken)
                            .ConfigureAwait(false);
                        return Result.Failure<ValueObjects.TimestampedId, Errors>(
                        Errors.One(
                          message: $"The stakeholder with id {command.Input.StakeholderId} has the aggregate type {aggregateType} which is none of the expected types {typeof(Aggregates.InstitutionAggregate)} and {typeof(Aggregates.PersonAggregate)}",
                          code: ErrorCodes.InvalidType
                          )
                        );
                    }
                    )
                  .ConfigureAwait(false);
            }
        }
    }
}