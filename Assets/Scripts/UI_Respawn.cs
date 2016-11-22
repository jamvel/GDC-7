using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Respawn : MonoBehaviour {
    public static UI_Respawn instance;

    public int tier2_cost = 150;
    public int re_roll_cost = 150;

    public Canvas UI_Canvas;
    public Text cost_Tier2_Text;
    public Text cost_Reroll_Text;
    public Text errorText;

    private GameObject option1;
    private GameObject option2;

    private Button option1_button;
    private Button option2_button;
    private Text perk_text;
    private Text curse_text;
    private Text player_coins;

    public EventSystem evtSystemRef;

    int selectedId = 0;

    List<int> selectedCurses = new List<int>();

    void Awake() {
        if (instance != null && instance != this) {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
    }

    void Start() {
        option1 = UI_Canvas.transform.FindChild("Option1").gameObject;
        option2 = UI_Canvas.transform.FindChild("Option2").gameObject;

        option1_button = option1.transform.FindChild("Button_Option").GetComponent<Button>();
        option2_button = option2.transform.FindChild("Button_Option").GetComponent<Button>();

        option1_button.GetComponent<UI_Respawn_OnSelect>().perk_tier = true;
        option2_button.GetComponent<UI_Respawn_OnSelect>().perk_tier = false;

        perk_text = UI_Canvas.transform.FindChild("Left_Panel").FindChild("Perk_Text").GetComponent<Text>();
        curse_text = UI_Canvas.transform.FindChild("Left_Panel").FindChild("Curse_Text").GetComponent<Text>();
        player_coins = UI_Canvas.transform.FindChild("PlayerCoins_Text").GetComponent<Text>();

        cost_Tier2_Text.text = tier2_cost.ToString();
        cost_Reroll_Text.text = re_roll_cost.ToString();
        RandomSelect();
    }

    void RandomSelect() {
        int perk_1 = Random.Range(0, GameManager.instance.perks_tier1.Length); //select 1st perk
        option1_button.GetComponent<UI_Respawn_OnSelect>().perk_id = perk_1;
        int perk_2 = Random.Range(0, GameManager.instance.perks_tier2.Length); //select 2nd perk
        option2_button.GetComponent<UI_Respawn_OnSelect>().perk_id = perk_2;

        int curse_1 = UniqueRandomInt(0, GameManager.instance.curses.Length, selectedCurses);
        option1_button.GetComponent<UI_Respawn_OnSelect>().curse_id = curse_1;

        int curse_2 = UniqueRandomInt(0, GameManager.instance.curses.Length, selectedCurses);
        option2_button.GetComponent<UI_Respawn_OnSelect>().curse_id = curse_2;

        selectedCurses.Clear();
    }

    void Update() {
        player_coins.text = GameManager.instance.coins.ToString(); //update player count coins
        if (evtSystemRef.currentSelectedGameObject == null) {
            evtSystemRef.SetSelectedGameObject(option1_button.gameObject);
            select(0);
        }
    }
    public void select(int id) {
        selectedId = id;
        if(id == 2) {
            if(GameManager.instance.coins >= re_roll_cost) {
                GameManager.instance.coins -= re_roll_cost;
                RandomSelect();
            }else {
                errorText.gameObject.SetActive(true);
            }
            
        }
        Debug.Log(selectedId);
        //select[0] -> perk_1/curse_1...
    }
    

    public void ChangePerk(int id,bool tier) { //called by UI_Respawn_OnSelect , true - tier1 , false -tier2
        if (tier) {
            perk_text.text = GameManager.instance.perks_tier1[id].desc;
        }else {
            perk_text.text = GameManager.instance.perks_tier2[id].desc;
        }
        
    }

    public void ChangeCurse(int id) { //called by UI_Respawn_OnSelect
        curse_text.text = GameManager.instance.curses[id].desc;
    }

    public void OnSelectedReroll() { //called by UI_Respawn_OnSelect
        perk_text.text = "-";
        curse_text.text = "-";
    }

    private int UniqueRandomInt(int min, int max, List<int> usedValues) {
        int val = Random.Range(min, max);
        while (usedValues.Contains(val)) {
            val = Random.Range(min, max);
        }
        usedValues.Add(val);
        return val;
    }

}
