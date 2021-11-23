using AllOverIt.Collections;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Collections.Generic;

namespace AllOverIt.Tests.Collections
{
    public class ListFixture : FixtureBase
    {
        public class EmptyReadOnly : ListFixture
        {
            public void Should_Return_Empty_List()
            {
                var actual = List.EmptyReadOnly<int>();

                actual.Should().BeEmpty();
            }

            public void Should_Return_As_ReadOnly()
            {
                var actual = List.EmptyReadOnly<int>();

                actual.Should().BeOfType<IReadOnlyList<int>>();
            }
        }
    }
}
