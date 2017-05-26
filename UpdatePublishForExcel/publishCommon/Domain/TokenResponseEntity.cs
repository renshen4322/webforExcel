using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace publishCommon
{
  public class TokenResponseEntity
    {
        public string Access_token { get; set; }
        public string Token_type { get; set; }

        public string UserName { get; set; }
    }
}
