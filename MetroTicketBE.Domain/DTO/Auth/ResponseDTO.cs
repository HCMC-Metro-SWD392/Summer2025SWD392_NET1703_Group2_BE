namespace MetroTicketBE.Domain.DTO.Auth
{
    public class ResponseDTO
    {
        public object? Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = true;
        public int StatusCode { get; set; } = 200;
    }
}
