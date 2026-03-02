using Masofa.Common.Models.SystemCrical;

namespace Masofa.Cli.DevopsUtil.Commands.Weather
{
    public class EraGribImportCommand : IBaseCommand
    {

        public Task Execute()
        {
            return Task.FromResult(0);
            //Console.WriteLine("Pls entre grid file path");
            //var filePath = Console.ReadLine();
            //using (GribFile file = new GribFile(filePath))
            //{
            //    GribMessage msg = file.First();

            //    Console.WriteLine("Grid Type: " + msg.GridType);

            //    double latInDegrees = msg["latitudeOfFirstGridPoint"].AsDouble();
            //    // GribApi.NET normalizes the coordinate values to degrees. This follows the best practice advised by ECMWF.

            //    // values are also accessible as strings
            //    Console.WriteLine("latitudeOfFirstGridPointInDegrees = " + msg["latitudeOfFirstGridPoint"].AsString());
            //}
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }



        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
