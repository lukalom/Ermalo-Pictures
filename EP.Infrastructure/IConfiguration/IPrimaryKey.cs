namespace EP.Infrastructure.IConfiguration
{
    public interface IPrimaryKey<T> where T : notnull
    {
        T Id { get; set; }
    }
}
