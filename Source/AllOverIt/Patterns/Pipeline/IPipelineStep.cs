namespace AllOverIt.Patterns.Pipeline
{
    public interface IPipelineStep<TIn, TOut>
    {
        TOut Execute(TIn input);
    }
}
