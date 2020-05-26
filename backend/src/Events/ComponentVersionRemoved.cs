using Errors = Icon.Errors;
using CSharpFunctionalExtensions;
using Guid = System.Guid;
using Commands = Icon.Commands;
using Newtonsoft.Json;

namespace Icon.Events
{
    public sealed class ComponentVersionRemoved
      : RemovedEvent
    {
        public static ComponentVersionRemoved From(
            Guid componentVersionId,
            Commands.Remove<ValueObjects.RemoveManyToManyAssociationInput<Models.ComponentVersion>> command
            )
        {
            return new ComponentVersionRemoved(
                componentVersionId: componentVersionId,
                creatorId: command.CreatorId
                );
        }

#nullable disable
        public ComponentVersionRemoved() { }
#nullable enable

        public ComponentVersionRemoved(
            Guid componentVersionId,
            Guid creatorId
            )
          : base(
              aggregateId: componentVersionId,
              creatorId: creatorId
              )
        {
            EnsureValid();
        }
    }
}