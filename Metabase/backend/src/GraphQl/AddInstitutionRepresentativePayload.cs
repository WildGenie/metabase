using Infrastructure.GraphQl;
using Infrastructure.ValueObjects;
namespace Metabase.GraphQl
{
    public sealed class AddInstitutionRepresentativePayload
      : AddOrRemoveInstitutionRepresentativePayload
    {
        /* public ValueObjects.InstitutionRepresentativeRole Role { get; } */
        public InstitutionRepresentativeEdge InstitutionRepresentativeEdge { get; }
        public RepresentedInstitutionEdge RepresentedInstitutionEdge { get; }

        public AddInstitutionRepresentativePayload(
            InstitutionRepresentative institutionRepresentative
            )
          : base(
              institutionId: institutionRepresentative.InstitutionId,
              userId: institutionRepresentative.UserId,
              requestTimestamp: institutionRepresentative.RequestTimestamp
              )
        {
            InstitutionRepresentativeEdge = new InstitutionRepresentativeEdge(institutionRepresentative);
            RepresentedInstitutionEdge = new RepresentedInstitutionEdge(institutionRepresentative);
        }
    }
}