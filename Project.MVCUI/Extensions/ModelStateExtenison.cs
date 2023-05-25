using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Project.MVCUI.Extensions
{
    public static class ModelStateExtenison
    {
        public static void AddModelErrorListWithOutKey(this ModelStateDictionary modelState, IEnumerable<IdentityError> errors)
        {
            foreach (IdentityError error in errors)
            {
                modelState.AddModelError(string.Empty, error.Description);
            }
        }

        public static void AddModelErrorWithOutKey(this ModelStateDictionary modelState, string error)
        {
            modelState.AddModelError(string.Empty, error);
        }
    }
}
