using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Infrastructure.Queries;
using Infrastructure.ValueObjects;
using Errors = Infrastructure.Errors;

namespace Infrastructure.Queries
{
    public sealed class GetModelsAtTimestamps<M>
      : IQuery<IEnumerable<Result<IEnumerable<Result<M, Errors>>, Errors>>>
    {
        public IReadOnlyCollection<Timestamp> Timestamps { get; }

        private GetModelsAtTimestamps(
            IReadOnlyCollection<Timestamp> timestamps
            )
        {
            Timestamps = timestamps;
        }

        public static Result<GetModelsAtTimestamps<M>, Errors> From(
            IReadOnlyCollection<Timestamp> timestamps
            )
        {
            return Result.Success<GetModelsAtTimestamps<M>, Errors>(
                    new GetModelsAtTimestamps<M>(
                        timestamps
                        )
                    );
        }
    }
}