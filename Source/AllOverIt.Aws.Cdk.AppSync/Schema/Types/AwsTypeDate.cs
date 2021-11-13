﻿using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;

namespace AllOverIt.Aws.Cdk.AppSync.Schema.Types
{
    /// <summary>A custom scalar type that will be interpreted as an AwsDate type.</summary>
    [SchemaScalar(nameof(AwsTypeDate))]
    public sealed class AwsTypeDate
    {
    }
}