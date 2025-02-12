using Microsoft.JSInterop;
using System.Threading.Tasks;

public class ToasterService
{
    private readonly IJSRuntime _jsRuntime;

    public ToasterService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task ShowToast(string message, string type = "info")
    {
        await _jsRuntime.InvokeVoidAsync("showToast", message, type);
    }
}
