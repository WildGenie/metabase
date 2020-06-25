using Icon.Infrastructure.Aggregate;

namespace Icon.Handlers
{
    public abstract class GetManyToManyAssociationsOfModelsHandler<TModel, TAssociationModel, TAggregate, TAssociationAggregate>
      : GetAssociationsOfModelsHandler<TModel, TAssociationModel, TAggregate, TAssociationAggregate>
      where TAggregate : class, IEventSourcedAggregate, IConvertible<TModel>, new()
      where TAssociationAggregate : class, Aggregates.IManyToManyAssociationAggregate, IConvertible<TAssociationModel>, new()
    {
        public GetManyToManyAssociationsOfModelsHandler(
            IAggregateRepository repository
            )
          : base(repository)
        {
        }
    }
}