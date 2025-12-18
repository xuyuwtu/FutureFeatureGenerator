namespace System;

internal enum FutureCSharpLanguageVersion
{
    /// <summary>
    /// C# language version 1
    /// </summary>
    CSharp1 = 1,

    /// <summary>
    /// C# language version 2
    /// </summary>
    CSharp2 = 2,

    /// <summary>
    /// C# language version 3
    /// <para>
    /// Features: LINQ.
    /// </para>
    /// </summary>
    CSharp3 = 3,

    /// <summary>
    /// C# language version 4
    /// <para>
    /// Features: dynamic.
    /// </para>        
    /// </summary>        
    CSharp4 = 4,

    /// <summary>
    /// C# language version 5
    /// <para>
    /// Features: async, caller info attributes.
    /// </para>        
    /// </summary> 
    CSharp5 = 5,

    /// <summary>
    /// C# language version 6
    /// <para>Features:</para>
    /// <list type="bullet">
    /// <item><description>Using of a static class</description></item>
    /// <item><description>Exception filters</description></item>
    /// <item><description>Await in catch/finally blocks</description></item>
    /// <item><description>Auto-property initializers</description></item>
    /// <item><description>Expression-bodied methods and properties</description></item>
    /// <item><description>Null-propagating operator ?.</description></item>
    /// <item><description>String interpolation</description></item>
    /// <item><description>nameof operator</description></item>
    /// <item><description>Dictionary initializer</description></item>
    /// </list>
    /// </summary>
    CSharp6 = 6,

    /// <summary>
    /// C# language version 7.0
    /// <para>Features:</para>
    /// <list type="bullet">
    /// <item><description>Out variables</description></item>
    /// <item><description>Pattern-matching</description></item>
    /// <item><description>Tuples</description></item>
    /// <item><description>Deconstruction</description></item>
    /// <item><description>Discards</description></item>
    /// <item><description>Local functions</description></item>
    /// <item><description>Digit separators</description></item>
    /// <item><description>Ref returns and locals</description></item>
    /// <item><description>Generalized async return types</description></item>
    /// <item><description>More expression-bodied members</description></item>
    /// <item><description>Throw expressions</description></item>
    /// </list>
    /// </summary>
    CSharp7 = 7,

    /// <summary>
    /// C# language version 7.1
    /// <para>Features:</para>
    /// <list type="bullet">
    /// <item><description>Async Main</description></item>
    /// <item><description>Default literal</description></item>
    /// <item><description>Inferred tuple element names</description></item>
    /// <item><description>Pattern-matching with generics</description></item>
    /// </list>
    /// </summary>
    CSharp7_1 = 701,

    /// <summary>
    /// C# language version 7.2
    /// <para>Features:</para>
    /// <list type="bullet">
    /// <item><description>Ref readonly</description></item>
    /// <item><description>Ref and readonly structs</description></item>
    /// <item><description>Ref extensions</description></item>
    /// <item><description>Conditional ref operator</description></item>
    /// <item><description>Private protected</description></item>
    /// <item><description>Digit separators after base specifier</description></item>
    /// <item><description>Non-trailing named arguments</description></item>
    /// </list>
    /// </summary>
    CSharp7_2 = 702,

    /// <summary>
    /// C# language version 7.3
    /// <para>Features:</para>
    /// <list type="bullet">
    /// <item><description>Indexing fixed fields does not require pinning</description></item>
    /// <item><description>ref local variables may be reassigned</description></item>
    /// <item><description>stackalloc arrays support initializers</description></item>
    /// <item><description>More types support the fixed statement</description></item>
    /// <item><description>Enhanced generic constraints</description></item>
    /// <item><description>Tuples support == and !=</description></item>
    /// <item><description>Attach attributes to the backing fields for auto-implemented properties</description></item>
    /// <item><description>Method overload resolution improvements when arguments differ by 'in'</description></item>
    /// <item><description>Extend expression variables in initializers</description></item>
    /// <item><description>Improved overload candidates</description></item>
    /// <item><description>New compiler options (-publicsign and -pathmap)</description></item>
    /// </list>
    /// </summary>
    CSharp7_3 = 703,

    /// <summary>
    /// C# language version 8.0
    /// <para>Features:</para>
    /// <list type="bullet">
    /// <item><description>Readonly members</description></item>
    /// <item><description>Default interface methods</description></item>
    /// <item><description>Pattern matching enhancements (switch expressions, property patterns, tuple patterns, and positional patterns)</description></item>
    /// <item><description>Using declarations</description></item>
    /// <item><description>Static local functions</description></item>
    /// <item><description>Disposable ref structs</description></item>
    /// <item><description>Nullable reference types</description></item>
    /// <item><description>Asynchronous streams</description></item>
    /// <item><description>Asynchronous disposable</description></item>
    /// <item><description>Indices and ranges</description></item>
    /// <item><description>Null-coalescing assignment</description></item>
    /// <item><description>Unmanaged constructed types</description></item>
    /// <item><description>Stackalloc in nested expressions</description></item>
    /// <item><description>Enhancement of interpolated verbatim strings</description></item>
    /// </list>
    /// </summary>
    CSharp8 = 800,

