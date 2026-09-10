using TMPro;
using UnityEngine;

/// <summary>
/// Keeps the themed Micro Scene page indicator synchronized with the two
/// legacy content pages. The existing navigation callbacks remain authoritative.
/// </summary>
public sealed class Akt7MicroScenePresenter : MonoBehaviour
{
    [SerializeField] private GameObject firstPage;
    [SerializeField] private GameObject secondPage;
    [SerializeField] private TMP_Text pageIndicator;

    private void Start()
    {
        RefreshPageIndicator();
    }

    public void RefreshPageIndicator()
    {
        if (pageIndicator == null)
            return;

        bool secondPageVisible = secondPage != null && secondPage.activeSelf;
        pageIndicator.text = secondPageVisible ? "Halaman 2/2" : "Halaman 1/2";
    }
}
