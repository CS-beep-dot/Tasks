using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskManagementApi.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {

        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null) continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator == null) continue;

                var result = await validator.ValidateAsync(new ValidationContext<object>(argument));

                if (!result.IsValid)
                {
                    foreach (var error in result.Errors)
                        context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                    return;
                }
                if (!context.ModelState.IsValid)
                {
                    var controller = (ControllerBase)context.Controller;
                    context.Result = controller.ValidationProblem(context.ModelState);
                    return;
                }
            }

            await next();
        }
    }
}
