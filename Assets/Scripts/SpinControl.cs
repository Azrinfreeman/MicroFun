using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpinControl : MonoBehaviour
{
    [Header("Legacy Settings (kept for scene compatibility)")]
    public float minSpinSpeed = 500f;
    public float maxSpinSpeed = 1202f;
    public float maxRandomStartRotation = 360f;

    [Header("Micro Squad Wheel")]
    [Min(0.5f)] public float spinDuration = 4.2f;
    [Min(1)] public int minTurns = 3;
    [Min(1)] public int maxTurns = 5;
    public AnimationCurve spinCurve = new AnimationCurve(
        new Keyframe(0f, 0f, 0f, 1.8f),
        new Keyframe(0.72f, 0.93f, 0.35f, 0.35f),
        new Keyframe(1f, 1f, 0f, 0f));
    public string[] categories = { "Bakteria", "Fungi", "Alga", "Protozoa", "Virus" };

    [Header("Optional UI Hooks")]
    public Button spinButton;
    public TMP_Text resultLabel;
    public RectTransform[] uprightLabels;
    public UnityEvent<string> onCategorySelected = new UnityEvent<string>();

    public bool IsSpinning => isSpinning;
    public string LastCategory { get; private set; }

    private bool isSpinning;
    private Coroutine spinRoutine;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }
        if (spinButton != null)
        {
            spinButton.onClick.RemoveListener(Spin);
            spinButton.onClick.AddListener(Spin);
        }

    }

    private void OnDestroy()
    {
        if (spinButton != null)
            spinButton.onClick.RemoveListener(Spin);
    }

    private void OnDisable()
    {
        if (spinRoutine != null)
        {
            StopCoroutine(spinRoutine);
            spinRoutine = null;
        }

        isSpinning = false;
        if (spinButton != null)
            spinButton.interactable = true;
    }

    public void Spin()
    {
        if (isSpinning || categories == null || categories.Length == 0)
            return;

        spinRoutine = StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        isSpinning = true;
        if (spinButton != null)
            spinButton.interactable = false;
        if (resultLabel != null)
            resultLabel.text = "Sedang berputar...";

        int selectedIndex = Random.Range(0, categories.Length);
        int turns = Random.Range(Mathf.Max(1, minTurns), Mathf.Max(minTurns, maxTurns) + 1);

        float segmentAngle = 360f / categories.Length;
        float startAngle = transform.localEulerAngles.z;
        float targetNormalized = selectedIndex * segmentAngle;
        float positiveDelta = Mathf.Repeat(targetNormalized - Mathf.Repeat(startAngle, 360f), 360f);
        float targetAngle = startAngle + (turns * 360f) + positiveDelta;

        float elapsed = 0f;
        float duration = Mathf.Max(0.5f, spinDuration);
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = spinCurve != null ? spinCurve.Evaluate(t) : Mathf.SmoothStep(0f, 1f, t);
            SetLocalAngle(Mathf.LerpUnclamped(startAngle, targetAngle, eased));
            yield return null;
        }

        SetLocalAngle(targetNormalized);
        LastCategory = categories[selectedIndex];

        if (resultLabel != null)
            resultLabel.text = LastCategory + " dipilih!";

        onCategorySelected.Invoke(LastCategory);

        if (spinButton != null)
            spinButton.interactable = true;

        isSpinning = false;
        spinRoutine = null;
    }

    private void SetLocalAngle(float angle)
    {
        Vector3 rotation = transform.localEulerAngles;
        rotation.z = angle;
        transform.localEulerAngles = rotation;
        if (uprightLabels == null)
            return;

        for (int i = 0; i < uprightLabels.Length; i++)
        {
            if (uprightLabels[i] != null)
                uprightLabels[i].localEulerAngles = new Vector3(0f, 0f, -angle);
        }
    }
}
