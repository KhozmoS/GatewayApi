using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GatewayApi.Models;
using Microsoft.EntityFrameworkCore;
using GatewayApi.DTOS;

namespace GatewayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceController : ControllerBase
    {        
        private readonly ILogger<Device> _logger;
        private readonly GatewayContext _context;

        public DeviceController(ILogger<Device> logger, GatewayContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Device>>> Get()
        {
            return await _context.Devices.ToListAsync();
        }
        [HttpGet("{uid}")]
        public async Task<ActionResult<Device>> GetDevice(String uid)
        {
            var deviceItem = await _context.Devices.FindAsync(uid);
            if (deviceItem == null)
            {
                return Problem(statusCode: 404, title: "Device Not Found");
            }

            return deviceItem;
        }
        [HttpPost]
        public async Task<ActionResult<Device>> Post(DeviceDTO device)
        {
            try {
                Device newDevice = new Device{
                  UID = Guid.NewGuid().ToString(),
                  Vendor = device.Vendor,
                  Status = "offline",
                  Created = DateTime.Now
                };
                _context.Devices.Add(newDevice);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetDevice), new { uid = newDevice.UID }, newDevice);
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
                return Problem(statusCode: 400, title: ex.Message);
            }
        }
    }
}
