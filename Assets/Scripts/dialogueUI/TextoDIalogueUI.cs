﻿
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;


public class TextoDIalogueUI : Yarn.Unity.DialogueUIBehaviour
{
    public FaceName[] faces;
    public GameObject dialogueContainer;
    //left character face
    public Image leftFace;
    //right character face usualy the player
    public Image rightFace;
    private Sprite CurentFace;
    public Sprite PlaceHolderFace;

    public ScrollRect Scroll;
    public RectTransform lineTextContainer;
    public RectTransform lineTextPrefab;
    public Sprite bubble;
    public Sprite actionBubble;

    /// A UI element that appears after lines have finished appearing
    public GameObject continuePrompt;

    /// A delegate (ie a function-stored-in-a-variable) that
    /// we call to tell the dialogue system about what option
    /// the user selected
    private Yarn.OptionChooser SetSelectedOption;


    /// The buttons that let the user choose an option
    public List<Button> optionButtons;

    /// Make it possible to temporarily disable the controls when
    /// dialogue is active and to restore them when dialogue ends
   // public RectTransform gameControlsContainer;

    public ExampleVariableStorage variableStorage;
    public enum BubbleType
    {
        action,
        left,
        right
    }
    private BubbleType bubbleType = BubbleType.left;
    // to refator
    private bool faceSet = false;
    void Awake()
    {
        // Start by hiding the container, line and option buttons
        if (dialogueContainer != null)
            dialogueContainer.SetActive(false);

       // lineText.gameObject.SetActive(false);

        foreach (var button in optionButtons)
        {
            button.gameObject.SetActive(false);
        }

        // Hide the continue prompt if it exists
        if (continuePrompt != null)
            continuePrompt.SetActive(false);
    }
    private void DeleteChildren(Transform parent)
    {

        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
    /// Show a line of dialogue, gradually
    public override IEnumerator RunLine(Yarn.Line line)
    {
        //default bubble to the left
        bubbleType = BubbleType.left;

        if (line.text != "Skip:")
        {
            CurentFace = PlaceHolderFace;
            faceSet = false;
            // Display the new text speach bubble
            RectTransform ob = Instantiate(lineTextPrefab, lineTextContainer) ;
            
            Text txt = ob.GetComponentInChildren<Text>() ;
            HorizontalLayoutGroup hlg = ob.GetComponent<HorizontalLayoutGroup>();
            txt.text = CheckVars(line.text);

            HorizontalLayoutGroup hlg2 = ob.transform.GetChild(0).GetComponent<HorizontalLayoutGroup>();
            switch (bubbleType)
            {
                case BubbleType.action:
                    Image img = ob.GetComponentInChildren<Image>();
                    img.sprite = actionBubble;
                    hlg.padding.right = 22;
                    hlg.padding.left = 22;        

                    break;
                case BubbleType.left:
                    hlg.padding.left = 22;
                    hlg.padding.right = 0;
                    leftFace.sprite = CurentFace;
                   
                    hlg2.padding.right = 34;

                    break;
                case BubbleType.right:

                    txt.gameObject.transform.parent.localScale = new Vector3(txt.gameObject.transform.parent.localScale.x * -1, txt.gameObject.transform.parent.localScale.y, txt.gameObject.transform.parent.localScale.z);
                    txt.gameObject.transform.localScale = new Vector3(txt.gameObject.transform.localScale.x * -1, txt.gameObject.transform.localScale.y, txt.gameObject.transform.localScale.z);

                    hlg.padding.right = 22;
                    hlg.padding.left = 0;
                    rightFace.sprite = CurentFace;
                  
                    hlg2.padding.left = 14;
                    break;
            }
   



            //cryptic voodo code do not remove 
            yield return new WaitForEndOfFrame();
            Canvas.ForceUpdateCanvases();
            //cryptic voodo code do not remove 

            //scroll to the bottom to show the current reply 
            Scroll.verticalNormalizedPosition = 0;
        
     

            // Show the 'press any key' prompt when done, if we have one
            if (continuePrompt != null)
                continuePrompt.SetActive(true);

            // Wait for any user input
            while (Input.GetKeyDown(KeyCode.Space ) == false)
            {
                yield return null;
            }
            // hide the 'press any key' prompt when done, if we have one
            if (continuePrompt != null)
                continuePrompt.SetActive(false);
        }
    }

    /// Show a list of options, and wait for the player to make a selection.
    public override IEnumerator RunOptions(Yarn.Options optionsCollection,
                                            Yarn.OptionChooser optionChooser)
    {

        // Do a little bit of safety checking
        if (optionsCollection.options.Count > optionButtons.Count)
        {
            Debug.LogWarning("There are more options to present than there are" +
                             "buttons to present them in. This will cause problems.");
        }

        // Display each option in a button, and make it visible
        int i = 0;
        foreach (var optionString in optionsCollection.options)
        {
            optionButtons[i].gameObject.SetActive(true);
            optionButtons[i].GetComponentInChildren<Text>().text = CheckVars(optionString);
            i++;
        }

        // Record that we're using it
        SetSelectedOption = optionChooser;

        // Wait until the chooser has been used and then removed (see SetOption below)
        while (SetSelectedOption != null)
        {
            yield return null;
        }

        // Hide all the buttons
        foreach (var button in optionButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    /// Called by buttons to make a selection.
    public void SetOption(int selectedOption)
    {

        // Call the delegate to tell the dialogue system that we've
        // selected an option.
        SetSelectedOption(selectedOption);

        // Now remove the delegate so that the loop in RunOptions will exit
        SetSelectedOption = null;
    }

    /// Run an internal command.
    public override IEnumerator RunCommand(Yarn.Command command)
    {
        // "Perform" the command
        // Debug.Log ("Command: " + command.text);

        yield break;
    }

    /// Called when the dialogue system has started running.
    public override IEnumerator DialogueStarted()
    {
        Debug.Log ("Dialogue starting!");
        //set the face to the players face
        rightFace.sprite = faces[0].sprite;
        // Enable the dialogue controls.
        if (dialogueContainer != null)
            dialogueContainer.SetActive(true);

        //delete the previous speach bubbles 
        DeleteChildren(lineTextContainer);

        yield break;
    }

    /// Called when the dialogue system has finished running.
    public override IEnumerator DialogueComplete()
    {
        // Debug.Log ("Complete!");

        // Hide the dialogue interface.
        if (dialogueContainer != null)
            dialogueContainer.SetActive(false);

      /*  // Show the game controls.
        if (gameControlsContainer != null)
        {
            gameControlsContainer.gameObject.SetActive(true);
        }
        */
        yield break;
    }
    /// <summary>
    /// added from  https://github.com/thesecretlab/YarnSpinner/issues/25#issuecomment-227475923  
    /// credits to https://github.com/TheSabotender 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    string CheckVars(string input)
    {
        string output = string.Empty;
        bool checkingVar = false;
        string currentVar = string.Empty;

        int index = 0;
        while (index < input.Length)
        {
            if (input[index] == '[')
            {
                checkingVar = true;
                currentVar = string.Empty;
            }
            else if (input[index] == ']')
            {
                checkingVar = false;
                output += ParseVariable(currentVar);
                currentVar = string.Empty;
            }
            else if (checkingVar)
            {
                currentVar += input[index];
            }
            else
            {
                output += input[index];
            }
            index += 1;
        }

        return output;
    }

    string ParseVariable(string varName)
    {
        foreach(FaceName f in faces)
        {

            if (varName == f.Name)
            {
                if(faceSet == false)
                {
                    CurentFace = f.sprite;
                    faceSet = true;
                  //  return variableStorage.GetValue(varName).AsString;
                }
                
            }
        }
        if (varName == "$playerName")
        {
           if(bubbleType != BubbleType.action)
                bubbleType = BubbleType.right;
            return variableStorage.GetValue(varName).AsString;
        }
        //Check YarnSpinner's variable storage first
        if (variableStorage.GetValue(varName) != Yarn.Value.NULL)
        {
            return variableStorage.GetValue(varName).AsString;
        }

        //Handle other variables here
        if (varName == "$time")
        {
            return Time.time.ToString();
        }
        
        if (varName == "action")
        {
            bubbleType = BubbleType.action;
           
      
            return "";
        }
        if (varName == "right")
        {
            bubbleType = BubbleType.right;
            return "";
        }
     
        //If no variables are found, return the variable name
        return "";
    }
    //end 
}


