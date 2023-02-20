using AllOverIt.Validation.Extensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AllOverIt.Validation.Options.Extensions
{
    public static class OptionsBuilderExtensions
    {
        public static OptionsBuilder<TOptions> UseFluentValidation<TOptions>(this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
        {
            optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(provider =>
            {
                var validator = provider.GetRequiredService<IValidator<TOptions>>();

                return new FluentValidationValidateOptions<TOptions>(optionsBuilder.Name, validator);
            });

            return optionsBuilder;
        }
    }
}