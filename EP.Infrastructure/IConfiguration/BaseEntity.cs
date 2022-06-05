using System.ComponentModel.DataAnnotations;

namespace EP.Infrastructure.IConfiguration
{
    public abstract class BaseEntity<T> : IEntity, IPrimaryKey<T> where T : notnull
    {
        [Required]
        public T Id { get; set; }

        [Required] public DateTime CreatedOnUtc { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;
    }
}
