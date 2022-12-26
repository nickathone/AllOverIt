using AllOverIt.DependencyInjection.Extensions;
using InterceptorDemo.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace InterceptorDemo
{
    internal class Program
    {
        static async Task Main()
        {
            try
            {
                var services = new ServiceCollection();

                services
                    .AddScoped<ISecretService, SecretService>()
                    .DecorateWithInterceptor<ISecretService, TimedInterceptor>(inteceptor =>
                    {
                        inteceptor.MinimimReportableMilliseconds = 1000;
                    });

                var serviceProvider = services.BuildServiceProvider();

                var proxy = serviceProvider.GetRequiredService<ISecretService>();

                var secret = proxy.GetSecret();
                Console.WriteLine(secret);                          // should be reported as 0-1ms

                // Adding this to make sure this time is not included in the time period reported by the proxy
                await Task.Delay(2000);

                Console.WriteLine();

                secret = await proxy.GetSecretAsync(false);
                Console.WriteLine(secret);                          // should be reported as approx. 1000ms

                Console.WriteLine();

                secret = await proxy.GetSecretAsync(true);          // will throw
            }
            catch (Exception exception)
            {
                Console.WriteLine($"CAUGHT: {exception.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}