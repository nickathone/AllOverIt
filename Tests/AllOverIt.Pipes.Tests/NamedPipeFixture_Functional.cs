using AllOverIt.Fixture;
using System;

namespace AllOverIt.Pipes.Tests
{
    public class NamedPipeFixture_Functional : FixtureBase
    {
        private class DummyMessage
        {
            public int Id { get; set; }
            public string Value { get; set; }
            public Guid Guid { get; set; }
        }


    }
}