﻿using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pagination.TokenEncoding;
using FakeItEasy;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Pagination.Tests.TokenEncoding
{
    public class ContinuationTokenSerializerFixture : FixtureBase
    {
        public class Constructor : ContinuationTokenSerializerFixture
        {
            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                {
                    _ = new ContinuationTokenSerializer(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("tokenOptions");
            }
        }

        public class Serialize : ContinuationTokenSerializerFixture
        {
            [Fact]
            public void Should_Throw_When_Token_Null()
            {
                Invoking(() =>
                {
                    var serializer = new ContinuationTokenSerializer(A.Fake<IContinuationTokenOptions>());
                    _ = serializer.Serialize(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("continuationToken");
            }

            [Fact]
            public void Should_Serialize_Different()
            {
                var continuationToken1 = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>(),
                        Create<short>(),
                        Create<long>(),
                        Create<PaginationDirection>(),
                        Create<float>(),
                        Create<double>(),
                        Create<string>()
                    }
                };

                var continuationToken2 = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>(),
                        Create<short>(),
                        Create<long>(),
                        Create<PaginationDirection>(),
                        Create<float>(),
                        Create<double>(),
                        Create<string>()
                    }
                };

                var serializer = new ContinuationTokenSerializer(ContinuationTokenOptions.Default);

                var serialized1 = serializer.Serialize(continuationToken1);
                var serialized2 = serializer.Serialize(continuationToken2);

                serialized1.Should().NotBe(serialized2);
            }

            [Fact]
            public void Should_Compress_Smaller()
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>(),
                        Create<short>(),
                        Create<long>(),
                        Create<PaginationDirection>(),
                        Create<float>(),
                        Create<double>(),
                        Create<string>()
                    }
                };

                var serializer = new ContinuationTokenSerializer(new ContinuationTokenOptions { UseCompression = false });
                var notCompressed = serializer.Serialize(continuationToken);

                serializer = new ContinuationTokenSerializer(new ContinuationTokenOptions { UseCompression = true });
                var compressed = serializer.Serialize(continuationToken);

                notCompressed.Length
                    .Should()
                    .BeGreaterThan(compressed.Length);
            }

            [Fact]
            public void Should_Encode_Hash_Value_Different_To_None()
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>()
                    }
                };

                var serializer = new ContinuationTokenSerializer(new ContinuationTokenOptions { IncludeHash = false });
                var serialized1 = serializer.Serialize(continuationToken);

                serializer = new ContinuationTokenSerializer(new ContinuationTokenOptions { IncludeHash = true });
                var serialized2 = serializer.Serialize(continuationToken);

                serialized1.Should().NotBe(serialized2);
            }
        }

        public class Deserialize : ContinuationTokenSerializerFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Token_Null()
            {
                Invoking(() =>
                {
                    var serializer = new ContinuationTokenSerializer(ContinuationTokenOptions.Default);
                    var actual = serializer.Deserialize(null);

                    actual.Should().BeSameAs(ContinuationToken.None);
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Token_Empty()
            {
                Invoking(() =>
                {
                    var serializer = new ContinuationTokenSerializer(ContinuationTokenOptions.Default);
                    var actual = serializer.Deserialize(string.Empty);

                    actual.Should().BeSameAs(ContinuationToken.None);
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_Token_Whitespace()
            {
                Invoking(() =>
                {
                    var serializer = new ContinuationTokenSerializer(ContinuationTokenOptions.Default);
                    var actual = serializer.Deserialize(" ");

                    actual.Should().BeSameAs(ContinuationToken.None);
                })
                    .Should()
                    .NotThrow();
            }
        }

        public class Serialize_Deserialize : ContinuationTokenSerializerFixture
        {
            [Theory]
            [InlineData(true, true)]
            [InlineData(true, false)]
            [InlineData(false, true)]
            [InlineData(false, false)]
            public void Should_Serialize_Deserialize(bool includeHash, bool useCompression)
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>(),
                        Create<short>(),
                        Create<long>(),
                        Create<PaginationDirection>(),
                        Create<float>(),
                        Create<double>(),
                        Create<string>()
                    }
                };

                var tokenOptions = new ContinuationTokenOptions
                {
                    IncludeHash = includeHash,
                    UseCompression = useCompression
                };

                var serializer = new ContinuationTokenSerializer(tokenOptions);

                var encoded = serializer.Serialize(continuationToken);
                var decoded = serializer.Deserialize(encoded);

                decoded.Should().BeEquivalentTo(continuationToken);
            }
        }

        public class TryDeserialize : ContinuationTokenSerializerFixture
        {
            [Fact]
            public void Should_Throw_When_Token_Null()
            {
                Invoking(() =>
                {
                    var serializer = new ContinuationTokenSerializer(A.Fake<IContinuationTokenOptions>());
                    _ = serializer.TryDeserialize(null, out var _);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("continuationToken");
            }

            [Fact]
            public void Should_Return_False_When_Random_String()
            {
                var serializer = new ContinuationTokenSerializer(A.Fake<IContinuationTokenOptions>());
                var actual = serializer.TryDeserialize(Create<string>(), out var _);

                actual.Should().BeFalse();
            }

            [Fact]
            public void Should_Return_True_When_Valid_Token()
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>()
                    }
                };

                var serializer = new ContinuationTokenSerializer(ContinuationTokenOptions.Default);

                var tokenString = serializer.Serialize(continuationToken);

                var actual = serializer.TryDeserialize(tokenString, out var _);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Return_False_When_Invalid_Token()
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>()
                    }
                };

                var serializer = new ContinuationTokenSerializer(ContinuationTokenOptions.Default);

                var tokenString = serializer.Serialize(continuationToken);

                var actual = serializer.TryDeserialize($"12{tokenString}34", out var token);

                actual.Should().BeFalse();
                token.Should().BeNull();
            }

            [Fact]
            public void Should_Deserialize_Token()
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>()
                    }
                };

                var serializer = new ContinuationTokenSerializer(ContinuationTokenOptions.Default);

                var tokenString = serializer.Serialize(continuationToken);

                _ = serializer.TryDeserialize(tokenString, out var token);

                token.Should().BeEquivalentTo(continuationToken);
            }

            [Theory]
            [InlineData(true, true)]
            [InlineData(true, false)]
            [InlineData(false, true)]
            [InlineData(false, false)]
            public void Should_Deserialize_Token_When_Using_Options(bool includeHash, bool useCompression)
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[]
                    {
                        Create<bool>(),
                        Create<int>()
                    }
                };

                var serializer1 = new ContinuationTokenSerializer(ContinuationTokenOptions.Default);

                var serializer2 = new ContinuationTokenSerializer(new ContinuationTokenOptions
                {
                    IncludeHash = includeHash,
                    UseCompression = useCompression
                });

                var token1String = serializer1.Serialize(continuationToken);
                var token2String = serializer2.Serialize(continuationToken);

                _ = serializer1.TryDeserialize(token1String, out var token1);
                _ = serializer2.TryDeserialize(token2String, out var token2);

                token1.Should().BeEquivalentTo(continuationToken);
                token2.Should().BeEquivalentTo(continuationToken);
            }
        }
    }
}