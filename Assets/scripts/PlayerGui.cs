using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGui : MonoBehaviour
{


    [SerializeField] private Text fpsCounter;
    [SerializeField] private Player player;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //this.player.rb.velocity.magnitude;
        fpsCounter.text = "fps = " + Mathf.RoundToInt(1 / Time.deltaTime).ToString();
    }
}
