using AllOverIt.Fixture;
using AllOverIt.Process;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Process
{
    public class ProcessOutputFixture : FixtureBase
    {
        public class Constructor : ProcessOutputFixture
        {
            [Fact]
            public void Should_Set_Properties()
            {
                var exitCode = Create<int>();
                var output = Create<string>();
                var error = Create<string>();

                var actual = new ProcessOutput(exitCode, output, error);

                var expected = new
                {
                    ExitCode = exitCode,
                    StandardOutput = output,
                    StandardError = error
                };

                expected.Should().BeEquivalentTo(actual);
            }
        }
    }
}