using System;

/// <summary>
/// Summary description for AddProject
/// </summary>
public class AddProject
{
    public AddProject()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int BusinessID { get; set; }
    public string ProjectName { get; set; }
    public DateTime StartDate { get; set; }
    public string EndDate { get; set; }

    public string Status { get; set; }
    public int ProjectID { get; set; }
}