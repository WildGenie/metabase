using FluentAssertions;
using System.Threading.Tasks;
using Snapshooter.Xunit;
using Xunit;
using System.IO;

namespace Metabase.Tests.Integration.GraphQl.Users
{
    public sealed class ResendUserEmailVerificationTests
      : UserIntegrationTests
    {
        [Fact]
        public async Task LoggedInUser_ResendsUserEmailVerification()
        {
            // Arrange
            var email = "john.doe@ise.fraunhofer.de";
            var password = DefaultPassword;
            await RegisterAndConfirmAndLoginUser(
                email: email,
                password: password
                ).ConfigureAwait(false);
            EmailSender.Clear();
            // Act
            var response = await ResendUserEmailVerification().ConfigureAwait(false);
            // Assert
            Snapshot.Match(response);
            EmailsShouldContainSingle(
                address: email,
                subject: "Confirm your email",
                messageRegEx: @"^Please confirm your email address with the confirmation code \w+\.$"
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
