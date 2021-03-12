namespace Rasberry.Api.Excel
{
    public class TempData
    {
        public TempData(string name, double temp)
        {
            Name = name;
            Temperature = temp;
        }
        public string Name { get; set; }
        public double Temperature { get; set; }
    }
}
