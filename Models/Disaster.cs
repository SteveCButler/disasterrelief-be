namespace disasterrelief_be.Models;

public class Disaster
{
    public int Id { get; set; }
    public string DisasterName { get; set; }
    public string? Description { get; set; }
    public string Location { get; set; }
    public int Severity { get; set; }

    public ICollection<Item> Items { get; set; }

}
