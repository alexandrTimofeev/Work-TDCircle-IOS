// Интерфейс EntryPoint сцены
public interface ISceneEntryPoint
{
    string SceneName { get; }
    void InitGSystems();      // локальные G-системы сцены
    void OnSceneLoaded();
    void OnSceneUnloaded();   // сброс локальных G-систем
}