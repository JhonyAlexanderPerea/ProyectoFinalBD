namespace ProyectoFinalBD.Model;

public class Equipment
{
    public string EquipmentId { get; set; }
    public string Name { get; set; }
    public decimal Cost { get; set; }
    public string Features { get; set; }

    public string? EquipmentTypeId { get; set; }
    public EquipmentType? EquipmentType { get; set; }

    public string? LocationId { get; set; }
    public Location? Location { get; set; }

    public string? EquipmentStatusId { get; set; }
    public EquipmentStatus? EquipmentStatus { get; set; }

    public string? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
}