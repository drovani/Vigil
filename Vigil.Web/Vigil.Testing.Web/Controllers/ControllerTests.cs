using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Vigil.Testing.Web.Controllers
{
    public abstract class ControllerTests
    {
        protected bool BindModel<TController, TModel>(TController ctrl, TModel model) where TController : Controller
        {
            var modelBinder = new ModelBindingContext()
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, typeof(TModel)),
                ValueProvider = new NameValueCollectionValueProvider(new NameValueCollection(), CultureInfo.InvariantCulture)
            };
            var binder = new DefaultModelBinder().BindModel(new ControllerContext(), modelBinder);
            ctrl.ModelState.Clear();
            ctrl.ModelState.Merge(modelBinder.ModelState);

            return ctrl.ModelState.IsValid;
        }
    }
}
