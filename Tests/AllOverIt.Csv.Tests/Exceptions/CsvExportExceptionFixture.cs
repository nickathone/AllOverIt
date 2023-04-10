using AllOverIt.Csv.Exceptions;
using AllOverIt.Fixture;
using Xunit;

namespace AllOverIt.Csv.Tests.Exceptions
{
    public class CsvExportExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<CsvExportException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<CsvExportException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<CsvExportException>();
        }
    }
}