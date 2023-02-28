using Microsoft.Extensions.Localization;
using Template.Application.Resources;
using Template.Core.Interfaces;

namespace Template.Application.Services;

public class Globalizer : IGlobalizer
{
    private readonly IStringLocalizer _strings;
    public Globalizer(IStringLocalizer<SharedResource> strings)
    {
        _strings = strings;
    }
    public string this[string key, params string[] args] => _strings[key, args].Value;
}
