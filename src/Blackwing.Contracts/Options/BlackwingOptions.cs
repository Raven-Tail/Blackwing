namespace Blackwing.Contracts.Options;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
public sealed class BlackwingOptionsAttribute : Attribute
{
    /// <summary>
    /// Allows you to define if the handler wrapper class
    /// should be public or internal (defaults to internal).
    /// </summary>
    public bool HandlersClassPublic { get; set; }

    /// <summary>
    /// Allows you to define the namespace of the service collection
    /// extension class (defaults to Microsoft.Extensions.DependencyInjection).
    /// </summary>
    public string? ServicesNamespace { get; set; }

    /// <summary>
    /// Allows you to define if the service collection extension class
    /// should be public or internal (defaults to internal).
    /// </summary>
    public bool ServicesClassPublic { get; set; }

    /// <summary>
    /// Allows you to define the service collection extension class
    /// name (defaults to BlackwingServiceCollectionExtensions).
    /// </summary>
    public string? ServicesClassName { get; set; }

    /// <summary>
    /// Allows you to define the service collection extension method
    /// name (defaults to AddBlackwingHandlers).
    /// </summary>
    public string? ServicesMethodName { get; set; }

    //public bool ServicesRegisterWrapper { get; set;}

    /// <summary>
    /// Allows you to disable the interceptors generation (defaults to false).
    /// </summary>
    /// <remarks>
    /// You can also disable the interceptors generation by setting the 'DisableBlackwingInterceptor' MSBuild Property to 'true'.
    /// </remarks>
    public bool DisableInterceptor { get; set; }
}
