using ProductsApi.Data.Entities;
using System.Text.Json;

namespace ProductsApi.Data
{
    public static class DataSeeder
    {

        public static void SeedData(ProductContext _context)
        {
            //if (!_context.Speakers.Any())
            //{
            //    _context.Speakers.AddRange(LoadSpeakers());
            //    _context.SaveChanges();
            //}
        }

        //private static List<Product> LoadSpeakers()
        //{
        //    var jsonPath = @"D:\learning\MyGit\conference-main\backend\src\Conference.Api\Conference.Domain\data.json";
        //    using (StreamReader file = File.OpenText(jsonPath))
        //    {

        //        JsonSerializer serializer = new JsonSerializer();
        //        serializer.ContractResolver = new DefaultContractResolver()
        //        {
        //            NamingStrategy = new SnakeCaseNamingStrategy()
        //        };
        //        var speakers = (List<Speaker>)serializer.Deserialize(file, typeof(List<Speaker>));
        //        return speakers;
        //    }
        }
    
}
