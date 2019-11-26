using DateTime = System.DateTime;
using Array = System.Array;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using ErrorBuilder = HotChocolate.ErrorBuilder;
using IError = HotChocolate.IError;
using ErrorCodes = Icon.ErrorCodes;

namespace Icon.ValueObjects
{
  public sealed class Timestamp
    : ValueObject
  {
    // TODO Use `NodaTime.ZonedDateTime`
    public DateTime Value { get; }

    private Timestamp(DateTime value)
    {
      Value = value;
    }

    public static Result<Timestamp, IError> From(
        DateTime timestamp,
        DateTime? now = null,
        IReadOnlyList<object>? path = null
        )
    {
      if (timestamp > now ?? DateTime.UtcNow)
        return Result.Failure<Timestamp, IError>(
            ErrorBuilder.New()
            .SetMessage("Timestamp is in the future")
            .SetCode(ErrorCodes.InvalidValue)
            .SetPath(path)
            .Build()
            );

      return Result.Ok<Timestamp, IError>(
          new Timestamp(timestamp)
          );
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return Value;
    }

    public static explicit operator Timestamp(DateTime timestamp)
    {
      return From(timestamp).Value;
    }

    public static implicit operator DateTime(Timestamp timestamp)
    {
      return timestamp.Value;
    }
  }
}
