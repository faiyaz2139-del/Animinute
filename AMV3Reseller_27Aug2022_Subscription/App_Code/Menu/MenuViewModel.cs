using System.Collections.Generic;

public class MenuViewModel
{
    public int Id { get; set; }
    public int Seq { get; set; }
    public string Name { get; set; }
    public string Href { get; set; }
    public string Icon { get; set; }
    public IList<MenuViewModel> SubMenu { get; set; }

}