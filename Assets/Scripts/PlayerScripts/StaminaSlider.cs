using UnityEngine;
using UnityEngine.UI;

public class StaminaSlider : MonoBehaviour
{
    public Slider slider;

    public float maxStamina = 100;

    public float stamina;
    public float staminaCost;
    public float staminaRecover;

    public bool sprinting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stamina = maxStamina;
        slider.maxValue = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value != stamina) {
            slider.value = stamina;
        }
        if(sprinting) 
            reduceStamina();
        else 
            gainStamina();
    }

    public void reduceStamina() {
        if (stamina > -1)
            stamina -= staminaCost * Time.deltaTime;
    }

    public void gainStamina() {
        if (stamina < maxStamina) {
            stamina += staminaRecover * Time.deltaTime;
        }
    }
}
