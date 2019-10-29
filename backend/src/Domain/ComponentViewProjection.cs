using Marten.Events.Projections;
using Guid = System.Guid;
using Marten;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Icon.Domain
{
    public sealed class ComponentViewProjection : ViewProjection<ComponentView, Guid>
    {
        public ComponentViewProjection()
        {
            ProjectEvent<Component.Create.ComponentCreateEvent>(e => e.ComponentId, Apply);
            ProjectEventAsync<ComponentVersion.Create.ComponentVersionCreateEvent>(e => e.ComponentId, Apply);
        }

        private void Apply(ComponentView view, Component.Create.ComponentCreateEvent @event)
        {
            view.Id = @event.ComponentId;
        }

        private async Task Apply(IDocumentSession documentSession, ComponentView view, ComponentVersion.Create.ComponentVersionCreateEvent @event)
        {
            var version = await documentSession.LoadAsync<ComponentVersionView>(@event.ComponentVersionId);
            view.Versions.Add(version);
        }
    }
}