using Guid = System.Guid;
using DateTime = System.DateTime;
using System.Threading.Tasks;
using CancellationToken = System.Threading.CancellationToken;
using Icon.Infrastructure;
using Icon.Infrastructure.Command;
using Icon.Infrastructure.Event;
using Icon.Infrastructure.Aggregate;
using Models = Icon.Models;

namespace Icon.Commands
{
    public sealed class CreateComponentVersion
      : CommandBase<(Guid Id, DateTime Timestamp)>
    {
        public Guid ComponentId { get; }

        public CreateComponentVersion(Guid componentId, Guid creatorId)
          : base(creatorId)
        {
            ComponentId = componentId;
        }
    }
}