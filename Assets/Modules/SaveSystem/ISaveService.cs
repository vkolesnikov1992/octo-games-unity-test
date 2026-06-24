using Cysharp.Threading.Tasks;
using System.Threading;

namespace OctoGamesTest.SaveSystem
{
    public interface ISaveService
    {
        UniTask SaveAsync<T>(string key, T data, CancellationToken cancellationToken = default)
            where T : class;

        UniTask<LoadResult<T>> LoadAsync<T>(string key, CancellationToken cancellationToken = default)
            where T : class;
    }
}
