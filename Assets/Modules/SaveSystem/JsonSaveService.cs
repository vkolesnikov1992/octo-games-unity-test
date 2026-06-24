using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Threading;
using UnityEngine;

namespace OctoGamesTest.SaveSystem
{
    public sealed class JsonSaveService : ISaveService
    {
        private const int EnvelopeVersion = 1;

        private readonly ISaveStorage _storage;
        private readonly ISaveSerializer _serializer;

        public JsonSaveService(ISaveStorage storage, ISaveSerializer serializer)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async UniTask SaveAsync<T>(
            string key,
            T data,
            CancellationToken cancellationToken = default)
            where T : class
        {
            ValidateKey(key);
            cancellationToken.ThrowIfCancellationRequested();

            var envelope = new SaveEnvelope
            {
                Version = EnvelopeVersion,
                TypeName = typeof(T).AssemblyQualifiedName,
                IsNull = data == null,
                Payload = data == null ? null : _serializer.Serialize(data)
            };

            await _storage.WriteTextAsync(key, JsonUtility.ToJson(envelope), cancellationToken);
        }

        public async UniTask<LoadResult<T>> LoadAsync<T>(
            string key,
            CancellationToken cancellationToken = default)
            where T : class
        {
            ValidateKey(key);
            cancellationToken.ThrowIfCancellationRequested();

            bool exists;
            try
            {
                exists = await _storage.ExistsAsync(key, cancellationToken);
            }
            catch (IOException exception)
            {
                return LoadResult<T>.Failed(LoadErrorCode.ReadFailed, exception);
            }

            if (!exists)
            {
                return LoadResult<T>.Failed(LoadErrorCode.FileNotFound);
            }

            string json;
            try
            {
                json = await _storage.ReadTextAsync(key, cancellationToken);
            }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
            {
                return LoadResult<T>.Failed(LoadErrorCode.ReadFailed, exception);
            }

            SaveEnvelope envelope;
            try
            {
                envelope = JsonUtility.FromJson<SaveEnvelope>(json);
            }
            catch (Exception exception) when (exception is ArgumentException || exception is InvalidOperationException)
            {
                return LoadResult<T>.Failed(LoadErrorCode.InvalidJson, exception);
            }

            if (!IsValidEnvelope(envelope))
            {
                return LoadResult<T>.Failed(LoadErrorCode.InvalidJson);
            }

            if (!string.Equals(envelope.TypeName, typeof(T).AssemblyQualifiedName, StringComparison.Ordinal))
            {
                return LoadResult<T>.Failed(LoadErrorCode.TypeMismatch);
            }

            if (envelope.IsNull)
            {
                return LoadResult<T>.Ok(null);
            }

            try
            {
                return LoadResult<T>.Ok(_serializer.Deserialize<T>(envelope.Payload));
            }
            catch (Exception exception) when (exception is ArgumentException || exception is InvalidOperationException)
            {
                return LoadResult<T>.Failed(LoadErrorCode.DeserializationFailed, exception);
            }
        }

        private static bool IsValidEnvelope(SaveEnvelope envelope)
        {
            return envelope != null
                   && envelope.Version == EnvelopeVersion
                   && !string.IsNullOrWhiteSpace(envelope.TypeName);
        }

        private static void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Save key cannot be empty.", nameof(key));
            }
        }
    }
}
