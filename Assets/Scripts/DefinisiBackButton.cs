using UnityEngine;
using UnityEngine.UI;

/// <summary>Returns from the Definisi panel to the preceding Pengenalan panel.</summary>
[RequireComponent(typeof(Button))]
public class DefinisiBackButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(ReturnToPengenalan);
    }

    private void ReturnToPengenalan()
    {
        Transform canvas = transform.root;
        Transform definisi = canvas.Find("Definisi");
        Transform pengenalan = canvas.Find("Pengenalan");
        if (definisi != null) definisi.gameObject.SetActive(false);
        if (pengenalan != null) pengenalan.gameObject.SetActive(true);
    }
}