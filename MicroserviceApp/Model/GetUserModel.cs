using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class GetUserModel
    {
        public User currentUser { get; set; }
        public IQueryable<string> currentUserProducts { get; set; }
    }
}
