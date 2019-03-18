﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VKVideoDownloader
{
    class InterNet
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        public static bool IsConnected
        {
            get
            {
                int description;
                return InternetGetConnectedState(out description, 0);
            }
        }
    }
}
