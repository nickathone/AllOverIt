using AllOverIt.DependencyInjection.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.DependencyInjection.Tests.Extensions
{
    public partial class ServiceCollectionExtensionsFixture
    {
        public class AddNamedServices : ServiceCollectionExtensionsFixture
        {
            private interface IDummyInterface
            {
            }

            private sealed class DummyType : IDummyInterface
            {
            }

            [Fact]
            public void Should_Throw_When_ServiceCollection_Null()
            {
                Invoking(() =>
                {
                    _ = ServiceCollectionExtensions.AddNamedServices<IDummyInterface>(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serviceCollection");
            }

            [Fact]
            public void Should_Return_INamedServiceBuilder()
            {
                var services = new ServiceCollection();

                var actual = ServiceCollectionExtensions.AddNamedServices<IDummyInterface>(services);

                actual.Should().BeAssignableTo<INamedServiceBuilder<IDummyInterface>>();
            }

            [Fact]
            public void Should_Register_INamedServiceResolver_TService_Singleton()
            {
                var services = new ServiceCollection();

                _ = ServiceCollectionExtensions.AddNamedServices<IDummyInterface>(services);

                var descriptor = services.Single(descriptor => descriptor.ServiceType == typeof(INamedServiceResolver<IDummyInterface>));

                descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Set_Provider_On_INamedServiceResolver_TService_Generic(ServiceLifetime lifetime)
            {
                var services = new ServiceCollection();

                var builder = ServiceCollectionExtensions.AddNamedServices<IDummyInterface>(services);

                var name = Create<string>();

                switch (lifetime)
                {
                    case ServiceLifetime.Singleton:
                        builder.AsSingleton<DummyType>(name);
                        break;

                    case ServiceLifetime.Scoped:
                        builder.AsScoped<DummyType>(name);
                        break;

                    case ServiceLifetime.Transient:
                        builder.AsTransient<DummyType>(name);
                        break;
                }

                var provider = services.BuildServiceProvider();

                var resolver = provider.GetRequiredService<INamedServiceResolver<IDummyInterface>>() as NamedServiceResolver<IDummyInterface>;

                resolver._provider.Should().BeAssignableTo<IServiceProvider>();
            }

            [Theory]
            [InlineData(ServiceLifetime.Singleton)]
            [InlineData(ServiceLifetime.Scoped)]
            [InlineData(ServiceLifetime.Transient)]
            public void Should_Set_Provider_On_INamedServiceResolver_TService_Type(ServiceLifetime lifetime)
            {
                var services = new ServiceCollection();

                var builder = ServiceCollectionExtensions.AddNamedServices<IDummyInterface>(services);

                var name = Create<string>();

                switch (lifetime)
                {
                    case ServiceLifetime.Singleton:
                        builder.AsSingleton(name, typeof(DummyType));
                        break;

                    case ServiceLifetime.Scoped:
                        builder.AsScoped(name, typeof(DummyType));
                        break;

                    case ServiceLifetime.Transient:
                        builder.AsTransient(name, typeof(DummyType));
                        break;
                }

                var provider = services.BuildServiceProvider();

                var resolver = provider.GetRequiredService<INamedServiceResolver<IDummyInterface>>() as NamedServiceResolver<IDummyInterface>;

                resolver._provider.Should().BeAssignableTo<IServiceProvider>();
            }

            [Fact]
            public void Should_Register_INamedServiceResolver_Singleton()
            {
                var services = new ServiceCollection();

                _ = ServiceCollectionExtensions.AddNamedServices<IDummyInterface>(services);

                var descriptor = services.Single(descriptor => descriptor.ServiceType == typeof(INamedServiceResolver));

                descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            }
        }
    }
}