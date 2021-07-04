using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GatewayApi.DTOS
{
    public class DevicesToGatewayDTO
    {        
        [Required]        
        public String GatewaySerial { get; set; }

        [Required]
        public List<String> DevicesIds { get; set; }
    }
}
