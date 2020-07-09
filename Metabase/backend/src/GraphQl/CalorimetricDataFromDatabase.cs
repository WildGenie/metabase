using System.Threading.Tasks;
using GreenDonut;
using HotChocolate;
using Infrastructure.GraphQl;
using Infrastructure.ValueObjects;

namespace Metabase.GraphQl
{
    public sealed class CalorimetricDataFromDatabase
      : NodeBase
    {
        public static CalorimetricDataFromDatabase FromModel(
            Models.CalorimetricDataFromDatabase model,
            Timestamp requestTimestamp
            )
        {
            return new CalorimetricDataFromDatabase(
                id: model.Id,
                databaseId: model.DatabaseId,
                componentId: model.ComponentId,
                data: model.Data.ToNestedCollections(),
                timestamp: model.Timestamp,
                requestTimestamp: requestTimestamp
                );
        }

        public Id DatabaseId { get; }
        public Id ComponentId { get; }
        public object Data { get; }

        public CalorimetricDataFromDatabase(
            Id id,
            Id databaseId,
            Id componentId,
            object data,
            Timestamp timestamp,
            Timestamp requestTimestamp
            )
          : base(
              id: id,
              timestamp: timestamp,
              requestTimestamp: requestTimestamp
              )
        {
            DatabaseId = databaseId;
            ComponentId = componentId;
            Data = data;
        }

        public Task<Database> GetDatabase(
            [Parent] CalorimetricDataFromDatabase calorimetricData,
            [DataLoader] DatabaseDataLoader databaseLoader
            )
        {
            return databaseLoader.LoadAsync(
                TimestampHelpers.TimestampId(calorimetricData.DatabaseId, calorimetricData.RequestTimestamp)
                );
        }

        public Task<Component> GetComponent(
            [Parent] CalorimetricDataFromDatabase calorimetricData,
            [DataLoader] ComponentDataLoader componentLoader
            )
        {
            return componentLoader.LoadAsync(
                TimestampHelpers.TimestampId(calorimetricData.ComponentId, calorimetricData.RequestTimestamp)
                );
        }
    }
}