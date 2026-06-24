using System;
using Zenject;

namespace OctoGamesTest.SaveSystem
{
    public static class SaveSystemModule
    {
        public static void Register(DiContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.Bind<ISaveStorage>().FromMethod(_ => new FileSaveStorage()).AsSingle();
            container.Bind<ISaveSerializer>().To<UnityJsonSaveSerializer>().AsSingle();
            container.Bind<ISaveService>().To<JsonSaveService>().AsSingle();
        }
    }
}
