using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Xunit;
using WebApplicationFactoryClientOptions = Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions;
using IdentityModel.Client;
using FluentAssertions;

namespace Test.Integration.Web.Api.Controller
{
    public class ComponentsControllerTest : TestBase
    {
        public ComponentsControllerTest(CustomWebApplicationFactory factory) : base(factory) { }

        public class ComponentsTestBase : TestBase
        {
            protected Components.Client ComponentsClient { get; }
            protected ComponentVersions.Client ComponentVersionsClient { get; }

            public ComponentsTestBase(CustomWebApplicationFactory factory) : base(factory)
            {
                ComponentsClient = new Components.Client(HttpClient);
                ComponentVersionsClient = new ComponentVersions.Client(HttpClient);
            }
        }

        public class ListTest : ComponentsTestBase
        {
            public ListTest(CustomWebApplicationFactory factory) : base(factory)
            {
            }

            [Fact]
            public async Task WhenEmpty()
            {
                // Act
                var components = await ComponentsClient.List.Deserialized();
                // Assert
                components.Should().BeEmpty();
            }

            [Fact]
            public async Task WhenSingle()
            {
                // Arrange
                await Factory.SeedUsers();
                Factory.SeedAuth();
                await Authorize(HttpClient, "simon@icon.com", "simonSIMON123@");
                var component = new Components.ListClient.Output { id = await ComponentsClient.Post.Deserialized(), version = 1 };
                // Act
                var components = await ComponentsClient.List.Deserialized();
                // Assert
                components.Should().BeEquivalentTo(component);
            }

            [Fact]
            public async Task WhenMultiple()
            {
                // Arrange
                await Factory.SeedUsers();
                Factory.SeedAuth();
                await Authorize(HttpClient, "simon@icon.com", "simonSIMON123@");
                var component1 = new Components.ListClient.Output { id = await ComponentsClient.Post.Deserialized(), version = 1 };
                var component2 = new Components.ListClient.Output { id = await ComponentsClient.Post.Deserialized(), version = 1 };
                var component3 = new Components.ListClient.Output { id = await ComponentsClient.Post.Deserialized(), version = 1 };
                var component4 = new Components.ListClient.Output { id = await ComponentsClient.Post.Deserialized(), version = 1 };
                var component5 = new Components.ListClient.Output { id = await ComponentsClient.Post.Deserialized(), version = 1 };
                // Act
                var components = await ComponentsClient.List.Deserialized();
                // Assert
                components.Should().BeEquivalentTo(component1, component2, component3, component4, component5);
            }
        }

        public class GetTest : ComponentsTestBase
        {
            public GetTest(CustomWebApplicationFactory factory) : base(factory) { }

            [Fact]
            public async Task NonExistent()
            {
                // Arrange
                var componentId = Guid.NewGuid();
                // Act
                var httpResponse = await ComponentsClient.Get.Raw(componentId);
                // Assert
                httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }

            [Fact]
            public async Task ExistentNoVersions()
            {
                // Arrange
                await Factory.SeedUsers();
                Factory.SeedAuth();
                await Authorize(HttpClient, "simon@icon.com", "simonSIMON123@");
                var postedComponent = new Components.GetClient.Output
                {
                    id = await ComponentsClient.Post.Deserialized(),
                    versions = new List<Components.GetClient.VersionOutput>()
                };
                // Act
                var component = await ComponentsClient.Get.Deserialized(postedComponent.id);
                // Assert
                component.Should().BeEquivalentTo(postedComponent);
            }

            [Fact]
            public async Task ExistentSingleVersion()
            {
                // Arrange
                await Factory.SeedUsers();
                Factory.SeedAuth();
                await Authorize(HttpClient, "simon@icon.com", "simonSIMON123@");
                var postedVersions = new List<Components.GetClient.VersionOutput>();
                var postedComponent = new Components.GetClient.Output
                {
                    id = await ComponentsClient.Post.Deserialized(),
                    versions = postedVersions,
                };
                postedVersions.Add(
                  new Components.GetClient.VersionOutput
                  {
                      id = await ComponentVersionsClient.Post.Deserialized(postedComponent.id),
                      componentId = postedComponent.id,
                      ownerships = new List<Components.GetClient.OwnershipOutput>(),
                  }
                );
                // Act
                var component = await ComponentsClient.Get.Deserialized(postedComponent.id);
                // Assert
                component.Should().BeEquivalentTo(postedComponent);
            }

            [Fact]
            public async Task ExistentMultipleVersions()
            {
                // Arrange
                await Factory.SeedUsers();
                Factory.SeedAuth();
                await Authorize(HttpClient, "simon@icon.com", "simonSIMON123@");
                var postedVersions = new List<Components.GetClient.VersionOutput>();
                var postedComponent = new Components.GetClient.Output
                {
                    id = await ComponentsClient.Post.Deserialized(),
                    versions = postedVersions,
                };
                postedVersions.Add(
                  new Components.GetClient.VersionOutput
                  {
                      id = await ComponentVersionsClient.Post.Deserialized(postedComponent.id),
                      componentId = postedComponent.id,
                      ownerships = new List<Components.GetClient.OwnershipOutput>(),
                  }
                );
                postedVersions.Add(
                  new Components.GetClient.VersionOutput
                  {
                      id = await ComponentVersionsClient.Post.Deserialized(postedComponent.id),
                      componentId = postedComponent.id,
                      ownerships = new List<Components.GetClient.OwnershipOutput>(),
                  }
                );
                postedVersions.Add(
                  new Components.GetClient.VersionOutput
                  {
                      id = await ComponentVersionsClient.Post.Deserialized(postedComponent.id),
                      componentId = postedComponent.id,
                      ownerships = new List<Components.GetClient.OwnershipOutput>(),
                  }
                );
                // Act
                var component = await ComponentsClient.Get.Deserialized(postedComponent.id);
                // Assert
                component.Should().BeEquivalentTo(postedComponent);
            }
        }

        public class PostTest : ComponentsTestBase
        {
            public PostTest(CustomWebApplicationFactory factory) : base(factory) { }

            [Fact]
            public async Task Anonymously()
            {
                // Act
                var httpResponse = await ComponentsClient.Post.Raw();
                // Assert
                httpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            }

            [Fact]
            public async Task Authorized()
            {
                // Arrange
                // TODO Instead of seeds we should use the public API as much as possible. Otherwise the state of the database may not be a possible state in production.
                await Factory.SeedUsers();
                Factory.SeedAuth();
                await Authorize(HttpClient, "simon@icon.com", "simonSIMON123@");
                // Act
                var id = await ComponentsClient.Post.Deserialized();
                // Assert
                id.Should().NotBeEmpty();
            }
        }
    }
}