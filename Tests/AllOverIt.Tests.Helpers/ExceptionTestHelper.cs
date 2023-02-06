using AllOverIt.Extensions;
using AutoFixture;
using FluentAssertions;
using System;

namespace AllOverIt.Tests.Helpers
{
    public static class ExceptionTestHelper
    {
        public static void AssertDefaultConstructor<TException>(this IFixture fixture) where TException : Exception, new()
        {
            var exception = new TException();

            var expected = $"Exception of type '{typeof(TException).FullName}' was thrown.";

            exception.Message.Should().Be(expected);
        }

        public static void AssertNoDefaultConstructor<TException>(this IFixture fixture) where TException : Exception
        {
            var constructor = typeof(TException).GetConstructor(Type.EmptyTypes);

            constructor.Should().BeNull();
        }

        public static void AssertConstructorWithMessage<TException>(this IFixture fixture) where TException : Exception
        {
            var message = fixture.Create<string>();

            var constructor = typeof(TException).GetConstructor(new Type[] { typeof(string) });

            var exception = constructor.Invoke(new[] { message });

            exception
                .GetPropertyValue<string>("Message")
                .Should()
                .Be(message);
        }

        public static void AssertNoConstructorWithMessage<TException>(this IFixture fixture) where TException : Exception
        {
            var message = fixture.Create<string>();

            var constructor = typeof(TException).GetConstructor(new Type[] { typeof(string) });

            constructor.Should().BeNull();
        }

        public static void AssertConstructorWithMessageAndInnerException<TException>(this IFixture fixture) where TException : Exception
        {
            var message = fixture.Create<string>();
            var innerException = new Exception();

            var constructor = typeof(TException).GetConstructor(new Type[] { typeof(string), typeof(Exception) });

            var exception = constructor.Invoke(new object[] { message, innerException });

            exception
                .GetPropertyValue<string>("Message")
                .Should()
                .Be(message);

            exception
                .GetPropertyValue<Exception>("InnerException")
                .Should()
                .BeSameAs(innerException);
        }
    }
}