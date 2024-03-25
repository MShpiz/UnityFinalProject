using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;
using System;
using Ink.UnityIntegration;

public class DialogManager : MonoBehaviour
{
    private static DialogManager instance;

    private Story currentStory;
    private bool _isDialogPlaying = false;
    private DialogVariables _dialogVariables;

    [Header("Dialog UI objects")]
    [SerializeField] private GameObject _dialogArea;
    [SerializeField] private TextMeshProUGUI _dialogText;
    [SerializeField] private TextMeshProUGUI _nameText;
    

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choiceBtns;
    private TextMeshProUGUI[] choiceText;
    public Func<int> onDialogStart; 
    public Func<int> onDialogEnd;

    [Header("VariableFiles")]
    [SerializeField] private TextAsset _variables;

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

        _dialogVariables.Observe(currentStory);

        _isDialogPlaying = true;
        Debug.Log(_isDialogPlaying);
        try
        {
            onDialogStart.Invoke();
        } 
        catch (NullReferenceException e)
        {
            Debug.LogException(e);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        _dialogArea.SetActive(true);
       
    }

     void playNextDialogLine()
    {
        if (currentStory.canContinue)
        {            
            _dialogText.text = currentStory.Continue();
            handleDialogTags(currentStory.currentTags);
            displayChoices();
        }
        else
        {
            finnishDialog();
        }
    }

    void finnishDialog()
    {
         clearDialogArea();
            try
            {
                onDialogEnd.Invoke();
            }
            catch (NullReferenceException e)
            {
                Debug.LogException(e);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            _dialogVariables.StopObserving(currentStory);
        currentStory = null;
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
        _dialogVariables = new DialogVariables(_variables);
    }

    private void Start()
    {
        choiceText = new TextMeshProUGUI[choiceBtns.Length];
        int index = 0;
        foreach (GameObject choice in choiceBtns)
        {
            try
            {
                choiceText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            } catch (NullReferenceException e)
            {
                Debug.LogException(e);
                continue;
            } catch (Exception e)
            {
                Debug.LogException(e);
                continue;
            }
            index++;
        }

    }

    void displayChoices()
    {
        List<Choice> currChoice = currentStory.currentChoices;
        if (currChoice.Count > choiceText.Length)
        {
            Debug.LogError("not enough choice buttons");
        }

        int idx = 0;
        for (; idx < currChoice.Count && idx < choiceText.Length; ++idx)
        {
            choiceBtns[idx].gameObject.SetActive(true);
            choiceText[idx].text = currChoice[idx].text;
        }

        for (; idx < choiceText.Length; ++idx)
        {
            choiceBtns[idx].gameObject.SetActive(false);
        }
    }

    public Ink.Runtime.Object getDialogVariable(string name)
    {
        Ink.Runtime.Object result;
        _dialogVariables.variables.TryGetValue(name, out result);
        if (result == null)
        {
            Debug.LogError($"no such variable {name}");
        }
        return result;
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
