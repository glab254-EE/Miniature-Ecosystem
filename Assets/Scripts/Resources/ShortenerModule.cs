using System;

public class Rounderer{
    public string ToRoundedString(float current){
        string output = current.ToString();
        if (current<1000000000&&current>100000){
            output = Math.Round(current/1000000,2).ToString()+"M";
        } else if (current<1000000&&current>1000){
            output = Math.Round(current/1000,2).ToString()+"K";
        } else {
            output = Math.Round(current,2).ToString();
        }
        return output;
    }
}