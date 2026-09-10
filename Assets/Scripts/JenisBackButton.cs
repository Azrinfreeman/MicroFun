using UnityEngine;
using UnityEngine.UI;

/// <summary>Returns from Jenis to the Definisi panel.</summary>
[RequireComponent(typeof(Button))]
public class JenisBackButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(ReturnToDefinisi);
    }

    private void ReturnToDefinisi()
    {
        Transform canvas = transform.root;
        Transform jenis = canvas.Find("Jenis");
        Transform definisi = canvas.Find("Definisi");
        if (jenis != null) jenis.gameObject.SetActive(false);
        if (definisi != null) definisi.gameObject.SetActive(true);
    }
}