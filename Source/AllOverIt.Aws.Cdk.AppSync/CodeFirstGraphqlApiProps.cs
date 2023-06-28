using Amazon.CDK.AWS.AppSync;
using Cdklabs.AwsCdkAppsyncUtils;

namespace AllOverIt.Aws.Cdk.AppSync
{
    /// <summary>Contains options for an AppSync GraphQL API with the Schema pre-configured as a <see cref="CodeFirstSchema"/>.</summary>
    public sealed class CodeFirstGraphqlApiProps : GraphqlApiProps
    {
        /// <summary>Constructor.</summary>
        public CodeFirstGraphqlApiProps()
        {
            Schema = new CodeFirstSchema();
        }

        internal CodeFirstSchema GetCodeFirstSchema()
        {
            return Schema as CodeFirstSchema;
        }
    }
}