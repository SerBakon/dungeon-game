using UnityEngine;
using Unity.AI.Navigation;
using System.Collections;
public class BuildNavMesh : MonoBehaviour
{
    public NavMeshSurface m_NavmeshSurface;

    public void buildMesh() {
        m_NavmeshSurface.BuildNavMesh();
    }
}
