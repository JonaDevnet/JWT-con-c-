using AWJWT.Context;
using AWJWT.Models;

namespace AWJWT.OpVehiculo
{
    public class VehiculosDAO
    {
        private BdjwtContext context = new BdjwtContext();

        // Agregar un vehiculos
        public bool agrgarVehiculo(Vehiculo vehiculo)
        {
            try
            {
                Vehiculo nuevoVehiculo = new Vehiculo();
                nuevoVehiculo = vehiculo; 
                if (vehiculo != null)
                {
                    context.Vehiculos.Add(vehiculo);
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex )
            {
                return false;
                throw new Exception(ex.Message);
            }
        }
        //eliminar vehiculo
        public bool EliminarCalificacion(int id)
        {
            try
            {
                var vehiculo = context.Vehiculos.Where(v => v.IdVehiculo == id).FirstOrDefault();
                if (vehiculo == null) 
                    return false; // no existe
                else
                {
                    context.Vehiculos.Remove(vehiculo);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception(ex.Message);
            }
        }
        // editar vehiculo
        public bool EditarVehiculo(int id, string tipo,string patente, string marca, string modelo, int anio)
        {
            try
            {
                var vehiculo = context.Vehiculos.Where(v => v.IdVehiculo == id).FirstOrDefault();
                if( vehiculo != null)
                {
                    vehiculo.Tipo = tipo;
                    vehiculo.Patente = patente;
                    vehiculo.Marca = marca;
                    vehiculo.Modelo = modelo;
                    vehiculo.Anio = anio;
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception(ex.Message);
            }
        }
    }
}
