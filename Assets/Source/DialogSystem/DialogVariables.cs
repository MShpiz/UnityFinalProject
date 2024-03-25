using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;


public class DialogVariables
{
    public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }
    private Story _variablesStory;

    

    public DialogVariables(TextAsset loadGlobalsJSON)
    {
        variables = new Dictionary<string, Ink.Runtime.Object>();
        _variablesStory = new Story(loadGlobalsJSON.text);

        foreach (string name in _variablesStory.variablesState)
        {
            Ink.Runtime.Object value = _variablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
        }
    }

    public void Observe(Story story)
    {
        SaveVariablesToInk(story);
        story.variablesState.variableChangedEvent += onVariableChanged;
    }

    public void StopObserving(Story story)
    {
        try
        {
            story.variablesState.variableChangedEvent -= onVariableChanged;
        } catch (Exception e)
        {
            Debug.LogException(e);
        }
        
    }

    private void onVariableChanged(string name, Ink.Runtime.Object value)
    {
        if (variables.ContainsKey(name))
        {
            variables[name] = value;
            
        }
    }

    private void SaveVariablesToInk(Story story)
    {
        foreach (KeyValuePair<string, Ink.Runtime.Object> var in variables)
        {
            story.variablesState.SetGlobal(var.Key, var.Value);
        }
    }

}
