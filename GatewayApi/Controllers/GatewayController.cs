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
    public class GatewayController : ControllerBase
    {        
        private readonly ILogger<Gateway> _logger;
        private readonly GatewayContext _context;

        public GatewayController(ILogger<Gateway> logger, GatewayContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gateway>>> Get()
        {
            return await _context.Gateways.ToListAsync();
        }
        [HttpGet("{serialNumber}/devices")]
        public async Task<ActionResult<IEnumerable<Device>>> GetGatewayDevices(String serialNumber)
        {
            var gatewayItem = await _context.Gateways.FindAsync(serialNumber);
            if (gatewayItem == null)
            {
                return Problem(statusCode: 404, title: "Gateway Not Found");
            }
            
            return await _context.Devices.Where(d => d.SerialNumber == serialNumber).ToListAsync();
        }
        [HttpGet("{serialNumber}")]
        public async Task<ActionResult<Gateway>> GetGateway(String serialNumber)
        {
            var gatewayItem = await _context.Gateways.FindAsync(serialNumber);
            if (gatewayItem == null)
            {
                return Problem(statusCode: 404, title: "Gateway Not Found");
            }

            return gatewayItem;
        }
        [HttpPost]
        public async Task<ActionResult<Gateway>> Post(Gateway gateway)
        {
            try {
                _context.Gateways.Add(gateway);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetGateway), new { serialNumber = gateway.SerialNumber }, gateway);
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
                return Problem(statusCode: 400, title: ex.Message);
            }
        }
        [HttpPost]
        [Route("add-devices")]
        public async Task<IActionResult> AddDevices(DevicesToGatewayDTO body)
        {
            try {
                var gatewayItem = await _context.Gateways.FindAsync(body.GatewaySerial);
                if (gatewayItem == null)
                {
                    return Problem(statusCode: 400, title: "Gateway Not Found");
                }
                int toAdd = body.DevicesIds.Count;
                int c = _context.Devices.Count(d => d.SerialNumber == gatewayItem.SerialNumber);
                if (c + toAdd > 10) {
                    return Problem(statusCode: 400, title: "A gateway can't have more than 10 devices");
                }
                foreach(var deviceId in body.DevicesIds) {
                    Device d = await _context.Devices.FindAsync(deviceId);                    
                    if (d == null)
                    {
                        return Problem(statusCode: 400, title: $"Device Not Found ({deviceId})");
                    }
                    d.SerialNumber = gatewayItem.SerialNumber;
                }
                await _context.SaveChangesAsync();
                return Ok("Devices added succesfully.");
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
                return Problem(statusCode: 400, title: ex.Message);
            }
        }
        [HttpPost]
        [Route("remove-devices")]
        public async Task<IActionResult> RemoveDevices(DevicesToGatewayDTO body)
        {
            try {
                var gatewayItem = await _context.Gateways.FindAsync(body.GatewaySerial);
                if (gatewayItem == null)
                {
                    return Problem(statusCode: 400, title: "Gateway Not Found");
                }
                foreach(var deviceId in body.DevicesIds) {
                    Device d = await _context.Devices.FindAsync(deviceId);                    
                    if (d == null)
                    {
                        return Problem(statusCode: 400, title: $"Device Not Found ({deviceId})");
                    }
                    d.SerialNumber = null;
                }
                await _context.SaveChangesAsync();
                return Ok("Devices removed succesfully.");
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
                return Problem(statusCode: 400, title: ex.Message);
            }
        }
        [HttpPost]
        [Route("assign-devices")]
        public async Task<IActionResult> AsignDevices(DevicesToGatewayDTO body)
        {
            try {
                var gatewayItem = await _context.Gateways.FindAsync(body.GatewaySerial);
                if (gatewayItem == null)
                {
                    return Problem(statusCode: 400, title: "Gateway Not Found");
                }
                if (body.DevicesIds.Count > 10) {
                    return Problem(statusCode: 400, title: "A gateway can't have more than 10 devices");
                }
                var devices = await _context.Devices.ToListAsync();
                foreach(var device in devices) {
                    if (body.DevicesIds.Contains(device.UID)) 
                    {
                        device.SerialNumber = body.GatewaySerial;
                    }
                    else if (device.SerialNumber == body.GatewaySerial)
                    {
                        device.SerialNumber = null;
                    }
                }
                await _context.SaveChangesAsync();
                return Ok("Devices assigned succesfully.");
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
                return Problem(statusCode: 400, title: ex.Message);
            }
        }
        [HttpDelete("{serialNumber}")]
        public async Task<ActionResult<Gateway>> Delete(String serialNumber)
        {
            try {
                var gatewayItem = await _context.Gateways.FindAsync(serialNumber);
                if (gatewayItem == null)
                {
                    return Problem(statusCode: 404, title: "Gateway Not Found");
                }
                _context.Gateways.Remove(gatewayItem);
                await _context.SaveChangesAsync();
                return Ok("Deleted Succesfully");
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
                return Problem(statusCode: 400, title: ex.Message);
            }
        }

    }
}
