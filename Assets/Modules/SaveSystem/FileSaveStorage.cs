using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

namespace OctoGamesTest.SaveSystem
{
    public sealed class FileSaveStorage : ISaveStorage
    {
        private const string Extension = ".json";

        private readonly string _rootPath;

        public FileSaveStorage()
            : this(null)
        {
        }

        public FileSaveStorage(string rootPath)
        {
            _rootPath = string.IsNullOrWhiteSpace(rootPath)
                ? Path.Combine(Application.persistentDataPath, "Saves")
                : rootPath;
        }

        public UniTask<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return UniTask.FromResult(File.Exists(GetPath(key)));
        }

        public async UniTask<string> ReadTextAsync(string key, CancellationToken cancellationToken = default)
        {
            var path = GetPath(key);
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async UniTask WriteTextAsync(string key, string content, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Directory.CreateDirectory(_rootPath);

            var path = GetPath(key);
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                await writer.WriteAsync(content);
            }
        }

        private string GetPath(string key)
        {
            return Path.Combine(_rootPath, NormalizeKey(key) + Extension);
        }

        private static string NormalizeKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Save key cannot be empty.", nameof(key));
            }

            var fileName = key.Trim();
            foreach (var invalidCharacter in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(invalidCharacter, '_');
            }

            return fileName;
        }
    }
}
