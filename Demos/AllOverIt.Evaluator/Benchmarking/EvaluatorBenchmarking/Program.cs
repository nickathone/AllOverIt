using BenchmarkDotNet.Running;

namespace EvaluatorBenchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Evaluator>();
        }
    }
}
