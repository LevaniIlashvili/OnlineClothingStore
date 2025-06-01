using System.Data;

namespace OnlineClothingStore.Application
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