    /// <summary>
    /// C# language version 9.0
    /// <para>Features:</para>
    /// <list type="bullet">
    /// <item><description>Records</description></item>
    /// <item><description>Init only setters</description></item>
    /// <item><description>Top-level statements</description></item>
    /// <item><description>Pattern matching enhancements</description></item>
    /// <item><description>Native sized integers</description></item>
    /// <item><description>Function pointers</description></item>
    /// <item><description>Suppress emitting localsinit flag</description></item>
    /// <item><description>Target-typed new expressions</description></item>
    /// <item><description>Static anonymous functions</description></item>
    /// <item><description>Target-typed conditional expressions</description></item>
    /// <item><description>Covariant return types</description></item>
    /// <item><description>Extension GetEnumerator support for foreach loops</description></item>
    /// <item><description>Lambda discard parameters</description></item>
    /// <item><description>Attributes on local functions</description></item>
    /// <item><description>Module initializers</description></item>
    /// <item><description>New features for partial methods</description></item>
    /// </list>
    /// </summary>
    CSharp9 = 900,

    /// <summary>
    /// C# language version 10.0
    /// <para>Features:</para>
    /// <list type="bullet">
    /// <item><description>Record structs</description></item>
    /// <item><description>Global using directives</description></item>
    /// <item><description>Lambda improvements</description></item>
    /// <item><description>Improved definite assignment</description></item>
    /// <item><description>Constant interpolated strings</description></item>
    /// <item><description>Mix declarations and variables in deconstruction</description></item>
    /// <item><description>Extended property patterns</description></item>
    /// <item><description>Sealed record ToString</description></item>
    /// <item><description>Source Generator v2 APIs</description></item>
    /// <item><description>Method-level AsyncMethodBuilder</description></item>
    /// </list>
    /// </summary>
    CSharp10 = 1000,

    /// <summary>
    /// C# language version 11.0
    /// <br/>
    /// Features:
    /// <list type="bullet">
    /// <item><description>Raw string literals</description></item>
    /// <item><description>Static abstract members in interfaces</description></item>
    /// <item><description>Generic attributes</description></item>
    /// <item><description>Newlines in interpolations</description></item>
    /// <item><description>List-patterns</description></item>
    /// <item><description>Required members</description></item>
    /// <item><description>Span&lt;char> constant pattern</description></item>
    /// <item><description>Struct auto-default</description></item>
    /// <item><description>Nameof(parameter)</description></item>
    /// <item><description>Checked user-defined operators</description></item>
    /// <item><description>UTF-8 string literals</description></item>
    /// <item><description>Unsigned right-shift operator</description></item>
    /// <item><description>Relaxed shift operator</description></item>
    /// <item><description>Ref fields</description></item>
    /// <item><description>File-local types</description></item>
    /// </list> 
    /// </summary>
    CSharp11 = 1100,

    /// <summary>
    /// C# language version 12.0
    /// <br/>
    /// Features:
    /// <list type="bullet">
    /// <item><description>Primary constructors</description></item>
    /// <item><description>Using aliases for any types</description></item>
    /// <item><description>Nameof accessing instance members</description></item>
    /// <item><description>Inline arrays</description></item>
    /// <item><description>Collection expressions</description></item>
    /// <item><description>Ref readonly parameters</description></item>
    /// <item><description>Lambda optional parameters</description></item>
    /// </list>
    /// </summary>
    CSharp12 = 1200,

    /// <summary>
    /// C# language version 13.0
    /// <br/>
    /// Features:
    /// <list type="bullet">
    /// <item><description>Escape character</description></item>
    /// <item><description>Method group natural type improvements</description></item>
    /// <item><description>`Lock` object</description></item>
    /// <item><description>Implicit indexer access in object initializers</description></item>
    /// <item><description>`params` collections</description></item>
    /// <item><description>ref/unsafe in iterators/async</description></item>
    /// <item><description>`allows ref struct` constraint</description></item>
    /// <item><description>Partial properties</description></item>
    /// </list>
    /// </summary>
    CSharp13 = 1300,

    /// <summary>
    /// C# language version 14.0
    /// <br/>
    /// Features:
    /// <list type="bullet">
    /// <item><description>`field` keyword in properties</description></item>
    /// <item><description>First-class `Span` types</description></item>
    /// <item><description>Unbound generic types in `nameof`</description></item>
    /// <item><description>Simple lambda parameters with modifiers</description></item>
    /// <item><description>Partial events and constructors</description></item>
    /// <item><description>Extension methods, properties and operators</description></item>
    /// <item><description>Null-conditional assignment</description></item>
    /// <item><description>Ignored directives</description></item>
    /// <item><description>User-defined compound assignment operators</description></item>
    /// <item><description>Optional and named arguments in `Expression` trees</description></item>
    /// </list>
    /// </summary>
    CSharp14 = 1400,

    /// <summary>
    /// The latest major supported version.
    /// </summary>
    LatestMajor = int.MaxValue - 2,

    /// <summary>
    /// Preview of the next language version.
    /// </summary>
    Preview = int.MaxValue - 1,

    /// <summary>
    /// The latest supported version of the language.
    /// </summary>
    Latest = int.MaxValue,

    /// <summary>
    /// The default language version, which is the latest supported version.
    /// </summary>
    Default = 0,
}
