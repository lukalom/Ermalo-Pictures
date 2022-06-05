namespace EP.Infrastructure.IConfiguration
{
    public interface IEntity
    {
        public DateTime CreatedOnUtc { get; set; }
        public bool IsDeleted { get; set; }
    }

}
