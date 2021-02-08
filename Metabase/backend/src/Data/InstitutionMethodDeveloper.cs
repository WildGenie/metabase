using System;

namespace Metabase.Data
{
    public sealed class InstitutionMethodDeveloper
      : Infrastructure.Data.Entity
    {
        public Guid MethodId { get; set; }
        public Method Method { get; set; } = default;

        public Guid InstitutionId { get; set; }
        public Institution Institution { get; set; } = default;
    }
}