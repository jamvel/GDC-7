using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour {
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float health;
    public float damage;
    //public Canvas UI_Canvas;

    private SpriteRenderer sr;
    private RectTransform healthbarRectTransform;
    private Text healthRatioText;

    private int i;
    private Rigidbody2D rb;
    private Transform tr;

    void Start(){
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();
        healthbarRectTransform = transform.Find("Enemy_Health_Bar").gameObject.transform.Find("Fill Area").gameObject.transform.Find("Fill").gameObject.GetComponent<RectTransform>(); //find healthbar
        //healthRatioText = UI_Canvas.transform.Find("HealthBar_Background").gameObject.transform.Find("RatioText_HealthBar").gameObject.GetComponent<Text>(); //find ratio text for health bar
        updateHealthBar();

    }

    public void updateHealthBar() {
        float healthRatio = currentHealth / maxHealth;
        healthbarRectTransform.localScale = new Vector3(healthRatio, 1, 1);
    }

    public void updateHealthBar(Damage dmg) { //updates current health and health bar
        i = Random.Range(0, 2);
        //grunt.clip = grunts[i];
        //grunt.Play();

        if (dmg.isRight) {
            tr.position = Vector2.Lerp(tr.position, new Vector2(tr.position.x - 0.6f, tr.position.y), 0.5f);
            Debug.Log("Player hit the enemy from the right");
        } else {
            tr.position = Vector2.Lerp(tr.position, new Vector2(tr.position.x + 0.6f, tr.position.y), 0.5f);
            Debug.Log("Player hit the enemy from the left");
        }
        currentHealth -= dmg.damage;
        updateHealthBar();
        StartCoroutine(changeSpriteColor());

    }

    private IEnumerator changeSpriteColor() {
        Color original = new Color(255, 255, 255, 255);
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = original;
    }

    public static void killEnemy(Enemy enemy) {
        Destroy(enemy.gameObject);
    }
}
