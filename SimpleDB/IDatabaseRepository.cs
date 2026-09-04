namespace SimpleDB;

public interface IDatabaseRepository<T>
{
    public IEnumerable<T> Read(string file, int? limit = null);
    public void Store(T record);
}
