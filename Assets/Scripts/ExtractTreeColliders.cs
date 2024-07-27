using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class ExtractTreeColliders : MonoBehaviour
{
    [SerializeField]
    private Terrain terrain;

    private void Reset()
    {
        terrain = GetComponent<Terrain>();

        Extract();
    }

    private void CreateCollidersForObject(CapsuleCollider prefabCollider,  GameObject obj, TreePrototype tree)
    {

        CapsuleCollider objCollider = obj.AddComponent<CapsuleCollider>();

        objCollider.center = prefabCollider.center;
        objCollider.height = prefabCollider.height;
        objCollider.radius = prefabCollider.radius;

        if (terrain.preserveTreePrototypeLayers) obj.layer = tree.prefab.layer;
        else obj.layer = terrain.gameObject.layer;
    }

    private void CreateCollidersForObject(BoxCollider prefabCollider,  GameObject obj, TreePrototype tree)
    {

        BoxCollider objCollider = obj.AddComponent<BoxCollider>();

        objCollider.center = prefabCollider.center;
        objCollider.size = prefabCollider.size;

        if (terrain.preserveTreePrototypeLayers) obj.layer = tree.prefab.layer;
        else obj.layer = terrain.gameObject.layer;
    }



    [ContextMenu("Extract")]
    public void Extract()
    {
        Collider[] colliders = terrain.GetComponentsInChildren<Collider>();

        //Skip the first, since its the Terrain Collider
        for (int i = 1; i < colliders.Length; i++)
        {
            //Delete all previously created colliders first
            DestroyImmediate(colliders[i].gameObject);
        }

        for (int i = 0; i < terrain.terrainData.treePrototypes.Length; i++)
        {
            TreePrototype tree = terrain.terrainData.treePrototypes[i];

            //Get all instances matching the prefab index
            TreeInstance[] instances = terrain.terrainData.treeInstances.Where(x => x.prototypeIndex == i).ToArray();

            for (int j = 0; j < instances.Length; j++)
            {
                //Un-normalize positions so they're in world-space
                instances[j].position = Vector3.Scale(instances[j].position, terrain.terrainData.size);
                instances[j].position += terrain.GetPosition();

                //Fetch the collider from the prefab object parent
                CapsuleCollider prefabCapsuleCollider;
                BoxCollider prefabBoxCollider;
                GameObject obj = new GameObject();
                obj.transform.position = instances[j].position;
                obj.transform.parent = terrain.transform;
                obj.name = tree.prefab.name + j;
                if (tree.prefab.TryGetComponent(out prefabCapsuleCollider))
                {
                    CreateCollidersForObject(prefabCapsuleCollider, obj, tree);
                }
                else if (tree.prefab.TryGetComponent(out prefabBoxCollider))
                {
                    CreateCollidersForObject(prefabBoxCollider, obj, tree);
                }
                else
                {
                    obj.name = "NoCollider_" + obj.name;
                }
            }
        }
    }
}