﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Respawn : MonoBehaviour {
    public static UI_Respawn instance;

    public int tier2_cost = 150;
    public int re_roll_cost = 150;

    public Canvas UI_Canvas;
    public GameObject Loading_Panel;
    public Text cost_Tier2_Text;
    public Text cost_Reroll_Text;
    public Text errorText;
    public Image image_perk1, image_perk2, image_curse1, image_curse2;

    private GameObject option1;
    private GameObject option2;

    private int perk_1, perk_2, curse_1, curse_2;

    private Button option1_button;
    private Button option2_button;
    private Text perk_text;
    private Text curse_text;
    private Text player_coins;

    public EventSystem evtSystemRef;

    int selectedId = 0;

    public AudioClip click;
    private AudioSource badClick;

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

        badClick = gameObject.AddComponent<AudioSource>();
        badClick.clip = click;
    }

    void RandomSelect() {
        perk_1 = Random.Range(0, GameManager.instance.perks_tier1.Length); //select 1st perk
        option1_button.GetComponent<UI_Respawn_OnSelect>().perk_id = perk_1;
        perk_2 = Random.Range(0, GameManager.instance.perks_tier2.Length); //select 2nd perk
        option2_button.GetComponent<UI_Respawn_OnSelect>().perk_id = perk_2;

        image_perk1.sprite = GameManager.instance.perks_tier1[perk_1].img;
        image_perk2.sprite = GameManager.instance.perks_tier2[perk_2].img;

        curse_1 = UniqueRandomInt(0, GameManager.instance.curses.Length, selectedCurses);
        option1_button.GetComponent<UI_Respawn_OnSelect>().curse_id = curse_1;

        curse_2 = UniqueRandomInt(0, GameManager.instance.curses.Length, selectedCurses);
        option2_button.GetComponent<UI_Respawn_OnSelect>().curse_id = curse_2;

        image_curse1.sprite = GameManager.instance.curses[curse_1].img;
        image_curse2.sprite = GameManager.instance.curses[curse_2].img;

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
        if(id == 0) {
            Loading_Panel.SetActive(true);
            GameManager.instance.SetPerk(true,perk_1);
            GameManager.instance.SetCurse(curse_1);
  
            GameManager.instance.LoadLevelScene();
        }

        if (id == 1) {
            if (GameManager.instance.coins >= tier2_cost) {
                Loading_Panel.SetActive(true);
                GameManager.instance.SetPerk(false, perk_2);
                GameManager.instance.SetCurse(curse_2);

                GameManager.instance.LoadLevelScene();
            }
            else {
                StartCoroutine(ShowError());
            }

        }

        if (id == 2) {

            if (GameManager.instance.coins >= re_roll_cost) {
                GameManager.instance.coins -= re_roll_cost;
                RandomSelect();
            }else {
                if (!errorText.IsActive()) {
                    StartCoroutine(ShowError());
                }
                
            }
            
        }
        Debug.Log(selectedId);
    }
    

    public void ChangePerk(int id,bool tier) { //called by UI_Respawn_OnSelect , true - tier1 , false -tier2
        if (tier) {
            perk_text.text = GameManager.instance.perks_tier1[id].desc;
        }
        else {
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

    IEnumerator ShowError() {
        badClick.Play();
        errorText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        errorText.gameObject.SetActive(false);
    }

}
