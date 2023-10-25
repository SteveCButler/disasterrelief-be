namespace disasterrelief_be.Models;

public class Category
{
    public int Id { get; set; }
    public string CategoryName { get; set; }
    public string Description { get; set; }
    public Item Item { get; set; }
}
