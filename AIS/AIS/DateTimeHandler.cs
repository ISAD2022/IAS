using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIS.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.Globalization;

namespace AIS
{

    public class DateTimeHandler
    {
     
        public string DateTimeInDDMMYY(DateTime dt)
        {
            return dt.ToString("dd'/'MM'/'yyyy", CultureInfo.InvariantCulture);
        }

        public string DateTimeInDDMMYYHHMMSS(DateTime dt)
        {
            return dt.ToString("dd'/'MM'/'yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
