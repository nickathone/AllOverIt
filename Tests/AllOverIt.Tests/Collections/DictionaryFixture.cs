using AllOverIt.Collections;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Collections.Generic;

namespace AllOverIt.Tests.Collections
{
    public class DictionaryFixture : FixtureBase
    {
        public class EmptyReadOnly : DictionaryFixture
        {
            public void Should_Return_Empty_List()
            {
                var actual = Dictionary.EmptyReadOnly<int, string>();

                actual.Should().BeEmpty();
            }

            public void Should_Return_As_ReadOnly()
            {
                var actual = Dictionary.EmptyReadOnly<int, string>();

                actual.Should().BeOfType<IReadOnlyDictionary<int, string>>();
            }
        }
    }
}
