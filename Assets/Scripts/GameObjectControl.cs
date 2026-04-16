using UnityEngine;

public class GameObjectControl : MonoBehaviour
{
    public void Disappear()
    {
        gameObject.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
