namespace EP.Application.Services.Account.Role.DTO.Request
{
    public record GetUsersFilterDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int PageSize { get; set; }
        public int Page { get; set; }
    }
}
