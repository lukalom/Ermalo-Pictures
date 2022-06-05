namespace EP.Application.Services.UserContext
{
    public interface IUserContext
    {
        public Guid? UserId { get; init; }
        public string Email { get; init; }
    }
}
