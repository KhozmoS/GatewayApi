using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GatewayApi.DTOS
{
    public class DeviceDTO
    {        
        [Required]        
        public String Vendor { get; set; }        
    }
}
