namespace SystemLibrary.Common.Web.Tests
{
    public interface IVehicle
    {
        string Name { get; set; }
    }
    public class Car : IVehicle
    {
        public string Name { get; set; }
    }
}
