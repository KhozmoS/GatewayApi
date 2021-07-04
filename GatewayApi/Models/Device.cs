using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace GatewayApi.Models
{
    public class Device
    {           
        [Key]
        public String UID { get; set; }
        [Required]
        public String Vendor { get; set; }

        public DateTime Created { get; set; }
        private string statusValue = "offline";
        [Required]
        public String Status
        { 
            get {
                return statusValue;
            }
            set {
                if (value == "offline" || value == "online")
                {
                    statusValue = value;
                }
                else 
                {
                    throw new ArgumentException("Invalid status.");
                }
            }
        }
        
        [ForeignKey("Gateway")]
        [JsonIgnore]
        public String SerialNumber { get; set; }
    }
}


