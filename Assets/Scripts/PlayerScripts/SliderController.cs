using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider slider;
    public float maxHP = 10;

    public float HP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HP = maxHP;
        slider.maxValue = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value != HP) {
            slider.value = HP;
        }
    }

    public void takeDamage(float damage) {
        HP -= damage;
    }
}
