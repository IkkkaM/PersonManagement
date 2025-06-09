using System.ComponentModel.DataAnnotations;

namespace PersonDirectory.Domain.Entities;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; protected set; }

    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    protected BaseEntity()
    {
    }

    public void SetUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}