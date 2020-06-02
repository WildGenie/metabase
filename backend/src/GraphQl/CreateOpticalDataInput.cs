using Console = System.Console;
using System.Collections.Generic;
using System.Linq;
using Array = System.Array;
using DateTime = System.DateTime;
using ValueObjects = Icon.ValueObjects;
using CSharpFunctionalExtensions;
using HotChocolate.Types;

namespace Icon.GraphQl
{
    public sealed class CreateOpticalDataInput
    {
        public ValueObjects.Id ComponentId { get; }
        public object Data { get; }

        public CreateOpticalDataInput(
            ValueObjects.Id componentId,
            object data
            )
        {
            ComponentId = componentId;
            Data = data;
        }

        public static Result<ValueObjects.CreateOpticalDataInput, Errors> Validate(
            CreateOpticalDataInput self,
            IReadOnlyList<object> path
            )
        {
            var dataResult = ValueObjects.OpticalDataJson.From(
                self.Data,
                path.Append("data").ToList().AsReadOnly()
                );

            return
              dataResult.Bind(_ =>
                  ValueObjects.CreateOpticalDataInput.From(
                    componentId: self.ComponentId,
                    data: dataResult.Value
                    )
                  );
        }
    }
}
