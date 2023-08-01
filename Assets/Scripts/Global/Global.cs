using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    public class Global_Rank
    {
        public string mem_name;
        public int g_point;
        public Global_Rank(string _mem_name, int _g_point)
        {
            mem_name = _mem_name;
            g_point = _g_point;
        }
    }
    public class Global_Notice
    {
        public string title;
        public string contents;
        public string date;
        public Global_Notice(string _title, string _contents, string _date)
        {
            title = _title;
            contents = _contents;
            date = _date;
        }
    }

    public static float time;
    public static int Diamond;
    public static int Glod;
    public static int Gload_bar1;
    public static int Gload_bar2;
    public static int Stone;
    public static int Skeloton;
    public static int Emo1;
    public static int Emo2;
    public static int Emo3;

    public static bool pointState = true; 

    public static List<Global_Rank> ranks = new List<Global_Rank>();
    public static List<Global_Notice> notices = new List<Global_Notice>();

    public static string api_domain = "http://54.159.178.176:8080/game/api_ready.php";
    public static string mode_login = "login";
    public static string mode_getinfo = "getinfo";
    public static string mode_getrank = "getrank-test";
    public static string mode_getnotice = "getnotice";
    public static string mode_regpoint = "endgame";
    public static string mode_startgame = "startgame";
    public static string api_key = "e795a57ee540c3d4c2941d9ca40238abd908ad3a3163277c90c1c6a563be3a0";
    public static string api_date = "";
    public static string email;

    
}
