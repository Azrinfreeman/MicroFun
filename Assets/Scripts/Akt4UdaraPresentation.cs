using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Two readable spreads retain the discussion-to-investigation lesson order.</summary>
public sealed class Akt4UdaraPresentation : MonoBehaviour
{
    [SerializeField] GameObject discussion;
    [SerializeField] GameObject investigation;
    [SerializeField] TMP_Text pageLabel;
    [SerializeField] TMP_Text nextLabel;
    [SerializeField] Button previousButton;
    public int CurrentPage { get; private set; }

    void OnEnable() { Show(0); }
    public void Next() { Show(CurrentPage == 0 ? 1 : 0); }
    public void Previous() { Show(0); }
    public void Show(int page)
    {
        CurrentPage = Mathf.Clamp(page, 0, 1);
        if (discussion != null) discussion.SetActive(CurrentPage == 0);
        if (investigation != null) investigation.SetActive(CurrentPage == 1);
        if (pageLabel != null) pageLabel.text = CurrentPage == 0 ? "1 / 2  •  BINCANG" : "2 / 2  •  SIASAT";
        if (nextLabel != null) nextLabel.text = CurrentPage == 0 ? "TERUSKAN" : "ULANG";
        if (previousButton != null) previousButton.gameObject.SetActive(CurrentPage == 1);
    }
}
