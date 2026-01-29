namespace CRM.Demo.Domain.Common;

/// <summary>
/// Railway Oriented Programming - Result pattern
/// Reprezentuje wynik operacji, który może być sukcesem (z wartością) lub błędem.
/// Umożliwia jawną obsługę błędów bez używania wyjątków dla błędów biznesowych.
/// </summary>
/// <typeparam name="TValue">Typ wartości sukcesu</typeparam>
/// <typeparam name="TError">Typ błędu</typeparam>
public class Result<TValue, TError>
{
    private readonly TValue? _value;
    private readonly TError? _error;

    private Result(TValue? value, TError? error)
    {
        _value = value;
        _error = error;
    }

    /// <summary>
    /// Czy wynik jest sukcesem
    /// </summary>
    public bool IsSuccess => _error == null;

    /// <summary>
    /// Czy wynik jest błędem
    /// </summary>
    public bool IsFailure => _error != null;

    /// <summary>
    /// Wartość (tylko gdy IsSuccess == true)
    /// </summary>
    public TValue Value
    {
        get
        {
            if (IsFailure)
                throw new InvalidOperationException("Cannot get value from failed result");
            return _value!;
        }
    }

    /// <summary>
    /// Błąd (tylko gdy IsFailure == true)
    /// </summary>
    public TError Error
    {
        get
        {
            if (IsSuccess)
                throw new InvalidOperationException("Cannot get error from successful result");
            return _error!;
        }
    }

    /// <summary>
    /// Tworzy wynik sukcesu
    /// </summary>
    public static Result<TValue, TError> Success(TValue value) => new(value, default);

    /// <summary>
    /// Tworzy wynik błędu
    /// </summary>
    public static Result<TValue, TError> Failure(TError error) => new(default, error);

    /// <summary>
    /// Mapuje wartość sukcesu na inną wartość
    /// </summary>
    public Result<TNewValue, TError> Map<TNewValue>(Func<TValue, TNewValue> func)
    {
        if (IsSuccess)
            return Result<TNewValue, TError>.Success(func(_value!));
        return Result<TNewValue, TError>.Failure(_error!);
    }

    /// <summary>
    /// Bind (flat map) - łączy wyniki w pipeline (Railway Oriented Programming)
    /// </summary>
    public Result<TNewValue, TError> Bind<TNewValue>(Func<TValue, Result<TNewValue, TError>> func)
    {
        if (IsSuccess)
            return func(_value!);
        return Result<TNewValue, TError>.Failure(_error!);
    }

    /// <summary>
    /// Bind async - łączy wyniki w pipeline asynchronicznie (Railway Oriented Programming)
    /// </summary>
    public async Task<Result<TNewValue, TError>> BindAsync<TNewValue>(Func<TValue, Task<Result<TNewValue, TError>>> func)
    {
        if (IsSuccess)
            return await func(_value!);
        return Result<TNewValue, TError>.Failure(_error!);
    }

    /// <summary>
    /// Wykonuje akcję gdy sukces
    /// </summary>
    public Result<TValue, TError> OnSuccess(Action<TValue> action)
    {
        if (IsSuccess)
            action(_value!);
        return this;
    }

    /// <summary>
    /// Wykonuje akcję gdy błąd
    /// </summary>
    public Result<TValue, TError> OnFailure(Action<TError> action)
    {
        if (IsFailure)
            action(_error!);
        return this;
    }

    /// <summary>
    /// Mapuje błąd na inny typ błędu
    /// </summary>
    public Result<TValue, TNewError> MapError<TNewError>(Func<TError, TNewError> func)
    {
        if (IsFailure)
            return Result<TValue, TNewError>.Failure(func(_error!));
        return Result<TValue, TNewError>.Success(_value!);
    }

    /// <summary>
    /// Match - pattern matching dla Result
    /// </summary>
    public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess,
        Func<TError, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(_value!) : onFailure(_error!);
    }

    /// <summary>
    /// Match - pattern matching z akcjami (void)
    /// </summary>
    public void Match(
        Action<TValue> onSuccess,
        Action<TError> onFailure)
    {
        if (IsSuccess)
            onSuccess(_value!);
        else
            onFailure(_error!);
    }

    /// <summary>
    /// Implicit conversion z wartości na Result (dla wygody)
    /// </summary>
    public static implicit operator Result<TValue, TError>(TValue value)
    {
        return Success(value);
    }
}

/// <summary>
/// Unit type dla metod, które nie zwracają wartości ale używają Result Pattern
/// </summary>
public struct Unit
{
    public static Unit Default => new();
}
