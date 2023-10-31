namespace disasterrelief_be.Models;

public class Item
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string ItemName { get; set; }
    public int Count { get; set; }

    public ICollection<Disaster> Disasters { get; set; }

}
