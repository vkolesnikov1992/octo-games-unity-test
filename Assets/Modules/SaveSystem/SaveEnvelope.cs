using System;

namespace OctoGamesTest.SaveSystem
{
    [Serializable]
    internal sealed class SaveEnvelope
    {
        public int Version;
        public string TypeName;
        public bool IsNull;
        public string Payload;
    }
}
