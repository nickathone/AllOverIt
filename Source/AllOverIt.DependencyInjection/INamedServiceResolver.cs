namespace AllOverIt.DependencyInjection
{
    public interface INamedServiceResolver<TService>
    {
        TService GetRequiredNamedService(string name);
    }

    // An additional option that could be used to resolve multiple interface types
    public interface INamedServiceResolver
    {
        TService GetRequiredNamedService<TService>(string name);
    }
}