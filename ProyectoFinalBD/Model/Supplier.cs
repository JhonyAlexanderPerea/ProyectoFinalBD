namespace ProyectoFinalBD.Model;

public class Supplier
{
    public string SupplierId { get; set; }
    public string Name { get; set; }
    public string Contact { get; set; }
    public string? Email { get; set; }
    public int WarrantyMonths { get; set; }

    public string? MunicipalityId { get; set; }
    public Municipality? Municipality { get; set; }
}