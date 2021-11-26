using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace CarSpeedDetection.Common.Extensions
{
    internal class VideoDevice
    {
        private readonly string _name;
        public VideoDevice(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
