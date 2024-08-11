using System.Collections.Generic;

namespace CsharpTaskManagement.Repositories
{
    public interface ICrudRepository<T, ID>
    {
        public IEnumerable<T> FindAll();

        public T? FindById(ID id);

        public T? Save(T entity);

        public bool Delete(T entity);

        public bool DeleteById(ID id);
    }
}