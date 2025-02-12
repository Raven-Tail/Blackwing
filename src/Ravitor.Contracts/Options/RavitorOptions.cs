namespace Ravitor.Contracts.Options;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
public sealed class RavitorOptionsAttribute : Attribute
{
    //public bool HandlersClassPublic { get; set; }

    /// <summary>
    /// Allows you to define if the service collection extension class
    /// should be public or internal (defaults to internal).
    /// </summary>
    public bool ServicesClassPublic { get; set; }

    /// <summary>
    /// Allows you to define the service collection extension class
    /// name (defaults to RavitorServiceCollectionExtensions).
    /// </summary>
    public string? ServicesClassName { get; set; }

    /// <summary>
    /// Allows you to define the service collection extension method
    /// name (defaults to AddRavitorHandlers).
    /// </summary>
    public string? ServicesMethodName { get; set; }

    //public bool ServicesRegisterWrapper { get; set;}

    //public bool DisableInterceptor { get; set; }
}
