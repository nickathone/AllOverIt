using AllOverIt.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Helpers
{
  public class GuardFixture : AllOverItFixtureBase
  {
    private class DummyClass
    {
    }

    public class WhenNotNull_Expression : GuardFixture
    {
      [Fact]
      public void Should_Throw_When_Expression_Null()
      {
        Invoking(() =>
          {
            Guard.WhenNotNull((Expression<Func<DummyClass>>)null);
          })
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
      }

      [Fact]
      public void Should_Throw_When_Null()
      {
        Invoking(() =>
          {
            DummyClass subject = null;

            Guard.WhenNotNull(() => subject);
          })
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("subject"));
      }

      [Fact]
      public void Should_Throw_When_Not_MemberExpression()
      {
        Invoking(() =>
          {
            Guard.WhenNotNull(() => (DummyClass)null);
          })
          .Should()
          .Throw<ArgumentException>()
          .WithMessage("expression must be a LambdaExpression containing a MemberExpression");
      }

      [Fact]
      public void Should_Return_Expression_Value()
      {
        Invoking(() =>
          {
            var expected = new DummyClass();

            var actual = Guard.WhenNotNull(() => expected);

            actual.Should().BeSameAs(expected);
          })
          .Should()
          .NotThrow();
      }
    }

    public class WhenNotNullOrEmpty_Expression_Type : GuardFixture
    {
      [Fact]
      public void Should_Throw_When_Expression_Null()
      {
        Invoking(() =>
          {
            Guard.WhenNotNullOrEmpty((Expression<Func<IEnumerable<DummyClass>>>)null);
          })
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
      }

      [Fact]
      public void Should_Throw_When_Null()
      {
        Invoking(() =>
          {
            IEnumerable<DummyClass> subject = null;

            Guard.WhenNotNullOrEmpty(() => subject);
          })
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("subject"));
      }

      [Fact]
      public void Should_Throw_When_Empty()
      {
        var expected = new List<DummyClass>();

        Invoking(() =>
          {
            var actual = Guard.WhenNotNullOrEmpty(() => expected);

            actual.Should().BeSameAs(expected);
          })
          .Should()
          .Throw<ArgumentException>()
          .WithMessage($"The argument cannot be empty (Parameter '{nameof(expected)}')");
      }

      [Fact]
      public void Should_Throw_When_Not_MemberExpression()
      {
        Invoking(() =>
          {
            Guard.WhenNotNullOrEmpty(() => (IEnumerable<DummyClass>)null);
          })
          .Should()
          .Throw<ArgumentException>()
          .WithMessage("expression must be a LambdaExpression containing a MemberExpression");
      }

      [Fact]
      public void Should_Return_Expression_Value()
      {
        Invoking(() =>
          {
            var expected = new List<DummyClass>
            {
              new DummyClass()
            };

            var actual = Guard.WhenNotNullOrEmpty(() => expected);

            actual.Should().BeSameAs(expected);
          })
          .Should()
          .NotThrow();
      }
    }

    public class WhenNotNullOrEmpty_Expression_String : GuardFixture
    {
      [Fact]
      public void Should_Throw_When_Expression_Null()
      {
        Invoking(() =>
          {
            Guard.WhenNotNullOrEmpty((Expression<Func<string>>)null);
          })
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
      }

      [Fact]
      public void Should_Throw_When_Null()
      {
        Invoking(() =>
          {
            string subject = null;

            Guard.WhenNotNullOrEmpty(() => subject);
          })
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("subject"));
      }

      [Fact]
      public void Should_Throw_When_Empty()
      {
        var expected = string.Empty;

        Invoking(() =>
          {
            var actual = Guard.WhenNotNullOrEmpty(() => expected);

            actual.Should().BeSameAs(expected);
          })
          .Should()
          .Throw<ArgumentException>()
          .WithMessage($"The argument cannot be empty (Parameter '{nameof(expected)}')");
      }

      [Fact]
      public void Should_Throw_When_Not_MemberExpression()
      {
        Invoking(() =>
          {
            Guard.WhenNotNullOrEmpty(() => (string)null);
          })
          .Should()
          .Throw<ArgumentException>()
          .WithMessage("expression must be a LambdaExpression containing a MemberExpression");
      }

      [Fact]
      public void Should_Return_Expression_Value()
      {
        Invoking(() =>
          {
            var expected = Create<string>();

            var actual = Guard.WhenNotNullOrEmpty(() => expected);

            actual.Should().Be(expected);
          })
          .Should()
          .NotThrow();
      }
    }

    public class WhenNotNull : GuardFixture
    {
      [Fact]
      public void Should_Throw_When_Null()
      {
        var name = Create<string>();

        Invoking(() =>
          {
            DummyClass dummy = null;

            Guard.WhenNotNull(dummy, name);
          })
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage($"{name}"));
      }

      [Fact]
      public void Should_Not_Throw()
      {
        Invoking(() =>
          {
            var dummy = new DummyClass();

            Guard.WhenNotNull(dummy, Create<string>());
          })
          .Should()
          .NotThrow();
      }

      [Fact]
      public void Should_Return_Object()
      {
        var expected = new DummyClass();

        var actual = Guard.WhenNotNull(expected, Create<string>());

        actual.Should().BeSameAs(expected);
      }
    }

    public class WhenNotNullOrEmpty_Type : GuardFixture
    {
      [Fact]
      public void Should_Throw_When_Null()
      {
        var name = Create<string>();

        Invoking(() =>
          {
            IEnumerable<DummyClass> dummy = null;

            Guard.WhenNotNullOrEmpty(dummy, name);
          })
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage($"{name}"));
      }

      [Fact]
      public void Should_Throw_When_Empty()
      {
        var name = Create<string>();
        var expected = new List<DummyClass>();

        Invoking(() =>
          {
            Guard.WhenNotNullOrEmpty(expected, name);
          })
          .Should()
          .Throw<ArgumentException>()
          .WithMessage($"The argument cannot be empty (Parameter '{name}')");
      }

      [Fact]
      public void Should_Not_Throw()
      {
        Invoking(() =>
          {
            var dummy = new List<DummyClass>
            {
              new DummyClass()
            };

            Guard.WhenNotNullOrEmpty(dummy, Create<string>());
          })
          .Should()
          .NotThrow();
      }

      [Fact]
      public void Should_Return_Object()
      {
        var expected = new List<DummyClass>
        {
          new DummyClass()
        };

        var actual = Guard.WhenNotNullOrEmpty(expected, Create<string>());

        actual.Should().BeSameAs(expected);
      }
    }

    public class WhenNotNullOrEmpty_String : GuardFixture
    {
      [Fact]
      public void Should_Throw_When_Null()
      {
        var name = Create<string>();

        Invoking(() =>
          {
            string dummy = null;

            Guard.WhenNotNullOrEmpty(dummy, name);
          })
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage($"{name}"));
      }

      [Fact]
      public void Should_Throw_When_Empty()
      {
        var name = Create<string>();
        var expected = string.Empty;

        Invoking(() =>
          {
            Guard.WhenNotNullOrEmpty(expected, name);
          })
          .Should()
          .Throw<ArgumentException>()
          .WithMessage($"The argument cannot be empty (Parameter '{name}')");
      }

      [Fact]
      public void Should_Not_Throw()
      {
        Invoking(() =>
          {
            var dummy = Create<string>();

            Guard.WhenNotNullOrEmpty(dummy, Create<string>());
          })
          .Should()
          .NotThrow();
      }

      [Fact]
      public void Should_Return_Object()
      {
        var expected = Create<string>();

        var actual = Guard.WhenNotNullOrEmpty(expected, Create<string>());

        actual.Should().BeSameAs(expected);
      }
    }
  }
}