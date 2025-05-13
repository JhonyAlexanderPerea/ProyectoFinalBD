namespace ProyectoFinalBD.Model;

public class Equipment
{
    public int EquipmentId { get; set; }
    public string Name { get; set; }
    public string Type { get; set; } 
    public string Status { get; set; } 
    public string Location { get; set; } 

    public int SupplierId { get; set; } 
    public Supplier Supplier { get; set; } 
}