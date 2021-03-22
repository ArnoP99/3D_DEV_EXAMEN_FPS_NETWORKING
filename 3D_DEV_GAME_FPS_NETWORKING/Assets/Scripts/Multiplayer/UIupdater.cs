using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class UIupdater : MonoBehaviour
{
    Text score;
    private int scoreCounter = 0;
    private void Start()
    {
        score = GameObject.Find("numCollected").GetComponent<Text>();
    }
    [PunRPC]
    public void updateScore(string standardString)
    {
        scoreCounter++;
        score.text = scoreCounter.ToString() + standardString;
    }
}
