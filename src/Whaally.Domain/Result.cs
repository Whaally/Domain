using FluentResults;

namespace Whaally.Domain;

public partial class Result<TSelf> where TSelf : Result<TSelf>, new()
{
    static Result()
    {
        Settings = new ResultSettingsBuilder().Build();
    }

    internal static ResultSettings Settings { get; private set; }

    /// <summary>
    ///     Setup global settings like logging
    /// </summary>
    public static void Setup(Action<ResultSettingsBuilder> setupFunc)
    {
        var settingsBuilder = new ResultSettingsBuilder();
        setupFunc(settingsBuilder);

        Settings = settingsBuilder.Build();
    }

    /// <summary>
    ///     Creates a success result
    /// </summary>
    public static TSelf Ok()
    {
        return new TSelf();
    }

    /// <summary>
    ///     Creates a failed result with the given error
    /// </summary>
    public static TSelf Fail(IError error)
    {
        var result = new TSelf();
        result.WithError(error);
        return result;
    }

    /// <summary>
    ///     Creates a failed result with the given error message. Internally an error object from the error factory is created.
    /// </summary>
    public static TSelf Fail(string errorMessage)
    {
        var result = new TSelf();
        result.WithError(Settings.ErrorFactory(errorMessage));
        return result;
    }

    /// <summary>
    ///     Creates a failed result with the given error messages. Internally a list of error objects from the error factory is
    ///     created
    /// </summary>
    public static TSelf Fail(IEnumerable<string> errorMessages)
    {
        if (errorMessages == null)
            throw new ArgumentNullException(nameof(errorMessages), "The list of error messages cannot be null");

        if (!errorMessages.Any())
            throw new ArgumentException("The list of errors is empty", nameof(errorMessages));

        var result = new TSelf();
        result.WithErrors(errorMessages.Select(Settings.ErrorFactory));
        return result;
    }

    /// <summary>
    ///     Creates a failed result with the given errors.
    /// </summary>
    public static TSelf Fail(IEnumerable<IError> errors)
    {
        if (errors == null)
            throw new ArgumentNullException(nameof(errors), "The list of errors cannot be null");

        if (!errors.Any())
            throw new ArgumentException("The list of errors is empty", nameof(errors));


        var result = new TSelf();
        result.WithErrors(errors);
        return result;
    }

    /// <summary>
    ///     Create a success/failed result depending on the parameter isSuccess
    /// </summary>
    public static TSelf OkIf(bool isSuccess, IError error)
    {
        return isSuccess ? Ok() : Fail(error);
    }

    /// <summary>
    ///     Create a success/failed result depending on the parameter isSuccess
    /// </summary>
    public static TSelf OkIf(bool isSuccess, string error)
    {
        return isSuccess ? Ok() : Fail(error);
    }

    /// <summary>
    ///     Create a success/failed result depending on the parameter isSuccess
    /// </summary>
    /// <remarks>
    ///     Error is lazily evaluated.
    /// </remarks>
    public static TSelf OkIf(bool isSuccess, Func<IError> errorFactory)
    {
        return isSuccess ? Ok() : Fail(errorFactory.Invoke());
    }

    /// <summary>
    ///     Create a success/failed result depending on the parameter isSuccess
    /// </summary>
    /// <remarks>
    ///     Error is lazily evaluated.
    /// </remarks>
    public static TSelf OkIf(bool isSuccess, Func<string> errorMessageFactory)
    {
        return isSuccess ? Ok() : Fail(errorMessageFactory.Invoke());
    }

    /// <summary>
    ///     Create a success/failed result depending on the parameter isFailure
    /// </summary>
    public static TSelf FailIf(bool isFailure, IError error)
    {
        return isFailure ? Fail(error) : Ok();
    }

    /// <summary>
    ///     Create a success/failed result depending on the parameter isFailure
    /// </summary>
    public static TSelf FailIf(bool isFailure, string error)
    {
        return isFailure ? Fail(error) : Ok();
    }

    /// <summary>
    ///     Create a success/failed result depending on the parameter isFailure
    /// </summary>
    /// <remarks>
    ///     Error is lazily evaluated.
    /// </remarks>
    public static TSelf FailIf(bool isFailure, Func<IError> errorFactory)
    {
        return isFailure ? Fail(errorFactory.Invoke()) : Ok();
    }

    /// <summary>
    ///     Create a success/failed result depending on the parameter isFailure
    /// </summary>
    /// <remarks>
    ///     Error is lazily evaluated.
    /// </remarks>
    public static TSelf FailIf(bool isFailure, Func<string> errorMessageFactory)
    {
        return isFailure ? Fail(errorMessageFactory.Invoke()) : Ok();
    }

    /// <summary>
    ///     Create a success/failed result depending on the parameter isFailure containing the specified errors
    /// </summary>
    public static TSelf FailIf(bool isFailure, IEnumerable<IError> errors)
    {
        return isFailure ? Fail(errors) : Ok();
    }

    /// <summary>
    ///     Create a success/failed result if any error objects exist
    /// </summary>
    public static TSelf FailIfNotEmpty(IEnumerable<IError> errors)
    {
        return errors.Any() ? Fail(errors) : Ok();
    }

