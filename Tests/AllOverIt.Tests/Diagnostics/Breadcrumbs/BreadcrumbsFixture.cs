using AllOverIt.Collections;
using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Diagnostics.Breadcrumbs
{
    public class BreadcrumbsFixture : FixtureBase
    {
        private readonly AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs _breadcrumbs = new();

        public class Constructor : BreadcrumbsFixture
        {
            [Fact]
            public void Should_Create_Default_Options()
            {
                var actual = _breadcrumbs.Options;

                var expected = new BreadcrumbsOptions();

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class Constructor_Options_ThreadSafe : BreadcrumbsFixture
        {
            [Fact]
            public async Task Should_Be_Thread_Safe()
            {
                var options = new BreadcrumbsOptions
                {
                    ThreadSafe = true
                };

                AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs breadcrumbs = new(options);

                var tasks = Enumerable.Range(1, 100).Select(_ =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        var breadcrumb = Create<BreadcrumbData>();

                        breadcrumbs.Add(breadcrumb);
                    });
                });

                await Task.WhenAll(tasks).ConfigureAwait(false);

                breadcrumbs.Count().Should().Be(100);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public async Task Should_Have_Max_Capacity(bool threadSafe)
            {
                var options = new BreadcrumbsOptions
                {
                    ThreadSafe = threadSafe,
                    MaxCapacity = 10
                };

                var breadcrumbData = CreateMany<BreadcrumbData>(100);

                AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs breadcrumbs = new(options);

                if (threadSafe)
                {
                    var tasks = Enumerable.Range(0, 100).Select(index =>
                    {
                        return Task.Factory.StartNew(() =>
                        {
                            breadcrumbs.Add(breadcrumbData[index]);
                        });
                    });

                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
                else
                {
                    Enumerable.Range(0, 100).ForEach((_, index) =>
                    {
                        breadcrumbs.Add(breadcrumbData[index]);
                    });
                }

                var expected = breadcrumbData.Skip(90);
                var actual = breadcrumbs.ToList();

                actual.Count.Should().Be(10);
                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class Enabled : BreadcrumbsFixture
        {
            [Fact]
            public void Should_Add_Breadcrumb()
            {
                var breadcrumb = Create<BreadcrumbData>();

                _breadcrumbs.Add(breadcrumb);

                var actual = _breadcrumbs.ToList();

                actual.Single().Should().BeSameAs(breadcrumb);
            }

            [Fact]
            public void Should_Not_Add_Breadcrumb()
            {
                var breadcrumbs = new AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs
                {
                    Enabled = false
                };

                var breadcrumb = Create<BreadcrumbData>();

                breadcrumbs.Add(breadcrumb);

                var actual = breadcrumbs.ToList();

                actual.Should().BeEmpty();
            }
        }

        public class Add : BreadcrumbsFixture
        {
            [Fact]
            public void Should_Throw_Null_When_Breadcrumb_Null()
            {
                Invoking(() =>
                {
                    _breadcrumbs.Add(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("breadcrumb");
            }

            [Fact]
            public void Should_Add_Breadcrumb()
            {
                var breadcrumb = Create<BreadcrumbData>();

                _breadcrumbs.Add(breadcrumb);

                var actual = _breadcrumbs.ToList();

                actual.Single().Should().BeSameAs(breadcrumb);
            }

            [Fact]
            public void Should_Limit_Number_Of_Breadcrumbs()
            {
                var options = new BreadcrumbsOptions
                {
                    MaxCapacity = 5
                };

                var breadcrumbs = new AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs(options);

                var items = CreateMany<BreadcrumbData>(10);

                foreach (var item in items)
                {
                    breadcrumbs.Add(item);
                }

                var actual = breadcrumbs.ToList();

                actual.Should().HaveCount(5);

                var expected = items.Skip(5).Take(5);

                actual.Should().Contain(expected);
            }
        }

        public class Clear : BreadcrumbsFixture
        {
            [Fact]
            public void Should_Clear_Breadcrumbs()
            {
                var breadcrumb = Create<BreadcrumbData>();

                _breadcrumbs.Add(breadcrumb);

                _breadcrumbs.Should().HaveCount(1);

                _breadcrumbs.Clear();

                _breadcrumbs.Should().BeEmpty();
            }

            [Fact]
            public async Task Should_Not_Reset_StartTimestamp()
            {
                var breadcrumbs = new AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs();
                var originalTimestamp = breadcrumbs.StartTimestamp;

                await Task.Delay(100);

                breadcrumbs.Clear();
                breadcrumbs.StartTimestamp.Should().Be(originalTimestamp);
            }
        }

        public class Reset : BreadcrumbsFixture
        {
            [Fact]
            public void Should_Clear_Breadcrumbs()
            {
                var breadcrumb = Create<BreadcrumbData>();

                _breadcrumbs.Add(breadcrumb);

                _breadcrumbs.Should().HaveCount(1);

                _breadcrumbs.Reset();

                _breadcrumbs.Should().BeEmpty();
            }

            [Fact]
            public async Task Should_Reset_StartTimestamp()
            {
                var breadcrumbs = new AllOverIt.Diagnostics.Breadcrumbs.Breadcrumbs();
                var originalTimestamp = breadcrumbs.StartTimestamp;

                await Task.Delay(100);

                breadcrumbs.Reset();
                breadcrumbs.StartTimestamp.Should().NotBe(originalTimestamp);
            }
        }

        public class GetEnumerator : BreadcrumbsFixture
        {
            [Fact]
            public void Should_Return_Empty_When_No_Breadcrumbs()
            {
                var enumerator = _breadcrumbs.GetEnumerator();

                var count = 0;

                while (enumerator.MoveNext())
                {
                    count++;
                }

                count.Should().Be(0);
            }

            [Fact]
            public void Should_Get_Breadcrumbs()
            {
                var breadcrumb1 = new BreadcrumbData
                {
                    CallerName = string.Empty,
                    FilePath = string.Empty,
                    LineNumber = 0,
                    Message = Create<string>(),
                    Metadata = new { Value = Create<int>() }
                };

                var breadcrumb2 = new BreadcrumbData
                {
                    CallerName = null,
                    FilePath = null,
                    LineNumber = 0,
                    Message = Create<string>()
                };

                var breadcrumb3 = new BreadcrumbData
                {
                    CallerName = Create<string>(),
                    FilePath = Create<string>(),
                    LineNumber = Create<int>(),
                    Message = Create<string>(),
                    Metadata = Create<int>()
                };

                _breadcrumbs.Add(breadcrumb1);
                _breadcrumbs.Add(breadcrumb2);
                _breadcrumbs.Add(breadcrumb3);

                var actual = new List<BreadcrumbData>();

                var enumerator = _breadcrumbs.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    actual.Add(enumerator.Current);
                }

                var expected = new[]
                {
                    new
                    {
                        breadcrumb1.CallerName,
                        breadcrumb1.FilePath,
                        breadcrumb1.LineNumber,
                        breadcrumb1.Message,
                        breadcrumb1.Metadata
                    },
                    new
                    {
                        breadcrumb2.CallerName,
                        breadcrumb2.FilePath,
                        breadcrumb2.LineNumber,
                        breadcrumb2.Message,
                        breadcrumb2.Metadata
                    },
                    new
                    {
                        breadcrumb3.CallerName,
                        breadcrumb3.FilePath,
                        breadcrumb3.LineNumber,
                        breadcrumb3.Message,
                        breadcrumb3.Metadata
                    }
                };

                expected
                    .Should()
                    .BeEquivalentTo(
                        actual,
                        options => options
                            .Excluding(model => model.Timestamp)
                            .Excluding(model => model.TimestampUtc));
            }
        }

        public class GetEnumerator_Explicit : BreadcrumbsFixture
        {
            [Fact]
            public void Should_Return_Empty_When_No_Breadcrumbs()
            {
                var enumerator = ((IEnumerable)_breadcrumbs).GetEnumerator();

                var count = 0;

                while (enumerator.MoveNext())
                {
                    count++;
                }

                count.Should().Be(0);
            }

            [Fact]
            public void Should_Get_Breadcrumbs()
            {
                var breadcrumb1 = new BreadcrumbData
                {
                    CallerName = string.Empty,
                    FilePath = string.Empty,
                    LineNumber = 0,
                    Message = Create<string>(),
                    Metadata = new { Value = Create<int>() }
                };

                var breadcrumb2 = new BreadcrumbData
                {
                    CallerName = null,
                    FilePath = null,
                    LineNumber = 0,
                    Message = Create<string>()
                };

                var breadcrumb3 = new BreadcrumbData
                {
                    CallerName = Create<string>(),
                    FilePath = Create<string>(),
                    LineNumber = Create<int>(),
                    Message = Create<string>(),
                    Metadata = Create<int>()
                };

                _breadcrumbs.Add(breadcrumb1);
                _breadcrumbs.Add(breadcrumb2);
                _breadcrumbs.Add(breadcrumb3);

                var actual = new List<BreadcrumbData>();

                var enumerator = ((IEnumerable) _breadcrumbs).GetEnumerator();

                while (enumerator.MoveNext())
                {
                    var value = (BreadcrumbData)enumerator.Current;

                    actual.Add(value);
                }

                var expected = new[]
                {
                    new
                    {
                        breadcrumb1.CallerName,
                        breadcrumb1.FilePath,
                        breadcrumb1.LineNumber,
                        breadcrumb1.Message,
                        breadcrumb1.Metadata
                    },
                    new
                    {
                        breadcrumb2.CallerName,
                        breadcrumb2.FilePath,
                        breadcrumb2.LineNumber,
                        breadcrumb2.Message,
                        breadcrumb2.Metadata
                    },
                    new
                    {
                        breadcrumb3.CallerName,
                        breadcrumb3.FilePath,
                        breadcrumb3.LineNumber,
                        breadcrumb3.Message,
                        breadcrumb3.Metadata
                    }
                };

                expected
                    .Should()
                    .BeEquivalentTo(
                        actual,
                        options => options
                            .Excluding(model => model.Timestamp)
                            .Excluding(model => model.TimestampUtc));
            }
        }
    }
}
