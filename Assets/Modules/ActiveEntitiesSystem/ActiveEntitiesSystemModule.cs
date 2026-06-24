using Zenject;

namespace OctoGamesTest.ActiveEntitiesSystem
{
    public static class ActiveEntitiesSystemModule
    {
        public static void Register(DiContainer container)
        {
            container.BindInterfacesAndSelfTo<EntityTrackingSystem>().AsSingle();
        }
    }
}
