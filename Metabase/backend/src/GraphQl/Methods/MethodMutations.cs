using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using Metabase.Extensions;
using NpgsqlTypes;
using DateTime = System.DateTime;

namespace Metabase.GraphQl.Methods
{
    [ExtendObjectType(Name = nameof(Mutation))]
    public sealed class MethodMutations
    {
        [UseDbContext(typeof(Data.ApplicationDbContext))]
        [Authorize(Policy = Configuration.Auth.WritePolicy)]
        public async Task<CreateMethodPayload> CreateMethodAsync(
            CreateMethodInput input,
            [ScopedService] Data.ApplicationDbContext context,
            CancellationToken cancellationToken
            )
        {
            if (input.Standard is not null &&
                input.Publication is not null
                )
            {
                return new CreateMethodPayload(
                    new CreateMethodError(
                        CreateMethodErrorCode.TWO_REFERENCES,
                        "Specify either a standard or a publication as reference.",
                      new[] { nameof(input), nameof(input.Publication).FirstCharToLower() }
                    )
                );
            }
            var method = new Data.Method(
                name: input.Name,
                description: input.Description,
                validity: // TODO Put into helper method!
                 input.Validity is not null
                 ? new NpgsqlRange<DateTime>(
                   input.Validity.From.GetValueOrDefault(), true, input.Validity.From is null,
                   input.Validity.To.GetValueOrDefault(), true, input.Validity.To is null
                   )
                 : null,
                availability:
                 input.Availability is not null
                 ? new NpgsqlRange<DateTime>(
                   input.Availability.From.GetValueOrDefault(), true, input.Availability.From is null,
                   input.Availability.To.GetValueOrDefault(), true, input.Availability.To is null
                   )
                 : null,
                calculationLocator: input.CalculationLocator,
                categories: input.Categories
            )
            { // TODO The below is also used in `DataFormatMutations`. Put into helper!
                Standard =
                input.Standard is null
                 ? null
                 : new Data.Standard(
          title: input.Standard.Title,
          @abstract: input.Standard.Abstract,
          section: input.Standard.Section,
          year: input.Standard.Year,
          standardizers: input.Standard.Standardizers,
          locator: input.Standard.Locator
                )
                 {
                     Numeration = new Data.Numeration(
            prefix: input.Standard.Numeration.Prefix,
            mainNumber: input.Standard.Numeration.MainNumber,
            suffix: input.Standard.Numeration.Suffix
          )
                 },
                Publication =
input.Publication is null
? null
: new Data.Publication(
            title: input.Publication.Title,
            @abstract: input.Publication.Abstract,
            section: input.Publication.Section,
            authors: input.Publication.Authors,
            doi: input.Publication.Doi,
            arXiv: input.Publication.ArXiv,
            urn: input.Publication.Urn,
            webAddress: input.Publication.WebAddress
                )
            };
            ;
            context.Methods.Add(method);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return new CreateMethodPayload(method);
        }
    }
}