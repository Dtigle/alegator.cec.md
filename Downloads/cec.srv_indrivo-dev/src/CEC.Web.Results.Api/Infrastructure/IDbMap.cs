using System.Data;

namespace CEC.Web.Results.Api.Infrastructure
{
    public interface IDbMap
    {

        void Map(IDataReader reader);
    }
}
