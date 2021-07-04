using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GatewayApi.Models
{
    public class Gateway
    {
        private const String ipv4Regex = @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";         
        [Key]
        [Required]
        public String SerialNumber { get; set; }
        [Required]
        public String Name { get; set; }        

        [Required]
        [RegularExpression(ipv4Regex, ErrorMessage = "Invalid ipv4 address.")]
        public String Address { get; set; }        
    }
}
