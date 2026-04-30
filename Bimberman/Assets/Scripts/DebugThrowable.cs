using UnityEngine;

public class DebugThrowable : MonoBehaviour
{
    public GameObject effect;

    Vector3 randomRotAxis;
    float randomRotSpeed;

    public void Awake() {
        this.randomRotAxis = Random.onUnitSphere;
        this.randomRotSpeed = 60 + Random.value * 300;
    }

    public void Update() {
        this.transform.rotation *= Quaternion.AngleAxis(randomRotSpeed * Time.deltaTime, randomRotAxis);
    }

    void OnTriggerEnter(Collider trigger) {
        Instantiate(effect, this.transform.position, Quaternion.identity, null).SetActive(true);

        Destroy(this.gameObject);

        this.transform.localScale *= 3;
    }
}
