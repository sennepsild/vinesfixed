using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lsystem : MonoBehaviour {


    string axiom = "A";
    List<Rule> rules = new List<Rule>();

    public string sentence;

    private void Awake()
    {
        rules.Add(new Rule("B", ""));
        rules.Add(new Rule("A", "DE"));
        rules.Add(new Rule("E", "D"));
        rules.Add(new Rule("D", "C"));
        rules.Add(new Rule("C", "B"));
        rules.Add(new Rule("B", "A"));
        


        sentence = axiom;

    }



    public void Generate()
    {
        string newSentence = "";

        

        for (int i = 0; i < sentence.Length; i++)
        {
            bool ruleFound = false;
            for (int j = 0; j < rules.Count; j++)
            {


                if (sentence[i] == rules[j].inputChar[0])
                {
                    newSentence = newSentence + rules[j].outputChar;
                    ruleFound = true;
                }

                
            }
            if (!ruleFound)
            {
                newSentence = newSentence + sentence[i];
            }



        }

        sentence = newSentence;

    }


}


struct Rule
{
    public string inputChar;
    public string outputChar;

    public Rule(string inputChar, string outputChar)
    {
        this.inputChar = inputChar;
        this.outputChar = outputChar;
    }
}
