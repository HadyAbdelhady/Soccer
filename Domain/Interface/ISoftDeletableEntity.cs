namespace Data.Interface
{
    public interface ISoftDeletableEntity : IAuditableEntity
    {
        bool IsDeleted { get; set; }
    }

    public interface IAuditableEntity : IEntity
    {
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset? UpdatedAt { get; set; }
    }

    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
