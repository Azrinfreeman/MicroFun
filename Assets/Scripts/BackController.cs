using UnityEngine;
using UnityEngine.UI;

public class BackController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>()
            .onClick.AddListener(
                GameObject.Find("SceneController").GetComponent<SceneController>().UnloadScene
            );
    }

    // Update is called once per frame
    void Update() { }
}
