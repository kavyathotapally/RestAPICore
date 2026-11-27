
namespace RestAPICore.Models
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; } = true;

        public ErrorDetails? Error { get; set; }

        public T? Data { get; set; }
    }

    public class ErrorDetails
    {
        public string Msg { get; set; } = string.Empty;

        public int Code { get; set; }

        public string? ProvMsg { get; set; }

        public string? TraceId { get; set; }
    }
}
