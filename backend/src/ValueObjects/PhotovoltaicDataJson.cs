using System.Collections.Generic;
using System.Collections.Immutable;
using CSharpFunctionalExtensions;

namespace Icon.ValueObjects
{
    public sealed class PhotovoltaicDataJson
      : DataJson
    {
        private static readonly IImmutableSet<string> JsonSchemaUris
          = JsonSchema.FilterUris(@"^.*/photovoltaic\.json$").ToImmutableHashSet();

        private PhotovoltaicDataJson(Json json)
          : base(json)
        {
        }

        public static Result<PhotovoltaicDataJson, Errors> FromNestedCollections(
            object? nestedCollectionsJson,
            IReadOnlyList<object>? path = null
            )
        {
            return
              Json.FromNestedCollections(nestedCollectionsJson, path)
              .Bind(json =>
                  FromJson(json, path)
                  );
        }

        public static Result<PhotovoltaicDataJson, Errors> FromJsonElement(
            System.Text.Json.JsonElement jsonElement,
            IReadOnlyList<object>? path = null
            )
        {
            return
              Json.FromJsonElement(jsonElement, path)
              .Bind(json =>
                  FromJson(json, path)
                  );
        }

        private static Result<PhotovoltaicDataJson, Errors> FromJson(
            Json json,
            IReadOnlyList<object>? path = null
            )
        {
            return
              FromJson(
                  json,
                  JsonSchemaUris,
                  validatedJson => new PhotovoltaicDataJson(validatedJson),
                  path
                  );
        }
    }
}