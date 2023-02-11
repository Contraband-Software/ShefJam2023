using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture
{
    public class Clouds : MonoBehaviour
    {
        [SerializeField] List<GameObject> cloudSprites;
        [SerializeField, Min(0)] int amount = 10;

        List<Transform> instances;
        RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            for (int i = 0; i < amount; i++)
            {
                Instantiate(
                    cloudSprites[Random.Range(0, cloudSprites.Count)], 
                    new Vector3(Random.Range(rectTransform.offsetMin.x, -rectTransform.offsetMax.x), Random.Range(rectTransform.offsetMin.y, -rectTransform.offsetMax.y), 0), 
                    Quaternion.identity, 
                    transform
                );
            }
        }
    }
}