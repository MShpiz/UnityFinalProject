using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class DialogManager : MonoBehaviour
{
    private static DialogManager instance;

    private Story currentStory;
    private bool _isDialogPlaying = false;

    [SerializeField] private GameObject _dialogArea;
    [SerializeField] private TextMeshProUGUI _dialogText;
    [SerializeField] private TextMeshProUGUI _nameText;

    public static DialogManager getInstance()
    {
        
        return instance;
    }
    public void palyDialog(TextAsset JSONStory)
    {
        
        if (_isDialogPlaying)
        {
            
            Debug.Log("trying to start a new dialog while current one is playing");
            throw new InvalidOperationException("other dialog currently playing");
        }
        currentStory = new Story(JSONStory.text);
        
        _isDialogPlaying = true;
        Debug.Log(_isDialogPlaying);
        _dialogArea.SetActive(true);
       
    }

     void playNextDialogLine()
    {
        if (currentStory.canContinue)
        {            
            _dialogText.text = currentStory.Continue();
            handleDialogTags(currentStory.currentTags);
        }
        else
        {
            clearDialogArea();
        }
    }

    void handleDialogTags(List<String> tags)
    {
        foreach(string tag in tags){
            string[] tagTuple = tag.Split(':');
            if (tagTuple.Length != 2)
            {
                Debug.LogError($"{tag} not a tag");
                // throw new ArgumentException($"{tag} not a tag");
            }
            string value = tagTuple[1].Trim();
            switch (tagTuple[0].Trim())
            {
                case DialogTags.speaker_tag:
                    _nameText.text = value;
                    break;
                default:
                    Debug.LogWarning($"Unknown tag {tagTuple[0]}");
                    break;
            }

        }
    }

    void clearDialogArea()
    {
        Debug.Log("clearDialogArea");
        _dialogText.text = "";
        _nameText.text = "";
        _isDialogPlaying = false;
        _dialogArea.SetActive(false);
    }

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("to many instances of DialogManager");
        }
        instance = this;
        if (_dialogText == null)
        {
            Debug.LogError("_dialogText not initialized");
            throw new ArgumentNullException("_dialogText not initialized");
        }

        if (_nameText == null)
        {
            Debug.LogError("_nameText not initialized");
            throw new ArgumentNullException("_nameText not initialized");
        }
        currentStory = null;
        clearDialogArea();
    }

    private void Start()
    {
        
    }



    private int cnt = 0;
    void Update()
    {
        if (_isDialogPlaying && cnt % 1000 == 0)
        {
            playNextDialogLine();
        }
        cnt += 1;
    }
}
