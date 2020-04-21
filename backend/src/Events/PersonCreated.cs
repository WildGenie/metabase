using Icon;
using Errors = Icon.Errors;
using CSharpFunctionalExtensions;
using Guid = System.Guid;
using Uri = System.Uri;
using System.Collections.Generic;
using DateTime = System.DateTime;
using System.Threading.Tasks;
using Icon.Infrastructure;
using Icon.Infrastructure.Command;
using Icon.Events;
using Icon.Infrastructure.Aggregate;
using Commands = Icon.Commands;

namespace Icon.Events
{
    public sealed class PersonCreated : Event
    {
        public static PersonCreated From(
            Guid personId,
            Commands.CreatePerson command
            )
        {
            return new PersonCreated(
                personId: personId,
                name: command.Input.Name,
                phoneNumber: command.Input.PhoneNumber,
                postalAddress: command.Input.PostalAddress,
                emailAddress: command.Input.EmailAddress,
                websiteLocator: command.Input.WebsiteLocator,
                creatorId: command.CreatorId
                );
        }

        public Guid PersonId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalAddress { get; set; }
        public string EmailAddress { get; set; }
        public Uri WebsiteLocator { get; set; }

#nullable disable
        public PersonCreated() { }
#nullable enable

        public PersonCreated(
            Guid personId,
            string name,
            string phoneNumber,
            string postalAddress,
            string emailAddress,
            Uri websiteLocator,
            Guid creatorId
            )
          : base(creatorId)
        {
            PersonId = personId;
            Name = name;
            PhoneNumber = phoneNumber;
            PostalAddress = postalAddress;
            EmailAddress = emailAddress;
            WebsiteLocator = websiteLocator;
            EnsureValid();
        }

        public override Result<bool, Errors> Validate()
        {
            return
              Result.Combine(
                  base.Validate(),
                  ValidateNonEmpty(PersonId, nameof(PersonId)),
                  ValidateNonNull(Name, nameof(Name)),
                  ValidateNonNull(PhoneNumber, nameof(PhoneNumber)),
                  ValidateNonNull(PostalAddress, nameof(PostalAddress)),
                  ValidateNonNull(EmailAddress, nameof(EmailAddress)),
                  ValidateNonNull(WebsiteLocator, nameof(WebsiteLocator))
                  );
        }
    }
}