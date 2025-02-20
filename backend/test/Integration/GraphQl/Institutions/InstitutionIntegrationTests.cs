using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Metabase.GraphQl.Institutions;

namespace Metabase.Tests.Integration.GraphQl.Institutions
{
    public abstract class InstitutionIntegrationTests
      : IntegrationTests
    {
        internal static CreateInstitutionInput OperativeInstitutionInput { get; } = new(
            Name: "Institution A",
            Abbreviation: "I!A",
            Description: "Best institution ever!",
            WebsiteLocator: new Uri("https://institution-a.com"),
            PublicKey: null,
            State: Enumerations.InstitutionState.OPERATIVE,
            OwnerIds: Array.Empty<Guid>()
            );

        internal static IEnumerable<CreateInstitutionInput> InstitutionInputs
        {
            get
            {
                yield return OperativeInstitutionInput;
            }
        }

        internal static IEnumerable<object[]> EnumerateInstitutionInputs()
        {
            yield return new object[] { nameof(OperativeInstitutionInput), OperativeInstitutionInput };
        }

        protected Task<string> GetInstitutions()
        {
            return GetInstitutions(HttpClient);
        }

        internal static Task<string> GetInstitutions(
            HttpClient httpClient
        )
        {
            return SuccessfullyQueryGraphQlContentAsString(
                httpClient,
                File.ReadAllText("Integration/GraphQl/Institutions/GetInstitutions.graphql")
                );
        }

        protected Task<string> GetInstitution(
            string uuid
            )
        {
            return GetInstitution(HttpClient, uuid);
        }

        internal static Task<string> GetInstitution(
            HttpClient httpClient,
            string uuid
            )
        {
            return SuccessfullyQueryGraphQlContentAsString(
                httpClient,
                File.ReadAllText("Integration/GraphQl/Institutions/GetInstitution.graphql"),
                variables: new Dictionary<string, object?>
                {
                    ["uuid"] = uuid
                }
                );
        }

        protected Task<string> CreateInstitution(
            CreateInstitutionInput input
            )
        {
            return CreateInstitution(HttpClient, input);
        }

        internal static Task<string> CreateInstitution(
            HttpClient httpClient,
            CreateInstitutionInput input
            )
        {
            return SuccessfullyQueryGraphQlContentAsString(
                httpClient,
                File.ReadAllText("Integration/GraphQl/Institutions/CreateInstitution.graphql"),
                variables: input
                );
        }

        protected Task<JsonElement> CreateInstitutionAsJson(
            CreateInstitutionInput input
            )
        {
            return CreateInstitutionAsJson(HttpClient, input);
        }

        internal static Task<JsonElement> CreateInstitutionAsJson(
            HttpClient httpClient,
            CreateInstitutionInput input
            )
        {
            return SuccessfullyQueryGraphQlContentAsJson(
                httpClient,
                File.ReadAllText("Integration/GraphQl/Institutions/CreateInstitution.graphql"),
                variables: input
                );
        }

        protected Task<Guid> CreateInstitutionReturningUuid(
            CreateInstitutionInput input
        )
        {
            return CreateInstitutionReturningUuid(HttpClient, input);
        }

        internal static async Task<Guid> CreateInstitutionReturningUuid(
            HttpClient httpClient,
            CreateInstitutionInput input
        )
        {
            var response = await CreateInstitutionAsJson(httpClient, input).ConfigureAwait(false);
            return new Guid(
                ExtractString(
                    "$.data.createInstitution.institution.uuid",
                    response
                )
            );
        }

        protected async Task<(string, string)> CreateInstitutionReturningIdAndUuid(
            CreateInstitutionInput input
        )
        {
            var response = await CreateInstitutionAsJson(input).ConfigureAwait(false);
            return (
                ExtractString(
                    "$.data.createInstitution.institution.id",
                    response
                ),
                ExtractString(
                    "$.data.createInstitution.institution.uuid",
                    response
                )
            );
        }
    }
}