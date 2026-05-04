using UnityEngine;

public class PetrifyPotion : MonoBehaviour
{
    public GameObject effect;

    Vector3 randomRotAxis;
    float randomRotSpeed;

    public void Awake()
    {
        this.randomRotAxis = Random.onUnitSphere;
        this.randomRotSpeed = 60 + Random.value * 300;
    }

    public void Update()
    {
        this.transform.rotation *= Quaternion.AngleAxis(randomRotSpeed * Time.deltaTime, randomRotAxis);
    }

    void OnTriggerEnter(Collider trigger)
    {
        if (effect != null)
        {
            Instantiate(effect, this.transform.position, Quaternion.identity, null).SetActive(true);
        }
        else Debug.LogWarning("PetrifyPotion has no effect set!");
        Destroy(this.gameObject);
    }
}
