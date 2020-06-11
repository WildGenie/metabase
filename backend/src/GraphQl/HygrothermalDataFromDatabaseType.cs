using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using HotChocolate.Types;
using Array = System.Array;
using Console = System.Console;
using DateTime = System.DateTime;
using ValueObjects = Icon.ValueObjects;

namespace Icon.GraphQl
{
    public sealed class HygrothermalDataFromDatabaseType
      : ObjectType<HygrothermalDataFromDatabase>
    {
        protected override void Configure(IObjectTypeDescriptor<HygrothermalDataFromDatabase> descriptor)
        {
            // `AnyType` https://github.com/ChilliCream/hotchocolate/issues/1051#issuecomment-613576432
            descriptor
              .Field(t => t.Data)
              .Type<NonNullType<AnyType>>();
        }
    }
}