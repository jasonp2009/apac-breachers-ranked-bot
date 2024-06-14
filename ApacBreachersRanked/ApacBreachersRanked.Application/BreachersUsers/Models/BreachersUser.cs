namespace ApacBreachersRanked.Application.BreachersUsers.Models;

public class BreachersUser
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string ClanTag { get; set; }
    public string FullUserName
    {
        get
        {
            string fullUserName = "";
            fullUserName += !string.IsNullOrWhiteSpace(ClanTag)
                ? $"[{ClanTag}]"
                : "";
            fullUserName += UserName;
            return fullUserName;
        }
    }
}
