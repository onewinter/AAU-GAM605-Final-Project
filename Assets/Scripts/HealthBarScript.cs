using UnityEngine;

// healthbar script that works based on resizing sprite with bar fill bg; can be inverted to fill instead of decrease
public class HealthBarScript : MonoBehaviour
{
    [SerializeField] private bool invertBar;
    
    private SpriteRenderer healthBarSprite;

    void Awake()
    {
        healthBarSprite = GetComponentInChildren<SpriteRenderer>();
    }

    // update the health bar based on health/maxhealth pct
    public void UpdateHealthBar(int health, int maxHealth)
    {
        Color healthBarColor;
        
        float healthPercentage = (float) health / maxHealth;
        if (invertBar) healthPercentage = 1f - healthPercentage;
        // ensure the percentage is always between 0 and 1
        healthPercentage = Mathf.Clamp(healthPercentage, 0, 1);

        // change color based on pct left
        if (healthPercentage <= .33)
            healthBarColor = Color.red;
        else if (healthPercentage <= .66)
            healthBarColor = Color.yellow;
        else
            healthBarColor = Color.green;

        // set healthbar size and color
        transform.localScale = new Vector3(healthPercentage, 1, 1);
        if(healthBarSprite) healthBarSprite.color = healthBarColor;

    }

}
