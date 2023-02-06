using AllOverIt.Csv.Exceptions;
using AllOverIt.Fixture;
using AllOverIt.Tests.Helpers;
using Xunit;

namespace AllOverIt.Csv.Tests.Exceptions
{
    public class CsvExportExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            Fixture.AssertDefaultConstructor<CsvExportException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            Fixture.AssertConstructorWithMessage<CsvExportException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            Fixture.AssertConstructorWithMessageAndInnerException<CsvExportException>();
        }
    }
}