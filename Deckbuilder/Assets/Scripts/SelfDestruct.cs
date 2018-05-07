using UnityEngine;

public class SelfDestruct : MonoBehaviour {

    public float time = 1f;
    float elapsedTime = 0f;
	
	//Destroy object after time has elapsed
	void Update () {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= time) {
            Destroy(gameObject);
        }
	}
}
