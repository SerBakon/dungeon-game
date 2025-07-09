using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;

    [SerializeField] private float maxProgress;
    [SerializeField] private float incrementAmount;

    public bool show = false;
    public float progress;
    void Start()
    {
        progress = 0;
        progressSlider.maxValue = maxProgress;
    }

    // Update is called once per frame
    void Update()
    {
            progressSlider.value = progress;
    }

    public bool increaseProgress() {
        progress += incrementAmount * Time.deltaTime;
        if (progressSlider.value >= maxProgress) {
            progress = 0;
            return true;
        }
        return false;
    }
}
