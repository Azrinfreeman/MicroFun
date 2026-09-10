using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Kuiz2HomeButtonRoute : MonoBehaviour
{
    void Awake()
    {
        var button = GetComponent<Button>();
        if (button != null) button.onClick.AddListener(ReturnToAkt2);
    }

    void ReturnToAkt2()
    {
        SceneManager.LoadScene("akt2");
    }
}
