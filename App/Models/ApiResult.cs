namespace Angular.App.Models
{
    public class ApiResponse<T>
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public object? Metadata { get; set; }
        public T? Data { get; set; }
        public object? Error { get; set; }
    }
}
