namespace SystemLibrary.Common.Web.HttpBaseClientTests
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
