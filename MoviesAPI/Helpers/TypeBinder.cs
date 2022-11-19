using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace MoviesAPI.Helpers
{
    public class TypeBinder<T>:IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var properyName = bindingContext.ModelName;
            var valueProvider = bindingContext.ValueProvider.GetValue(properyName);
            if (valueProvider == ValueProviderResult.None) return Task.CompletedTask;
            try
            {
                var deserializedValue = JsonConvert.DeserializeObject<T>(valueProvider.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(deserializedValue);
            }
            catch{
                bindingContext.ModelState.TryAddModelError(properyName, "Invalid format");
            }
            return Task.CompletedTask;
        }
    }
}
