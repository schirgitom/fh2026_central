using System.Collections.Generic;

namespace Services.Results;

public class ServiceResult<T>
{
    public T? Data { get; set; }
    public List<string> Errors { get; } = new();
    public List<string> Warnings { get; } = new();

    public bool IsSuccess => Errors.Count == 0;
}
