using Infra.enums;
using Infra.ResultWrapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Soccer.Filters
{
    public class TranslateResultToActionResultAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                var type = objectResult.Value.GetType();
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<>))
                {
                    dynamic result = objectResult.Value;
                    if (!result.IsSuccess)
                    {
                        var errorType = (ErrorType)result.ErrorType;
                        int statusCode = errorType switch
                        {
                            ErrorType.NotFound => 404,
                            ErrorType.BadRequest => 400,
                            ErrorType.UnAuthorized => 401,
                            ErrorType.Validation => 422,
                            ErrorType.Conflict => 409,
                            ErrorType.InternalServerError => 500,
                            _ => 400
                        };

                        context.Result = new ObjectResult(result) { StatusCode = statusCode };
                    }
                    // If Success, the default ObjectResult (200 OK) is fine, which contains the Result object.
                }
            }
            base.OnActionExecuted(context);
        }
    }
}
