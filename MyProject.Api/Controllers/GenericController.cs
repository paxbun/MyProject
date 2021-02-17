using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using MyProject.Core;
using MyProject.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyProject.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [GenericController]
    public class GenericController<TRequest, TResult> : ControllerBase
        where TRequest : ICoreRequestBase<TResult>
        where TResult : IResultBase
    {
        private readonly IMediator _mediator;
        public GenericController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Handle(TRequest request)
        {
            request.GetUserIdentity(this);
            var result = await _mediator.Send(request);
            return result.ToActionResult(this);
        }
    }

    internal static class GenericControllerNameHelpers
    {
        private static readonly Regex _requestNameRegex = new Regex(@"(.+)(?:Query|Command)");

        public static string GetRequestName(string requestTypename)
            => _requestNameRegex.Match(requestTypename).Groups[1].Value;

        public static string GetCamelCasedName(string pascalCasedName)
            => char.ToLower(pascalCasedName[0]) + pascalCasedName[1..];
    }

    public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var requestTypes = CoreRequestHelpers.GetTypesWithGenericInterface(typeof(ICoreRequestBase<>));
            foreach (var (requestType, interfaceType) in requestTypes)
            {
                var resultType = interfaceType.GenericTypeArguments[0];
                var pascalCasedName = GenericControllerNameHelpers.GetRequestName(requestType.Name);
                var controllerName = pascalCasedName + "Controller";

                if (feature.Controllers.Where(controller => controller.Name == controllerName).Any())
                    continue;

                var controllerType = typeof(GenericController<,>).MakeGenericType(requestType, resultType);
                feature.Controllers.Add(controllerType.GetTypeInfo());
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GenericControllerAttribute : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controllerModel)
        {
            var controllerType = controllerModel.ControllerType;
            if (controllerType.IsGenericType
                && controllerType.GetGenericTypeDefinition() == typeof(GenericController<,>))
            {
                var requestType = controllerType.GenericTypeArguments[0];

                var forAttribute = (ForAttribute)GetCustomAttribute(requestType, typeof(ForAttribute));
                var actionModel = controllerModel.Actions.FirstOrDefault();
                if (actionModel != null)
                {
                    var selector = actionModel.Selectors.FirstOrDefault();
                    if (forAttribute.Types.Any())
                        selector.EndpointMetadata.Add(new AuthorizeAttribute());
                    if (forAttribute.AllowAnonymous)
                        selector.EndpointMetadata.Add(new AllowAnonymousAttribute());
                }

                var pascalCasedName = GenericControllerNameHelpers.GetRequestName(requestType.Name);
                var camelCasedName = GenericControllerNameHelpers.GetCamelCasedName(pascalCasedName);
                controllerModel.Selectors.FirstOrDefault()
                    .AttributeRouteModel.Template = $"api/{camelCasedName}";
            }
        }
    }
}
