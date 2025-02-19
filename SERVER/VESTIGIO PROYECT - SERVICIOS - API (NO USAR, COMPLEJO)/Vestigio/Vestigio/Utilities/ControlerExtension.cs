using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Vestigio.Utilities
{
    public static class ControllerExtensions
    {
        public static async Task<string> RenderViewToStringAsync<TModel>(
        this Controller controller,
        string viewPath,
        TModel model)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewEngine = controller.HttpContext.RequestServices
                    .GetRequiredService<ICompositeViewEngine>();

                var viewResult = viewEngine.GetView(
                    executingFilePath: null,
                    viewPath: viewPath,
                    isMainPage: false);

                if (!viewResult.Success)
                {
                    // Buscar en carpetas alternativas si falla
                    viewResult = viewEngine.FindView(controller.ControllerContext, viewPath, false);
                }

                if (!viewResult.Success)
                {
                    throw new InvalidOperationException($"Vista no encontrada: {viewPath}");
                }

                var viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}
