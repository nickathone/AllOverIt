using System;

namespace AllOverIt.Bindings
{
  [Flags]
#pragma warning disable RCS1135 // Declare enum member with zero value (when enum has FlagsAttribute).
#pragma warning disable RCS1154 // Sort enum members.
  public enum BindingOptions
#pragma warning restore RCS1154 // Sort enum members.
#pragma warning restore RCS1135 // Declare enum member with zero value (when enum has FlagsAttribute).
  {
    Static = 1,
    Instance = 2,

    Abstract = 4,
    Virtual = 8,
    NonVirtual = 16,

    Internal = 32,
    Private = 64,
    Protected = 128,
    Public = 256,

    DefaultScope = Static | Instance,
    DefaultAccessor = Abstract | Virtual | NonVirtual,
    DefaultVisibility = Public | Protected,

    AllScope = DefaultScope,
    AllAccessor = DefaultAccessor,
    AllVisibility = DefaultVisibility | Private | Internal,

    Default = DefaultScope | DefaultAccessor | DefaultVisibility,
    All = AllScope | AllAccessor | AllVisibility
  }
}