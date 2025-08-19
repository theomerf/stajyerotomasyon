using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System.IO;
using System.Threading.Tasks;

public static class ViewComponentHelperExtensions
{
    public static async Task<string> RenderViewComponentAsync(this HttpContext httpContext, string componentName, object? arguments = null)
    {
        var serviceProvider = httpContext.RequestServices;

        var actionContext = new ActionContext(
            httpContext,
            httpContext.GetRouteData() ?? new RouteData(),
            new ActionDescriptor()
        );

        using var sw = new StringWriter();

        var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
        var tempDataFactory = serviceProvider.GetRequiredService<ITempDataDictionaryFactory>();
        var tempData = tempDataFactory.GetTempData(httpContext);

        var viewContext = new ViewContext(
            actionContext,
            NullView.Instance,
            viewData,
            tempData,
            sw,
            new HtmlHelperOptions()
        );

        var vcHelper = serviceProvider.GetRequiredService<IViewComponentHelper>();
        (vcHelper as IViewContextAware)!.Contextualize(viewContext);

        var result = await vcHelper.InvokeAsync(componentName, arguments);

        result.WriteTo(sw, System.Text.Encodings.Web.HtmlEncoder.Default);

        return sw.ToString();
    }

    private class NullView : IView
    {
        public static readonly NullView Instance = new NullView();
        public string Path => string.Empty;
        public Task RenderAsync(ViewContext context) => Task.CompletedTask;
    }
}
