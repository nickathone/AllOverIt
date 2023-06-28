using Amazon.CDK.AWS.AppSync;
using Cdklabs.AwsCdkAppsyncUtils;

namespace AllOverIt.Aws.Cdk.AppSync
{
    /// <summary>Contains options for an AppSync GraphQL API with the Schema pre-configured as a <see cref="CodeFirstSchema"/>.</summary>
    public sealed class AppGraphqlProps : GraphqlApiProps
    {
        /// <summary>Constructor.</summary>
        public AppGraphqlProps()
        {
            Schema = new CodeFirstSchema();
        }

        internal CodeFirstSchema GetCodeFirstSchema()
        {
            return Schema as CodeFirstSchema;
        }
    }
}