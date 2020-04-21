using System.Collections.Generic;
using Errors = Icon.Errors;
using Array = System.Array;
using Validatable = Icon.Validatable;
using Guid = System.Guid;
using DateTime = System.DateTime;
using CSharpFunctionalExtensions;
using IError = HotChocolate.IError;

namespace Icon.GraphQl
{
    public sealed class ComponentManufacturerMarketingInformation
    {
        public static ComponentManufacturerMarketingInformation FromModel(
            ValueObjects.ComponentManufacturerMarketingInformation model
            )
        {
            return new ComponentManufacturerMarketingInformation(
                componentInformation: model.ComponentInformation is null ? null : ComponentInformation.FromModel(model.ComponentInformation),
                institutionInformation: model.InstitutionInformation is null ? null : InstitutionInformation.FromModel(model.InstitutionInformation)
                );
        }

        public ComponentInformation? ComponentInformation { get; }
        public InstitutionInformation? InstitutionInformation { get; }

        public ComponentManufacturerMarketingInformation(
            ComponentInformation? componentInformation,
            InstitutionInformation? institutionInformation
            )
        {
            ComponentInformation = componentInformation;
            InstitutionInformation = institutionInformation;
        }
    }
}