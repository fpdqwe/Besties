namespace Domain.Abstractions
{
    public interface IDbEntity
    {
        Guid GetPrimaryKey();
    }
}
