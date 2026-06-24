namespace OctoGamesTest.SaveSystem
{
    public interface ISaveSerializer
    {
        string Serialize<T>(T data)
            where T : class;

        T Deserialize<T>(string json)
            where T : class;
    }
}
