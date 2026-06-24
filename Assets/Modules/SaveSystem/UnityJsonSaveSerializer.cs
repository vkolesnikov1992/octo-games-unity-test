using System;
using UnityEngine;

namespace OctoGamesTest.SaveSystem
{
    public sealed class UnityJsonSaveSerializer : ISaveSerializer
    {
        public string Serialize<T>(T data)
            where T : class
        {
            return JsonUtility.ToJson(data);
        }

        public T Deserialize<T>(string json)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("Json is empty.", nameof(json));
            }

            return JsonUtility.FromJson<T>(json);
        }
    }
}
