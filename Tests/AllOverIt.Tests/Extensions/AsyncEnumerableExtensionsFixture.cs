﻿using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class AsyncEnumerableExtensionsFixture : AoiFixtureBase
    {
        public class ToListAsync : AsyncEnumerableExtensionsFixture
        {
            [Fact]
            public async Task Should_Convert_To_List()
            {
                var expected = CreateMany<string>();
                
                var actual = await GetStrings(expected)
                    .ToListAsync()
                    .ConfigureAwait(false);

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public async Task Should_Cancel_Conversion()
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();

                var actual = await GetStrings(CreateMany<string>())
                    .ToListAsync(cancellationTokenSource.Token)
                    .ConfigureAwait(false);

                actual.Should().BeEmpty();
            }
        }

        private static async IAsyncEnumerable<string> GetStrings(IReadOnlyList<string> strings)
        {
            foreach (var item in strings)
            {
                await Task.Yield();

                yield return item;
            }
        }
    }
}