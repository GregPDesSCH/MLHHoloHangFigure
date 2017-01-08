using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager gameManager;

    // Game Properties
    public List<string> hiddenWords;
    private string hiddenWord;
    private string currentWord;
    private byte strikePoints = 0;
    private byte strikePointStep = 1;
    private byte numberOfLives = 0;

    private int playerScore = 0;
    private int levelBonus = 0;
    private byte addPlayerScoreMultiplier = 1;

    public int pointsPerLetterThatExist = 10;
    public int bonusPointsPerPartSaved = 200;

    // Game Objects
    public GameObject[] hangFigureParts;
    public GameObject inputButtonGroup;
    private Button[] buttons;

    // UI Elements
    public Text secretWordText;
    public Text playerScoreText;
    public Text strikePointsText;
    public Text statusText;
    //public Text livesText;

	void Start()
    {
        // Set the singleton instance
        if (gameManager == null)
            gameManager = this;
        else
            Destroy(gameObject);

        buttons = inputButtonGroup.GetComponentsInChildren<Button>();

        setActiveInputButtons(false);
        statusText.text = "GAME OVER";
    }
	

    /*
        UI Functions
    */
    public void startANewGame()
    {
        playerScore = 0;
        numberOfLives = 5;
        setPlayerScoreText();
        setNewLevel();
    }

    void setSecretWordText()
    {
        secretWordText.text = "" + currentWord;
    }

    void setPlayerScoreText()
    {
        playerScoreText.text = "" + playerScore;
    }
    
    void setStrikePointsText()
    {
        strikePointsText.text = "" + strikePoints;
    }

    void setActiveInputButtons(bool active)
    {
        for (int index = 0; index < buttons.Length; index++)
            buttons[index].interactable = active;
    }

    IEnumerator displayPointsAdded(int numberOfLetterMatches)
    {
        statusText.text = pointsPerLetterThatExist * addPlayerScoreMultiplier + " * " + numberOfLetterMatches + " pts.";
        yield return new WaitForSeconds(1.5f);
        statusText.text = "";
    }

    IEnumerator displayStrike()
    {
        statusText.text = "Letter does not exist! " + strikePointStep + " strike pt" + (strikePointStep == 1 ? "." : "s.") + " added.";
        yield return new WaitForSeconds(1.5f);
        statusText.text = "";
    }


    /*
        Game Logic Functions
    */

    void setNewLevel()
    {
        strikePoints = 0;
        strikePointStep = 1;
        levelBonus = 0;
        addPlayerScoreMultiplier = 1;

        hiddenWord = hiddenWords[Random.Range(0, hiddenWords.Count)];

        currentWord = "";

        for (int hiddenWordIndex = 0; hiddenWordIndex < hiddenWord.Length; hiddenWordIndex++)
            currentWord = currentWord.Insert(0, "-");

        for (int hangFigurePartsIndex = 0; hangFigurePartsIndex < hangFigureParts.Length; hangFigurePartsIndex++)
            hangFigureParts[hangFigurePartsIndex].SetActive(false);

        setSecretWordText();
        setStrikePointsText();

        setActiveInputButtons(true);
        statusText.text = "";
    }

    bool replaceCertainLettersInCurrentWordWithLetterSelected(int letterSelected, out byte numberOfLetterMatches)
    {
        numberOfLetterMatches = 0;
        char[] currentWordArray = currentWord.ToCharArray();
        char[] hiddenWordArray = hiddenWord.ToCharArray();


        for (int hiddenWordIndex = 0; hiddenWordIndex < hiddenWordArray.Length; hiddenWordIndex++)
            if (hiddenWordArray[hiddenWordIndex] == 'a' + letterSelected)
            {
                currentWordArray[hiddenWordIndex] = (char)('a' + letterSelected);
                numberOfLetterMatches++;
            }
            else if (hiddenWordArray[hiddenWordIndex] == 'A' + letterSelected)
            {
                currentWordArray[hiddenWordIndex] = (char)('A' + letterSelected);
                numberOfLetterMatches++;
            }

        currentWord = new string(currentWordArray);

        if (numberOfLetterMatches > 0)
            return true;

        return false;
    }

    public void checkLetterSelected(int letterSelected)
    {
        byte numberOfLetterMatches;

        Debug.Log("checkLetterSelected called.");

        if (replaceCertainLettersInCurrentWordWithLetterSelected(letterSelected, out numberOfLetterMatches))
        {
            StopAllCoroutines();
            StartCoroutine(displayPointsAdded(numberOfLetterMatches));
            playerScore += pointsPerLetterThatExist * numberOfLetterMatches * addPlayerScoreMultiplier;

            addPlayerScoreMultiplier++;
            setSecretWordText();
            setPlayerScoreText();

            strikePointStep = 1;

            if (currentWord == hiddenWord)
            {
                StopAllCoroutines();
                StartCoroutine(addBonusPoints());
            }
        }
        else
        {
            displayHangFigureParts();
            
            setStrikePointsText();

            addPlayerScoreMultiplier = 1;

            if (strikePoints == 15)
            {
                currentWord = hiddenWord;
                setSecretWordText();
                setActiveInputButtons(false);

                StopAllCoroutines();
                statusText.text = "GAME OVER";
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(displayStrike());
                strikePointStep++;
            }
        }
    }

    void displayHangFigureParts()
    {
        if (strikePoints + strikePointStep >= 15)
            while (strikePoints < 15)
            {
                hangFigureParts[strikePoints].SetActive(true);
                strikePoints++;
            }
        else
        {
            int stopPoint = strikePoints + strikePointStep;
            while (strikePoints < stopPoint)
            {
                hangFigureParts[strikePoints].SetActive(true);
                strikePoints++;
            }
        }
    }

    IEnumerator addBonusPoints()
    {
        setActiveInputButtons(false);
        statusText.text = "Level Complete";
        yield return new WaitForSeconds(1f);
        

        levelBonus = bonusPointsPerPartSaved * (hangFigureParts.Length - strikePoints);

        for (int index = strikePoints; index < hangFigureParts.Length; index++)
        {
            hangFigureParts[index].SetActive(true);
            playerScore += bonusPointsPerPartSaved;
            setPlayerScoreText();
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);
        setNewLevel();
    }

   
}
