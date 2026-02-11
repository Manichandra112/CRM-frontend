using System.Threading.Tasks;

namespace CRM_Backend.Data.Seed
{
    public interface IBootstrapSeeder
    {
        Task SeedAsync();
    }

}
