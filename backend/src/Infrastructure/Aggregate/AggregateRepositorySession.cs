// Inspired by https://jasperfx.github.io/marten/documentation/scenarios/aggregates_events_repositories/

using Errors = Icon.Errors;
using ValueObjects = Icon.ValueObjects;
using System;
using System.Linq;
using System.Collections.Generic;
using Marten;
using IEventStore = Marten.Events.IEventStore;
using Marten.Linq;
using System.Threading.Tasks;
using Icon.Events;
using CancellationToken = System.Threading.CancellationToken;
using ErrorCodes = Icon.ErrorCodes;
using HotChocolate;
using CSharpFunctionalExtensions;

namespace Icon.Infrastructure.Aggregate
{
    public sealed class AggregateRepositorySession : AggregateRepositoryReadOnlySession, IAggregateRepositorySession
    {
        private readonly IDocumentSession _session;
        private readonly IEventBus _eventBus;
        private IEnumerable<IEvent> _unsavedEvents;

        public AggregateRepositorySession(IDocumentSession session, IEventBus eventBus)
          : base(session)
        {
            _session = session;
            _eventBus = eventBus;
            _unsavedEvents = Enumerable.Empty<IEvent>();
        }

        public Task<Result<bool, Errors>> Create<T>(
            Guid id,
            IEvent @event,
            CancellationToken cancellationToken
            )
          where T : class, IEventSourcedAggregate, new()
        {
            return Create<T>(
                id,
                new IEvent[] { @event },
                cancellationToken
                );
        }

        public Task<Result<bool, Errors>> Create<T>(
            Guid id,
            IEnumerable<IEvent> events,
            CancellationToken cancellationToken
            )
          where T : class, IEventSourcedAggregate, new()
        {
            AssertNotDisposed();
            return RegisterEvents(
                events,
                eventArray => _session.Events.StartStream<T>(id, eventArray),
                cancellationToken
                );
        }

        public Task<Result<bool, Errors>> Append<T>(
            ValueObjects.TimestampedId timestampedId,
            IEvent @event,
            CancellationToken cancellationToken
            ) where T : class, IEventSourcedAggregate, new()
        {
            return Append<T>(
                timestampedId,
                new IEvent[] { @event },
                cancellationToken
                );
        }

        public async Task<Result<bool, Errors>> Append<T>(
            ValueObjects.TimestampedId timestampedId,
            IEnumerable<IEvent> events,
            CancellationToken cancellationToken
            )
          where T : class, IEventSourcedAggregate, new()
        {
            AssertNotDisposed();
            return await (
                await FetchVersion<T>(
                  timestampedId,
                  cancellationToken
                  )
               .ConfigureAwait(false)
               )
              .Bind(async version => await
                  RegisterEvents(
                    events,
                    eventArray => _session.Events.Append(timestampedId.Id, version + 1, eventArray),
                    cancellationToken
                    )
                  .ConfigureAwait(false)
                )
              .ConfigureAwait(false);
        }

        public Task<Result<bool, Errors>> Delete<T>(
            ValueObjects.TimestampedId timestampedId,
            IEvent @event,
            CancellationToken cancellationToken
            )
          where T : class, IEventSourcedAggregate, new()
        {
            AssertNotDisposed();
            _session.Delete<T>(timestampedId.Id);
            return Append<T>(
                timestampedId,
                @event,
                cancellationToken
                );
        }

        public Task<Result<bool, Errors>> Delete<T>(
            ValueObjects.Id id,
            ValueObjects.Timestamp timestamp,
            IEvent @event,
            CancellationToken cancellationToken
            )
          where T : class, IEventSourcedAggregate, new()
        {
            return
              ValueObjects.TimestampedId.From(id, timestamp)
              .Bind(timestampedId =>
                Delete<T>(
                  timestampedId,
                  @event,
                  cancellationToken
                  )
                );
        }

        public async Task<Result<bool, Errors>> Delete<T>(
            IEnumerable<(ValueObjects.TimestampedId, IEvent)> timestampedIdsAndEvents,
            CancellationToken cancellationToken
            )
          where T : class, IEventSourcedAggregate, new()
        {
          var deletionResults = new List<Result<bool, Errors>>();
          foreach (var (timestampedId, @event) in timestampedIdsAndEvents)
          {
            deletionResults.Add(
                await Delete<T>(
                  timestampedId, @event, cancellationToken
                  )
                .ConfigureAwait(false)
                );
          }
          return Result.Combine<bool, Errors>(deletionResults);
        }

        public async Task<Result<bool, Errors>> Save(
            CancellationToken cancellationToken
            )
        {
            await _session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await _eventBus.Publish(_unsavedEvents).ConfigureAwait(false);
            _unsavedEvents = Enumerable.Empty<IEvent>();
            return
              await Task.FromResult<Result<bool, Errors>>(
                  Result.Ok<bool, Errors>(true)
                  )
              .ConfigureAwait(false);
        }

        private async Task<Result<bool, Errors>> RegisterEvents(
            IEnumerable<IEvent> events,
            Action<IEvent[]> action,
            CancellationToken cancellationToken
            )
        {
            var eventArray = events.ToArray();
            Event.EnsureValid(eventArray);
            await AssertExistenceOfCreators(
                eventArray.Select(@event => @event.CreatorId),
                cancellationToken
                )
              .ConfigureAwait(false);
            action(eventArray);
            _unsavedEvents = _unsavedEvents.Concat(eventArray);
            return
              await Task.FromResult<Result<bool, Errors>>(
                  Result.Ok<bool, Errors>(true)
                  )
              .ConfigureAwait(false);
        }

        private Task AssertExistenceOfCreators(
            IEnumerable<Guid> creatorIds,
            CancellationToken cancellationToken
            )
        {
            return Task.CompletedTask;
            /* foreach (var creatorId in creatorIds) */
            /* { */
            /*   if (!_session.Query<UserAggregate>().AnyAsync(user => user.Id == creatorId, cancellationToken)) */
            /*     throw new ArgumentException("Creator does not exist!", nameof(creatorId)); */
            /* } */
        }
    }
}