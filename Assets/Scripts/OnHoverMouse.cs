using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Transform Content;
    public bool extraInfo;

    public Transform extraInfoContent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Content.gameObject.SetActive(true);
        if (extraInfo && extraInfoContent != null)
        {
            extraInfoContent.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Content != null)
        {
            Content.GetComponent<Animator>().Play("ExitContent");
        }
        if (extraInfo && extraInfoContent != null)
        {
            extraInfoContent.gameObject.SetActive(false);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
