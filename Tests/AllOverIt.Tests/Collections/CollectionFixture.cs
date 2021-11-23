using AllOverIt.Collections;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Collections.Generic;

namespace AllOverIt.Tests.Collections
{
    public class CollectionFixture : FixtureBase
    {
        public class EmptyReadOnly : CollectionFixture
        {
            public void Should_Return_Empty_List()
            {
                var actual = Collection.EmptyReadOnly<int>();

                actual.Should().BeEmpty();
            }

            public void Should_Return_As_ReadOnly()
            {
                var actual = Collection.EmptyReadOnly<int>();

                actual.Should().BeOfType<IReadOnlyCollection<int>>();
            }
        }
    }
}
