using Cysharp.Threading.Tasks;
using System.Threading;

namespace OctoGamesTest.SaveSystem
{
    public interface ISaveStorage
    {
        UniTask<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

        UniTask<string> ReadTextAsync(string key, CancellationToken cancellationToken = default);

        UniTask WriteTextAsync(string key, string content, CancellationToken cancellationToken = default);
    }
}
