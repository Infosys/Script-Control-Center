using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.Resource.IExportDataAccess
{
    public interface IEntity<T>
    {
        //T GetOne(T Entity);
        IQueryable<T> GetAny();
        //IList<T> GetAll();
        int Insert(T entity);
        //T Update(T entity);
        //bool Delete(T entity);
        //IList<T> InsertBatch(IList<T> entities);
        //IList<T> UpdateBatch(IList<T> entities);
        //IList<T> GetAll(T Entity);
    }
}
