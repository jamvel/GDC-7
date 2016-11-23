using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Respawn_Dialogue : MonoBehaviour {
    string[] dialogue= new string[3];
    
    public GameObject dialogue_panel;

    void Start() {
        dialogue[0] = "Ahoy  Matey! Me  name  be  Jack! I  was  lookin'  fer  the  treasure  too..";
        dialogue[1] = "Ye  can  never  leave  here ! Ye  must  find  and  defeat  th'  creature  with  three  heads!";
        dialogue[2] = "But  every  time  ye  die  must  choose  a  perk 'n  curse  to  continue  searchin'.  You  can  also  re-roll  to  choose  another  package. You  must  choose  to  continue.";
        if (!GameManager.instance.dialogue_UIRespawn) {
            GameManager.instance.dialogue_UIRespawn = true;
            dialogue_panel.SetActive(true);
            StartCoroutine(DialogueTimer(5,5,6, dialogue));
        }
    }

    IEnumerator DialogueTimer(float time1,float time2 , float time3 , string[] dialogue) {
        Debug.Log("here");
        dialogue_panel.transform.FindChild("Text").GetComponent<Text>().text = dialogue[0];
        yield return new WaitForSeconds(time1);
        dialogue_panel.transform.FindChild("Text").GetComponent<Text>().text = dialogue[1];
        yield return new WaitForSeconds(time2);
        dialogue_panel.transform.FindChild("Text").GetComponent<Text>().text = dialogue[2];
        yield return new WaitForSeconds(time3);
        dialogue_panel.SetActive(false);
    }
}
