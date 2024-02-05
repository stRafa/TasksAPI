using FluentValidation.Results;

namespace Tasks.Application;

public class ResultViewModel<T>
{
    public ResultViewModel(T data, List<string> errors)
    {
        Data = data;
        Errors = errors;
    }

    public ResultViewModel(T data)
    {
        Data = data;
    }

    public ResultViewModel(List<string> errors)
    {
        Errors = errors;
    }

    public ResultViewModel(ValidationResult validationResult)
    {
        Errors = validationResult.Errors.Select((error) => error.ErrorMessage).ToList();
    }

    public ResultViewModel(string error)
    {
        Errors.Add(error);
    }

    public T Data { get; private set; }
    public List<string> Errors { get; private set; } = new();
    public bool Success => Errors.Count == 0;
}