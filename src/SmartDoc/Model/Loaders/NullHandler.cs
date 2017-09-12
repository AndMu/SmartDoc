using System.Threading;
using System.Threading.Tasks;

namespace Wikiled.SmartDoc.Model.Loaders
{
    public class NullHandler<T> : IDataHandler<T>
        where T : class 
    {
        public Task<T> Load(CancellationToken cancellation)
        {
            return Task.FromResult((T)null);
        }

        public Task Save(T data, CancellationToken cancellation)
        {
            return Task.CompletedTask;
        }
    }
}
