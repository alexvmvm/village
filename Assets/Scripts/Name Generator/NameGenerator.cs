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

    public static string GenerateLastName()
    {
        var lastname = "";
        var lastnameCount = UnityEngine.Random.Range(2, 4);
        for (var i = 0; i < lastnameCount; i++)
        {
            var index = Random.Range(0, Lastnames.Length);
            lastname += Lastnames[index];
        }

        return lastname.ToUppercaseFirst();
    }

    public static string GenerateFirstName()
    {
        var firstname = "";
        var firstnameCount = UnityEngine.Random.Range(2, 4);
        for (var i = 0; i < firstnameCount; i++)
        {
            var index = Random.Range(0, Firstnames.Length);
            firstname += Firstnames[index];
        }
        return firstname.ToUppercaseFirst();
    }

    public static string GenerateCharacterName()
    {
        return string.Format("{0} {1}", GenerateFirstName(), GenerateLastName());
    }   
}
