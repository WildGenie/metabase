namespace Icon.ValueObjects
{
    public abstract class AddManyToManyAssociationInput
      : AddAssociationInput
    {
        protected AddManyToManyAssociationInput(
            Id parentId,
            Id associateId
            )
          : base(
              parentId: parentId,
              associateId: associateId
              )
        {
        }
    }
}