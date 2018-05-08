using System;
namespace GameReward
{
public class GrdSessionData {
    public long sessionid;
    public long sessionstart;
    public System.Collections.Generic.Dictionary<string, string> values = new System.Collections.Generic.Dictionary<string, string>();
    public DateTime getTime()
    {
        DateTime date = new DateTime(1970, 1, 1, 0, 0, 0);
        date.AddSeconds(sessionstart);
        return date;
    }
}
}