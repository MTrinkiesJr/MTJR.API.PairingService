using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTJR.API.PairingService.Model
{
    public class KeyWrapper
    {
        public string Key { get; set; }

        public KeyWrapper(string key)
        {
            Key = key;
        }
    }
}
