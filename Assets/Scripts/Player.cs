using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float mana = 100f;
    public int coins;
    public Canvas UI_Canvas;

    /*Perks & Curses*/
    public bool fireball_1; //true if ability is fireball1

    private RectTransform healthbarRectTransform;
    private Text healthRatioText;
    private Text coinCounter;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Transform tr;

    private int i;
    public AudioClip[] grunts, coin;
    public AudioSource grunt, coinsound;
    // Use this for initialization
    void Start () {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();

        healthbarRectTransform = UI_Canvas.transform.Find("HealthBar_Background").gameObject.transform.Find("HealthBar").gameObject.GetComponent<RectTransform>(); //find healthbar
        healthRatioText = UI_Canvas.transform.Find("HealthBar_Background").gameObject.transform.Find("RatioText_HealthBar").gameObject.GetComponent<Text>(); //find ratio text for health bar
        coinCounter = UI_Canvas.transform.Find("Counter_Parent").gameObject.transform.Find("Counter").gameObject.GetComponent<Text>(); //find counter text

        updateHealthBar();
        coinsound = gameObject.AddComponent<AudioSource>();
        grunt = gameObject.AddComponent<AudioSource>();
        grunt.volume = 0.1f;
    }

    public void updateHealthBar() {
        float healthRatio = currentHealth / maxHealth;
        healthbarRectTransform.localScale = new Vector3(healthRatio, 1, 1);
        healthRatioText.text = (healthRatio * 100).ToString()+"/"+maxHealth;
    }
    
    public void updateHealthBar(Damage dmg) { //updates current health and health bar
        i = Random.Range(0, 2);
        grunt.clip = grunts[i];
        grunt.Play();
        
        if (dmg.isRight) {
            tr.position = Vector2.Lerp(tr.position, new Vector2(tr.position.x - 0.6f, tr.position.y), 0.5f);
            Debug.Log("Enemy hit the player from the right");
        }
        else {
            tr.position = Vector2.Lerp(tr.position, new Vector2(tr.position.x + 0.6f, tr.position.y), 0.5f);
            Debug.Log("Enemy hit the player from the left");
        }
        currentHealth -= dmg.damage;
        updateHealthBar();
        StartCoroutine(changeSpriteColor());
        
    }

    public void updateCoinCounter(int value) { //takes increment as parameter
        coins += value;
        coinCounter.text = coins.ToString("D2");
        i = Random.Range(0, 4);
        coinsound.clip = coin[i];
        coinsound.Play();
    }


    private IEnumerator changeSpriteColor() {
        Color original = new Color(255,255,255,255);
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = original;
    }


}
