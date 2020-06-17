using Guid = System.Guid;

namespace Icon.Events
{
    public sealed class PhotovoltaicDataDeleted
      : DataDeletedEvent
    {
        public static PhotovoltaicDataDeleted From<TModel>(
            Commands.Delete<TModel> command
            )
        {
            return new PhotovoltaicDataDeleted(
                photovoltaicDataId: command.TimestampedId.Id,
                creatorId: command.CreatorId
                );
        }

#nullable disable
        public PhotovoltaicDataDeleted() { }
#nullable enable

        public PhotovoltaicDataDeleted(
            Guid photovoltaicDataId,
            Guid creatorId
            )
          : base(
              aggregateId: photovoltaicDataId,
              creatorId: creatorId
              )
        {
            EnsureValid();
        }
    }
}