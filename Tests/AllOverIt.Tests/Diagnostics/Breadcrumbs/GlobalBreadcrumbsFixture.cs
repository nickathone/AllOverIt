﻿using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Fixture;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Diagnostics.Breadcrumbs
{
    [Collection("GlobalBreadcrumbs")]
    public class GlobalBreadcrumbsFixture : FixtureBase
    {
        [Fact]
        public void Should_Create_Instance_With_Default_Options()
        {
            var instance = GlobalBreadcrumbs.Instance;

            instance.Should().BeAssignableTo<IBreadcrumbs>();
            instance.Enabled.Should().BeTrue();
        }

        [Fact]
        public void Should_Create_With_Options()
        {
            var options = new BreadcrumbsOptions
            {
                StartEnabled = false
            };

            var instance = GlobalBreadcrumbs.Create(options);

            instance.Enabled.Should().BeFalse();
        }

        [Fact]
        public void Should_Return_Same_Instance()
        {
            var instance1 = GlobalBreadcrumbs.Instance;
            var instance2 = GlobalBreadcrumbs.Instance;

            instance1.Should().BeSameAs(instance2);
        }

        [Fact]
        public void Should_Create_New_Instance_After_Destroy()
        {
            var instance1 = GlobalBreadcrumbs.Instance;

            GlobalBreadcrumbs.Destroy();

            var instance2 = GlobalBreadcrumbs.Instance;

            instance1.Should().NotBeSameAs(instance2);
        }
    }
}
