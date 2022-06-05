namespace EP.Infrastructure.IConfiguration
{
    public interface ISeeder
    {
        public int Index { get; set; }
        Task Seed();
    }

}
