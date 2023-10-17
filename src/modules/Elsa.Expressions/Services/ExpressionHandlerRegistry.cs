using Elsa.Expressions.Contracts;
using Elsa.Expressions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Elsa.Expressions.Services;

public class ExpressionHandlerRegistry : IExpressionHandlerRegistry
{
    private readonly IServiceProvider _serviceProvider;

    public ExpressionHandlerRegistry(IOptions<ExpressionOptions> options, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Dictionary = new Dictionary<Type, Type>(options.Value.ExpressionHandlers);
    }

    private IDictionary<Type, Type> Dictionary { get; }

    public void Register(Type expressionType, Type handler) => Dictionary.Add(expressionType, handler);

    public IExpressionHandler? GetHandler(IExpression expression)
    {
        var expressionType = expression.GetType();

        if (expressionType.IsConstructedGenericType)
            expressionType = expressionType.BaseType;

        if (!Dictionary.TryGetValue(expressionType!, out var handlerType))
            return null;

        return (IExpressionHandler)ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, handlerType);
    }
}