namespace Nlpc_Pension_Project.Application.Services;

public class Responses<T>
{
    public Responses()
    {

    }

    public Responses(DateTime requestTime, T? data, string? responseCode, string? responseMessage, string? description = null,
        string? requestId = null, bool isSuccess = false)
    {
        Value = data;
        RequestTime = requestTime;
        ResponseDescription = description;
        ResponseCode = responseCode;
        Message = responseMessage;
        RequestId = requestId;
        IsSuccess = isSuccess;
    }


    public Responses(string errorMessage) => ErrorMessage = errorMessage;

    public Responses(DateTime requestTime, string errorMessage, IDictionary<string, string[]> errors)
    {
        ErrorMessage = errorMessage;
        Errors = errors;
        RequestTime = requestTime;
    }


    public Responses(DateTime requestTime, string errorMessage, string? responseCode = "")
    {
        RequestTime = requestTime;
        ErrorMessage = errorMessage;
        ResponseCode = responseCode;
    }

    public T? Value { get; init; }
    public T? Content { get; set; }
    public string? RequestId { get; init; }
    public string? ErrorMessage { get; init; }

    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    public string? ResponseCode { get; init; }
    public string? ResponseDescription { get; init; }
    public DateTime RequestTime { get; init; }
    public DateTime ResponseTime { get; init; } = DateTime.Now;
    public double ActivityTime => ResponseTime.Subtract(RequestTime).TotalMilliseconds;
    public bool IsSuccess { get; init; }
    public string? Message { get; set; }


    public static Responses<T> Success(DateTime requestTime, T value, string? responseCode = "000",
        string? responseMessage = null,
        string? responseDescription = null, string? requestId = null) =>
        new(requestTime, value, responseCode, responseMessage, responseDescription, requestId, true);

    public static Responses<T> Failure(DateTime requestTime, string errorMessage, string? responseCode = "") =>
        new(requestTime, errorMessage, responseCode);

    public static Responses<T> Failure(string errorMessage) =>
        new(errorMessage);
}
