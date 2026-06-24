namespace OctoGamesTest.SaveSystem
{
    public enum LoadErrorCode
    {
        None = 0,
        FileNotFound = 1,
        ReadFailed = 2,
        InvalidJson = 3,
        TypeMismatch = 4,
        DeserializationFailed = 5
    }
}