    /// <summary>
    ///     Create a success/failed result depending if any error objects exist
    /// </summary>
    /// <remarks>
    ///     Error is lazily evaluated.
    /// </remarks>
    public static TSelf FailIfNotEmpty<T>(IEnumerable<T> errors, Func<T, IError> func)
    {
        return errors.Any() ? Fail(errors.Select(error => func(error))) : Ok();
    }

    /// <summary>
    ///     Executes the action. If an exception is thrown within the action then this exception is transformed via the
    ///     catchHandler to an Error object
    /// </summary>
    public static TSelf Try(Action action, Func<Exception, IError>? catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            action();
            return Ok();
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }

    /// <summary>
    ///     Executes the action. If an exception is thrown within the action then this exception is transformed via the
    ///     catchHandler to an Error object
    /// </summary>
    public static async Task<TSelf> Try(Func<Task> action, Func<Exception, IError>? catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            await action();
            return Ok();
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }

    /// <summary>
    ///     Executes the action. If an exception is thrown within the action then this exception is transformed via the
    ///     catchHandler to an Error object
    /// </summary>
    public static async ValueTask<TSelf> Try(Func<ValueTask> action, Func<Exception, IError>? catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            await action();
            return Ok();
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }

    /// <summary>
    ///     Executes the action. If an exception is thrown within the action then this exception is transformed via the
    ///     catchHandler to an Error object
    /// </summary>
    public static TSelf Try(Func<TSelf> action, Func<Exception, IError>? catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            return action();
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }

    /// <summary>
    ///     Executes the action. If an exception is thrown within the action then this exception is transformed via the
    ///     catchHandler to an Error object
    /// </summary>
    public static async Task<TSelf> Try(Func<Task<TSelf>> action, Func<Exception, IError>? catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            return await action();
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }

    /// <summary>
    ///     Executes the action. If an exception is thrown within the action then this exception is transformed via the
    ///     catchHandler to an Error object
    /// </summary>
    public static async ValueTask<TSelf> Try(Func<ValueTask<TSelf>> action,
        Func<Exception, IError>? catchHandler = null)
    {
        catchHandler = catchHandler ?? Settings.DefaultTryCatchHandler;

        try
        {
            return await action();
        }
        catch (Exception e)
        {
            return Fail(catchHandler(e));
        }
    }
}
    
public partial class Result<TSelf> : ResultBase<TSelf>
    where TSelf : Result<TSelf>, new()
{
    /// <summary>
    ///     Default constructor
    /// </summary>
    public Result()
    {
    }
        
    /// <summary>
    ///     Execute an action which returns a <see cref="Result" />.
    /// </summary>
    /// <example>
    ///     <code>
    ///  var done = result.Bind(ActionWhichMayFail);
    /// </code>
    /// </example>
    /// <param name="action">Action that may fail.</param>
    public TSelf Bind(Func<TSelf> action)
    {
        var result = new TSelf();
        result.WithReasons(Reasons);

        if (IsSuccess)
        {
            var converted = action();
            result.WithReasons(converted.Reasons);
        }

        return result;
    }

    /// <summary>
    ///     Execute an action which returns a <see cref="Result" /> asynchronously.
    /// </summary>
    /// <example>
    ///     <code>
    ///  var done = result.Bind(ActionWhichMayFail);
    /// </code>
    /// </example>
    /// <param name="action">Action that may fail.</param>
    public async Task<TSelf> Bind(Func<Task<TSelf>> action)
    {
        var result = new TSelf();
        result.WithReasons(Reasons);

        if (IsSuccess)
        {
            var converted = await action();
            result.WithReasons(converted.Reasons);
        }

        return result;
    }

    /// <summary>
    ///     Execute an action which returns a <see cref="Result" /> asynchronously.
    /// </summary>
    /// <example>
    ///     <code>
    ///  var done = result.Bind(ActionWhichMayFail);
    /// </code>
    /// </example>
    /// <param name="action">Action that may fail.</param>
    public async ValueTask<TSelf> Bind(Func<ValueTask<TSelf>> action)
    {
        var result = new TSelf();
        result.WithReasons(Reasons);

        if (IsSuccess)
        {
            var converted = await action();
            result.WithReasons(converted.Reasons);
        }

        return result;
    }

    /// <summary>
    ///     Deconstruct Result
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <param name="isFailed"></param>
    public void Deconstruct(out bool isSuccess, out bool isFailed)
    {
        isSuccess = IsSuccess;
        isFailed = IsFailed;
    }

    /// <summary>
    ///     Deconstruct Result
    /// </summary>
    /// <param name="isSuccess"></param>
    /// <param name="isFailed"></param>
    /// <param name="errors"></param>
    public void Deconstruct(out bool isSuccess, out bool isFailed, out IReadOnlyList<IError> errors)
    {
        isSuccess = IsSuccess;
        isFailed = IsFailed;
        errors = (IsFailed 
            ? Errors 
            : null)
                 ?? Array.Empty<IError>();
    }
}