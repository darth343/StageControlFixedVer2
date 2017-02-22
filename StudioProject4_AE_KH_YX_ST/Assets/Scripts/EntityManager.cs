using UnityEngine;
using System.Collections;

public class EntityManager : MonoBehaviour {

    void AddToEntityList(GameObject go)
    {
        go.transform.SetParent(gameObject.transform);
    }
}
