using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Snapshooter.Xunit;
using Xunit;

namespace Metabase.Tests.Integration.GraphQl.Users
{
    [Collection(nameof(Data.User))]
    public sealed class ResendUserEmailVerificationTests
      : UserIntegrationTests
    {
        [Fact]
        public async Task LoggedInUser_ResendsUserEmailVerification()
        {
            // Arrange
            var name = "John Doe";
            var email = "john.doe@ise.fraunhofer.de";
            var password = DefaultPassword;
            await RegisterAndConfirmAndLoginUser(
                name: name,
                email: email,
                password: password
                ).ConfigureAwait(false);
            EmailSender.Clear();
            // Act
            var response = await ResendUserEmailVerification().ConfigureAwait(false);
            // Assert
            Snapshot.Match(
                response,
                matchOptions => matchOptions.Assert(fieldOptions =>
                 fieldOptions.Field<string>("data.resendUserEmailVerification.user.id").Should().NotBeNullOrWhiteSpace()
                 )
                );
            EmailsShouldContainSingle(
                to: (name, email),
                subject: "Confirm your email",
                bodyRegEx: @"^Please confirm your email address by clicking the link https:\/\/local\.buildingenvelopedata\.org:4041\/users\/confirm-email\?email=john\.doe@ise\.fraunhofer\.de&confirmationCode=\w+\.$"
                );
        }

        [Fact]
        public async Task NonLoggedInUser_IsAuthenticationError()
        {
            // Arrange
            var email = "john.doe@ise.fraunhofer.de";
            var password = DefaultPassword;
            await RegisterAndConfirmUser(
                email: email,
                password: password
            ).ConfigureAwait(false);
            // Act
            var response = await UnsuccessfullyQueryGraphQlContentAsString(
                File.ReadAllText("Integration/GraphQl/Users/ResendUserEmailVerification.graphql")
                ).ConfigureAwait(false);
            // Assert
            Snapshot.Match(response);
        }
    }
}