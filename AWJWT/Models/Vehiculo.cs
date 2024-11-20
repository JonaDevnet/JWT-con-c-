using System;
using System.Collections.Generic;

namespace AWJWT.Models;

public partial class Vehiculo
{
    public int IdVehiculo { get; set; }

    public string? Tipo { get; set; }

    public string? Patente { get; set; }

    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public int? Anio { get; set; }
}
