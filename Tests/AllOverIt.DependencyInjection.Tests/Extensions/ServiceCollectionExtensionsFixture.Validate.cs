using AllOverIt.DependencyInjection.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;
namespace AllOverIt.DependencyInjection.Tests.Extensions
{
    public partial class ServiceCollectionExtensionsFixture
    {
        public class Validate : ServiceCollectionExtensionsFixture
        {
            private class InnerClassDummy
            {
            }

            private class ParentClassDummy
            {
                public ParentClassDummy(InnerClassDummy inner)
                {
                }
            }

            [Fact]
            public void Should_Throw_When_Services_Null()
            {
                Invoking(() =>
                {
                    ServiceCollectionExtensions.Validate(null, A.Fake<IServiceProvider>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("services");
            }

            [Fact]
            public void Should_Throw_When_Provider_Null()
            {
                Invoking(() =>
                {
                    ServiceCollectionExtensions.Validate(A.Fake<IServiceCollection>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("provider");
            }

            [Fact]
            public void Should_Not_Throw_When_Services_Empty()
            {
                Invoking(() =>
                {
                    var services = new ServiceCollection();
                    var provider = services.BuildServiceProvider();

                    ServiceCollectionExtensions.Validate(services, provider);
                })
               .Should()
               .NotThrow();
            }

            [Fact]
            public void Should_Not_Report_Errors_When_All_Dependencies_Registered()
            {
                var services = new ServiceCollection();

                services.AddSingleton<InnerClassDummy>();
                services.AddSingleton<ParentClassDummy>();

                var provider = services.BuildServiceProvider();

                var errors = ServiceCollectionExtensions.Validate(services, provider);

                errors.Should().BeEmpty();
            }

            [Fact]
            public void Should_Report_Errors_When_Not_All_Dependencies_Registered()
            {
                var services = new ServiceCollection();

                services.AddSingleton<ParentClassDummy>();

                var provider = services.BuildServiceProvider();

                var errors = ServiceCollectionExtensions.Validate(services, provider);

                errors.Should().HaveCount(1);

                var error = errors.First();

                error.Exception.Should().NotBeNull();
                error.Service.Should().NotBeNull();
                error.Service.ImplementationType.Should().Be(typeof(ParentClassDummy));
            }
        }
    }
}