using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Transform Content;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Content.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Content != null)
        {
            Content.GetComponent<Animator>().Play("ExitContent");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
