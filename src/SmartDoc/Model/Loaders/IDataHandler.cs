using System.Threading;
using System.Threading.Tasks;

namespace Wikiled.SmartDoc.Model.Loaders
{
    public interface IDataHandler<T>
        where T : class 
    {
        Task<T> Load(CancellationToken cancellation);

        Task Save(T data, CancellationToken cancellation);
    }
}
