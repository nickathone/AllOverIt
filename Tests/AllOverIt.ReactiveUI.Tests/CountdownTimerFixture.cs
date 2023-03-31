using AllOverIt.Collections;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using ReactiveUI;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.ReactiveUI.Tests
{
    public class CountdownTimerFixture : FixtureBase
    {
        public class Configure : CountdownTimerFixture
        {
            [Fact]
            public void Should_Initialize()
            {
                var totalMilliseconds = GetWithinRange(10000, 12000);
                var updateIntervalMilliseconds = GetWithinRange(1000, 1500);

                var timer = new CountdownTimer();
                timer.Configure(totalMilliseconds, updateIntervalMilliseconds);

                timer.TotalMilliseconds.Should().Be(totalMilliseconds);
                timer.TotalTimeSpan.Should().BeCloseTo(TimeSpan.FromMilliseconds(totalMilliseconds), TimeSpan.FromMilliseconds(1));
                timer.IsRunning.Should().BeFalse();

                // The timer hasn't started yet
                timer.RemainingMilliseconds.Should().Be(0);
                timer.RemainingTimeSpan.Should().Be(TimeSpan.FromMilliseconds(0));
            }

            [Fact]
            public void Should_Throw_When_Already_Running()
            {
                var totalMilliseconds = GetWithinRange(10000, 12000);
                var updateIntervalMilliseconds = GetWithinRange(1000, 1500);

                var timer = new CountdownTimer();
                timer.Configure(totalMilliseconds, updateIntervalMilliseconds);

                timer.Start();

                Invoking(() =>
                {
                    timer.Configure(totalMilliseconds, updateIntervalMilliseconds);
                })
                .Should()
                .Throw<InvalidOperationException>()
                .WithMessage("The countdown timer period cannot be modified while executing.");
            }        
        }

        public class Start : CountdownTimerFixture
        {
            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            public void Should_Update_Remaining_Time_When_Started(int skipFactor)
            {
                var totalMilliseconds = GetWithinRange(10000, 12000);
                var updateIntervalMilliseconds = GetWithinRange(1000, 1500);

                var timer = new CountdownTimer();
                timer.Configure(totalMilliseconds, updateIntervalMilliseconds);

                timer.TotalMilliseconds.Should().Be(totalMilliseconds);
                timer.TotalTimeSpan.Should().BeCloseTo(TimeSpan.FromMilliseconds(totalMilliseconds), TimeSpan.FromMilliseconds(1));
                timer.IsRunning.Should().BeFalse();

                var skipMilliseconds = skipFactor * GetWithinRange(100, 200);
                timer.Start(skipMilliseconds);

                var remaining = totalMilliseconds - skipMilliseconds;

                timer.RemainingMilliseconds.Should().Be(remaining);
                timer.RemainingTimeSpan.Should().BeCloseTo(TimeSpan.FromMilliseconds(remaining), TimeSpan.FromMilliseconds(1));
            }

            [Theory]
            [InlineData(0)]
            [InlineData(1)]
            public void Should_Update_Notifications_While_Running(int skipFactor)
            {
                var actualNotifications = new List<double>();

                double totalMilliseconds = (int) GetWithinRange(10000, 12000);
                double updateIntervalMilliseconds = (int) GetWithinRange(1000, 1500);
                var skipMilliseconds = skipFactor * GetWithinRange(100, 200);

                var expectedRemaining = totalMilliseconds - skipMilliseconds;
                var expectedNotifications = new List<double>(new[] { 0, totalMilliseconds - skipMilliseconds });

                while (expectedRemaining > 0)
                {
                    expectedRemaining -= updateIntervalMilliseconds;

                    if (expectedRemaining < 0)
                    {
                        expectedRemaining = 0;
                    }

                    expectedNotifications.Add(expectedRemaining);
                }

                var scheduler = new TestScheduler();
                scheduler.Start();

                var timer = new CountdownTimer(scheduler);
                timer.Configure(totalMilliseconds, updateIntervalMilliseconds);

                timer.WhenAnyValue(vm => vm.RemainingMilliseconds)
                    .Subscribe(value => actualNotifications.Add(value));

                timer.TotalMilliseconds.Should().Be(totalMilliseconds);
                timer.TotalTimeSpan.Should().BeCloseTo(TimeSpan.FromMilliseconds(totalMilliseconds), TimeSpan.FromMilliseconds(1));
                timer.IsRunning.Should().BeFalse();

                timer.Start(skipMilliseconds);

                timer.IsRunning.Should().BeTrue();

                scheduler.AdvanceBy(TimeSpan.FromMilliseconds(totalMilliseconds * 2).Ticks);

                timer.RemainingMilliseconds.Should().Be(0);
                timer.RemainingTimeSpan.Should().Be(TimeSpan.FromMilliseconds(0));
                timer.IsRunning.Should().BeFalse();

                expectedNotifications.Should().ContainInOrder(actualNotifications);
            }

            // More Start() tests here

        }


        // Stop() tests





        // Group observable subscription checks together

        [Fact]
        public void Should_Notify_IsRunning()
        {
            var scheduler = new TestScheduler();

            var timer = new CountdownTimer();
            timer.Configure(10000, 1000);

            var isRunning = false;

            timer.WhenAnyValue(vm => vm.IsRunning)
                .Subscribe(value =>
                {
                    isRunning = value;
                });

            isRunning.Should().BeFalse();

            timer.Start();

            isRunning.Should().BeTrue();
        }



    }
}