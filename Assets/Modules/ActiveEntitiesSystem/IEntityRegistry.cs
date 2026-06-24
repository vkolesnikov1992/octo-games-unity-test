namespace OctoGamesTest.ActiveEntitiesSystem
{
    public interface IEntityRegistry
    {
        bool Register(IGameplayEntity entity);

        bool Unregister(IGameplayEntity entity);

        bool SetActive(IGameplayEntity entity, bool isActive);
    }
}
