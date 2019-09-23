using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class NameGenerator  
{
    public static string[] Firstnames = new string[]
    {
        "mun", "bo", "fil", "pan", "bal", "ad", "wil", "sal", 
        "her", "gor", "mar", "cel", "din", "dro"
    };

    public static string[] Lastnames = new string[]
    {
        "bo", "good", "row", "wich", "hand", "fin", "ton", "cot",
        "light", "two", "foot", "green" 
    };


    public static string GenerateCharacterName()
    {
        var firstname = "";
        var firstnameCount = UnityEngine.Random.Range(2, 4);
        for (var i = 0; i < firstnameCount; i++)
        {
            firstname += Firstnames[Random.Range(0, Firstnames.Length)];
        }

        var lastname = "";
        var lastnameCount = UnityEngine.Random.Range(2, 4);
        for (var i = 0; i < lastnameCount; i++)
        {
            lastname += Lastnames[Random.Range(0, Lastnames.Length)];
        }

        return string.Format("{0} {1}", firstname.ToUppercaseFirst(), lastname.ToUppercaseFirst());
    }   
}
