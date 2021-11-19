using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plays : MonoBehaviour
{

    public int[][] plays;
    public bool isO;
    // Start is called before the first frame update
    void Start()
    {

        isO = GameSystemManager.isO ? true : false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReplayPlays()
    {

        for(int i = 0; i > GameSystemManager.instance.gameButtons.Length; i++)
        {

        }

    }

    public void CheckPlays()
    {

    }

}
