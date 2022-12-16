using AllOverIt.Aspects.Interceptor;
using BenchmarkDotNet.Attributes;
using System.Threading.Tasks;

namespace InterceptorBenchmarking
{
    /*
    |                          Method |        Mean |     Error |    StdDev |   Gen0 | Allocated |
    |-------------------------------- |------------:|----------:|----------:|-------:|----------:|
    |          Call_Service_GetSecret |   0.8919 ns | 0.1043 ns | 0.2748 ns |      - |         - |
    |     Call_Service_GetSecretAsync |  19.5539 ns | 0.3875 ns | 0.6987 ns | 0.0115 |      72 B |
    |      Call_Interceptor_GetSecret |  43.1708 ns | 0.8595 ns | 1.0232 ns | 0.0153 |      96 B |
    | Call_Interceptor_GetSecretAsync | 193.2915 ns | 3.8214 ns | 5.6014 ns | 0.0598 |     376 B |
     */

    [MemoryDiagnoser]
    public class BenchmarkTests
    {
        private readonly IService _service = new Service();
        private readonly IService _serviceInterceptor = InterceptorFactory.CreateProxy<IService, ServiceInterceptor>(new Service(), interceptor => { });

        [Benchmark]
        public void Call_Service_GetSecret()
        {
            _service.GetSecret();
        }

        [Benchmark]
        public async Task Call_Service_GetSecretAsync()
        {
            await _service.GetSecretAsync(true);
        }

        [Benchmark]
        public void Call_Interceptor_GetSecret()
        {
            _serviceInterceptor.GetSecret();
        }

        [Benchmark]
        public async Task Call_Interceptor_GetSecretAsync()
        {
            await _serviceInterceptor.GetSecretAsync(true);
        }
    }
}