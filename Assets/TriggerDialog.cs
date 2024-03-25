using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialog : MonoBehaviour
{
    [SerializeField] TextAsset myDialog;
    void Start()
    {
        DialogManager.getInstance().palyDialog(myDialog);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
