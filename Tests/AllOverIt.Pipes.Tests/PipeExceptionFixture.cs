﻿using AllOverIt.Fixture;
using AllOverIt.Pipes.Exceptions;
using Xunit;

namespace AllOverIt.Pipes.Tests
{
    public class PipeExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<PipeException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<PipeException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<PipeException>();
        }
    }
}