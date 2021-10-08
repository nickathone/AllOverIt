namespace AllOverIt.Evaluator.Variables
{
    /// <summary>A delegate based variable that is evaluated the first time the <see cref="IVariable.Value"/> is read.</summary>
    public interface ILazyVariable : IVariable
    {
        /// <summary>Resets the variable to force its value to be re-evaluated when <see cref="IVariable.Value"/> is next read.</summary>
        void Reset();
    }
}