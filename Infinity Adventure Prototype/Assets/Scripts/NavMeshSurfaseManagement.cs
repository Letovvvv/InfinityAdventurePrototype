using NavMeshPlus.Components;
using UnityEngine;

public class NavMeshSurfaseManagement : MonoBehaviour
{
    public static NavMeshSurfaseManagement Instance { get; private set; }

    private NavMeshSurface _navMeshSurface;

    private void Awake()
    {
        Instance = this;
        _navMeshSurface = GetComponent<NavMeshSurface>();
        //navMeshSurface.hideEditorLogs = true;
    }

    public void RebakeNavMeshSurface()
    {
        _navMeshSurface.BuildNavMesh();
    }
}
