﻿using AllOverIt.Fixture;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Tests.Helpers
{
    public class EnumHelperFixture : FixtureBase
    {
        private enum DummyEnum
        {
            Value1,
            Value2,
            Value3
        }

        public class GetEnumValues : EnumHelperFixture
        {
            [Fact]
            public void Should_Get_Enum_Values()
            {
                var actual = AllOverIt.Helpers.EnumHelper.GetEnumValues<DummyEnum>();

                var expected = new[] {DummyEnum.Value1, DummyEnum.Value2, DummyEnum.Value3};

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0150:Prefer 'null' check over type check", Justification = "Part of the test and readability")]
            public void Should_Return_As_IReadOnlyCollection()
            {
                var actual = AllOverIt.Helpers.EnumHelper.GetEnumValues<DummyEnum>();

                var isReadOnlyCollection = actual is IReadOnlyCollection<DummyEnum>;

                // IsOfType<> treats it as DummyEnum[]
                isReadOnlyCollection.Should().BeTrue();
            }
        }
    }
}
