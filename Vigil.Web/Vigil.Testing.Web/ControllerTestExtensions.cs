using System;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Web.Mvc;

namespace Vigil.Web.Controllers
{
    public static class ControllerTestExtensions
    {
        public static bool BindModel<TController, TModel>(this TController controller, TModel model) where TController : Controller
        {
            Contract.Requires<ArgumentNullException>(controller != null);
            Contract.Requires<ArgumentNullException>(model != null);

            Contract.Assume(ModelMetadataProviders.Current != null);
            Contract.Assume(controller.ModelState != null);

            var modelBinder = new ModelBindingContext()
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, typeof(TModel)),
                ValueProvider = new NameValueCollectionValueProvider(new NameValueCollection(), CultureInfo.InvariantCulture)
            };
            var binder = new DefaultModelBinder();
            binder.BindModel(new ControllerContext(), modelBinder);
            controller.ModelState.Clear();
            controller.ModelState.Merge(modelBinder.ModelState);

            return controller.ModelState.IsValid;
        }
    }
}