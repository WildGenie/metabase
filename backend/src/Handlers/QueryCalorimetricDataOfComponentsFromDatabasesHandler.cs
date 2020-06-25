using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Icon.Infrastructure.Aggregate;
using DateTime = System.DateTime;
using Guid = System.Guid;

namespace Icon.Handlers
{
    public sealed class QueryCalorimetricDataOfComponentsFromDatabasesGraphQlResponse
    {
        public Guid Id { get; set; }
        public Guid ComponentId { get; set; }
        public System.Text.Json.JsonElement? Data { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public sealed class QueryCalorimetricDataOfComponentsFromDatabasesHandler
      : QueryDataArrayOfComponentsFromDatabasesHandler<
          Models.CalorimetricDataFromDatabase,
          QueryCalorimetricDataOfComponentsFromDatabasesGraphQlResponse
        >
    {
        public QueryCalorimetricDataOfComponentsFromDatabasesHandler(
                        IAggregateRepository repository
                        )
          : base(
              graphQlQueryName: "calorimetricData",
              graphQlQueryFields: @"
                id
                componentId
                data
                timestamp
              ",
              repository: repository
              )
        {
        }

        protected override Result<Models.CalorimetricDataFromDatabase, Errors> ParseGraphQlResponse(
            Models.Database database,
            QueryCalorimetricDataOfComponentsFromDatabasesGraphQlResponse calorimetricDataResponse,
            IReadOnlyList<object> path
            )
        {
            var idResult = ValueObjects.Id.From(calorimetricDataResponse.Id, path.Append("id").ToList().AsReadOnly());
            var componentIdResult = ValueObjects.Id.From(calorimetricDataResponse.ComponentId, path.Append("componentId").ToList().AsReadOnly());
            var dataResult =
              calorimetricDataResponse.Data is null
              ? Result.Failure<ValueObjects.CalorimetricDataJson, Errors>(
                  Errors.One(
                    message: $"The value for key `data` is of the calorimetric data GraphQL response {calorimetricDataResponse.Id} with timestamp {calorimetricDataResponse.Timestamp} for component {calorimetricDataResponse.ComponentId} from database {database.Id} is `null`",
                    code: ErrorCodes.InvalidValue,
                    path: path.Append("data").ToList().AsReadOnly()
                    )
                  )
              : ValueObjects.CalorimetricDataJson.FromJsonElement(
                  calorimetricDataResponse.Data ?? throw new ArgumentNullException(nameof(calorimetricDataResponse.Data)), // TODO Why does the null-forgiving operator `!` not work here?
                  path.Append("data").ToList().AsReadOnly()
                  );
            var timestampResult = ValueObjects.Timestamp.From(calorimetricDataResponse.Timestamp, path: path.Append("timestamp").ToList().AsReadOnly());
            return
              Errors.Combine(
                  idResult,
                  componentIdResult,
                  dataResult,
                  timestampResult
                  )
              .Bind(_ =>
                  Models.CalorimetricDataFromDatabase.From(
                    id: idResult.Value,
                    databaseId: database.Id,
                    componentId: componentIdResult.Value,
                    data: dataResult.Value,
                    timestamp: timestampResult.Value
                    )
                  );
        }
    }
}