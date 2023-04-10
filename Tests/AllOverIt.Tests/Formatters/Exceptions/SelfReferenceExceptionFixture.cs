﻿using AllOverIt.Fixture;
using AllOverIt.Formatters.Exceptions;
using Xunit;

namespace AllOverIt.Tests.Formatters.Exceptions
{
    public class SelfReferenceExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<SelfReferenceException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<SelfReferenceException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<SelfReferenceException>();
        }
    }
}