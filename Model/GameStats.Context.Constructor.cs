using System.Data.Common;

namespace Model
{
    public partial class Entities
    {
        public Entities(DbConnection connection)
            : base(connection, false)
        {
        }
    }
}