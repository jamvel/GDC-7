using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour {
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float health;
    public float damage;
    public float waitDeathTime = 0.3f;
    public Slider healthBar;

    public int numberOfCoins = 1;
    public GameObject coin;
    private EmmiterVector emmiterCoin;

    private SpriteRenderer sr;
    private RectTransform healthbarRectTransform;
    private Text healthRatioText;
    private Animator animator;
    private bool check = false;
    private bool killEnemyCheck = false;

    private int i;
    private Rigidbody2D rb;
    private Transform tr;

    public AudioClip death;
    public AudioSource deathSound;

    void Start(){
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();
        animator = this.GetComponent<Animator>();
        healthbarRectTransform = transform.Find("Enemy_Health_Bar").gameObject.transform.Find("Fill Area").gameObject.transform.Find("Fill").gameObject.GetComponent<RectTransform>(); //find healthbar
        updateHealthBar();
        emmiterCoin = this.GetComponent<EmmiterVector>();
        deathSound = gameObject.AddComponent<AudioSource>();
        deathSound.clip = death;
        deathSound.volume = 0.1f;
    }

    void Update() {
        if(check) {
            check = false;
            if (health <= 0) {
                animator.SetBool("IsDeath", true);
                StartCoroutine(waitSeconds(waitDeathTime));

            }
        }
        if(killEnemyCheck) {
            Destroy(this.gameObject);
            Vector3 coinPos = emmiterCoin.GetComponent<Transform>().position;
            coinPos.x = coinPos.x + 0.5f;//+1
            for (int i = 0; i < numberOfCoins; i++) {
                Instantiate(coin, coinPos, emmiterCoin.GetComponent<Transform>().rotation);
                coinPos.x = coinPos.x + (0.25f * -1);
            }
        }
    }

    public void Damage(float dam) {
        check = true;
        health = health - dam/*20*/;
        healthBar.value = health;
        if (healthBar.value <= 0) {
            if (!deathSound.isPlaying){
                deathSound.Play();
            }
        }
    }

    public void updateHealthBar() {
        float healthRatio = currentHealth / maxHealth;
        healthbarRectTransform.localScale = new Vector3(healthRatio, 1, 1);
    }

    public void updateHealthBar(Damage dmg) { //updates current health and health bar
        i = Random.Range(0, 2);
        if (dmg.isRight) {
            tr.position = Vector2.Lerp(tr.position, new Vector2(tr.position.x - 0.6f, tr.position.y), 0.5f);
        } else {
            tr.position = Vector2.Lerp(tr.position, new Vector2(tr.position.x + 0.6f, tr.position.y), 0.5f);
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

    IEnumerator waitSeconds(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        killEnemyCheck = true;
    }
}
