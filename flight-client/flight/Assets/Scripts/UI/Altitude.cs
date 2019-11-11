using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fighter;
public class Altitude : MonoBehaviour
{
    public TextMeshProUGUI ALTText;
    public GameObject Player;
    private float ALT;
    private FighterController PlayerController;
    // Start is called before the first frame update
    void Start()
    {
        PlayerController = Player.GetComponent<FighterController>();
    }

    // Update is called once per frame
    void Update()
    {
        ALT = PlayerController.altitude;
        ALTText.text = ALT.ToString();
    }
}

