using Microsoft.CodeAnalysis;

namespace Ravitor;

internal sealed record RavitorOptions
{
    public bool HandlersClassPublic { get; private set; }

    public bool ServicesClassPublic { get; private set; }

    public string? ServicesClassName { get; private set; }

    public string? ServicesMethodName { get; private set; }

    //public bool ServicesRegisterWrapper { get; private set; }

    public bool DisableInterceptor { get; private set; }

    public void SetValue(string key, TypedConstant value)
    {
        switch (key)
        {
            case nameof(HandlersClassPublic):
                HandlersClassPublic = value.Value is bool handlersClassPublic && handlersClassPublic;
                break;
            case nameof(ServicesClassPublic):
                ServicesClassPublic = value.Value is bool servicesClassPublic && servicesClassPublic;
                break;
            case nameof(ServicesClassName):
                ServicesClassName = value.Value as string;
                break;
            case nameof(ServicesMethodName):
                ServicesMethodName = value.Value as string;
                break;
            //case nameof(ServicesRegisterWrapper):
            //    ServicesRegisterWrapper = value.Value is bool servicesRegisterWrapper && servicesRegisterWrapper;
            //    break;
            case nameof(DisableInterceptor):
                DisableInterceptor = value.Value is bool disableInterceptor && disableInterceptor;
                break;
        }
    }

    public RavitorOptions WithDisableInterceptor(bool disableInterceptor)
    {
        DisableInterceptor = DisableInterceptor || disableInterceptor;
        return this;
    }
};
