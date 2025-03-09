using UnityEngine;

public class BootStrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitContainers()
    {
        foreach (var manager in Object.FindObjectsOfType<MonoBehaviour>())
        {
            if (manager is not IManager iManager) continue;
            DIContainer.Register(iManager.GetType(), iManager);
            DIContainer.AutoInject(manager);
        }
    }
}
