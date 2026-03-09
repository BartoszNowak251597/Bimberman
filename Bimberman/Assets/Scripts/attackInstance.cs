using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class attackInstance : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    float t = 0;
    // Update is called once per frame
    void Update()
    {
        t += 1;
        if (t>180)
        {
            //Timer = new Timer(DestroySelf, null, 2000, Timeout.Infinite);
            Destroy(gameObject);
        }
    }
}
