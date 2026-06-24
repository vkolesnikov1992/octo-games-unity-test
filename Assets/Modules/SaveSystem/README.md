# SaveSystem

Generic save/load utility для Unity-serializable классов.

## API

- `ISaveService.SaveAsync<T>(key, data)` сохраняет объект по ключу.
- `ISaveService.LoadAsync<T>(key)` возвращает `LoadResult<T>`, а не бросает исключение для обычных ошибок загрузки.
- `ISaveStorage` отвечает за место хранения.
- `ISaveSerializer` отвечает за сериализацию.

## Реализация

- `JsonSaveService` оборачивает payload в `SaveEnvelope` с версией, типом и признаком `null`.
- `FileSaveStorage` хранит `.json` файлы в `Application.persistentDataPath/Saves`.
- `UnityJsonSaveSerializer` использует `JsonUtility`.
- `LoadErrorCode` покрывает missing file, read error, invalid JSON, type mismatch и deserialization error.

## Границы

`JsonUtility` требует Unity-serializable data classes и не покрывает все C#-типы. Миграции, слоты, encryption, cloud save, retry policy и UI выбора сохранений ожидаются как внешний код или следующие расширения.
