using Microsoft.VisualStudio.TestTools.UnitTesting;
using GatewayApi.Controllers;
using System.Net.Http;
using Newtonsoft.Json;
using GatewayApi.Models;
using GatewayApi.DTOS;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace GatewayApi.Tests
{
    class Ipv4ErrorObject
    {
        public String [] Address { get; set; }        
    }
    class InvalidIpv4Error
    {
        public Ipv4ErrorObject errors { get; set; }
    }
    class Problem 
    {
        public string title { get; set; }
    }
    [TestClass]
    public class GatewayApiTester
    {
        private readonly HttpClient client = new HttpClient();
        private readonly string baseUrl = "http://localhost:5000";
        [TestMethod]
        public void CreateValidGateway()
        {            
            var data = new Gateway{
                Address = "10.0.0.0",
                Name = "test1",
                // JUST TO HAVE A UNIQUE STRING (SERIAL NUMBER IS NOT NECESSARY A GUID)
                SerialNumber = Guid.NewGuid().ToString()
            };

            var json = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var resp = client.PostAsync($"{baseUrl}/gateway", stringContent);
            if (resp.Result.StatusCode != System.Net.HttpStatusCode.Created) {
                Assert.IsTrue(false, "Failed to add gateway.");
                return;
            }
            var res = resp.Result.Content.ReadAsStringAsync();            
            var gateway = JsonConvert.DeserializeObject<Gateway>(res.Result);
            Assert.IsTrue(data.SerialNumber == gateway.SerialNumber, "Failed to add gateway.");
        }
        [TestMethod]
        public void CreateInvalidGateway()
        {
            var invalidAdress = new Gateway{
                Address = "10.0.0.0.0",
                Name = GenerateRandomString(15),
                // JUST TO HAVE A UNIQUE STRING (SERIAL NUMBER IS NOT NECESSARY A GUID)
                SerialNumber = Guid.NewGuid().ToString()
            };

            var json = JsonConvert.SerializeObject(invalidAdress);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var resp = client.PostAsync($"{baseUrl}/gateway", stringContent);            
            if (resp.Result.StatusCode == System.Net.HttpStatusCode.Created) {
                Assert.IsTrue(false, "Incorrect gateway created.");
                return;
            }
            var res = resp.Result.Content.ReadAsStringAsync();            
            var errorResp = JsonConvert.DeserializeObject<InvalidIpv4Error>(res.Result);
            Assert.IsTrue(errorResp.errors.Address[0] == "Invalid ipv4 address.", "Incorrect error type.");
        }
        [TestMethod]
        public void CreateValidDevice()
        {            
            var resp = CreateRandomDevice();
            if (resp.Result.StatusCode != System.Net.HttpStatusCode.Created) {
                Assert.IsTrue(false, "Failed to add device.");
                return;
            }
            var res = resp.Result.Content.ReadAsStringAsync();            
            var gateway = JsonConvert.DeserializeObject<Device>(res.Result);
            Assert.IsTrue(gateway.Vendor != null, "Failed to add device.");
        }
        // THIS TEST METHOD IMPLICITY TEST POST DEVICE & POST GATEWAY TOO
        [TestMethod]
        public void Add11DevicesToGateway()
        {
            Gateway gateway = JsonConvert.DeserializeObject<Gateway>(
                                CreateRandomGateway().Result.Content.ReadAsStringAsync().Result
                              );
            List <String> devices = new List<String>();
            for (int i = 0; i < 11; i++)
            {
                Device device = JsonConvert
                                    .DeserializeObject<Device>(
                                        CreateRandomDevice().Result.Content.ReadAsStringAsync().Result
                                    );
                devices.Add(device.UID.ToString());
            }
            var data = new DevicesToGatewayDTO{
                GatewaySerial = gateway.SerialNumber,
                DevicesIds = devices
            };
            var json = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var resp = client.PostAsync($"{baseUrl}/gateway/add-devices", stringContent);
            // STATUS CODE CAN'T BE OK
            if (resp.Result.StatusCode == System.Net.HttpStatusCode.OK) {
                Assert.IsTrue(false, "Added more than 10 devices to a gateway.");
                return;
            }
            var res = resp.Result.Content.ReadAsStringAsync();            
            var errorResponse = JsonConvert.DeserializeObject<Problem>(res.Result);
            Assert.IsTrue(errorResponse.title == "A gateway can't have more than 10 devices", "Incorrect error type.");
        }
        [TestMethod]
        public void Add10DevicesToGateway()
        {
            Gateway gateway = JsonConvert.DeserializeObject<Gateway>(
                                CreateRandomGateway().Result.Content.ReadAsStringAsync().Result
                              );
            List <String> devices = new List<String>();
            for (int i = 0; i < 10; i++)
            {
                Device device = JsonConvert
                                    .DeserializeObject<Device>(
                                        CreateRandomDevice().Result.Content.ReadAsStringAsync().Result
                                    );
                devices.Add(device.UID.ToString());
            }
            var data = new DevicesToGatewayDTO{
                GatewaySerial = gateway.SerialNumber,
                DevicesIds = devices
            };
            var json = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var resp = client.PostAsync($"{baseUrl}/gateway/assign-devices", stringContent);
            // STATUS CODE CAN'T BE OK
            if (resp.Result.StatusCode == System.Net.HttpStatusCode.OK) 
            {
                Assert.IsTrue(true);
            }
            else 
            {
                Assert.IsTrue(false, "Request must be completed without errors");
            }
        }
        private String GenerateRandomString(int sz) 
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }
        private Task<System.Net.Http.HttpResponseMessage> CreateRandomGateway() 
        {
            var invalidAdress = new Gateway{
                Address = "10.0.0.0",
                Name = GenerateRandomString(15),
                // JUST TO HAVE A UNIQUE STRING (SERIAL NUMBER IS NOT NECESSARY A GUID)
                SerialNumber = Guid.NewGuid().ToString()
            };

            var json = JsonConvert.SerializeObject(invalidAdress);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var resp = client.PostAsync($"{baseUrl}/gateway", stringContent);
            return resp;
        }
        private Task<System.Net.Http.HttpResponseMessage> CreateRandomDevice()
        {
            var data = new DeviceDTO{
                Vendor = GenerateRandomString(10)
            };

            var json = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var resp = client.PostAsync($"{baseUrl}/device", stringContent);
            return resp;
        }
    }
}
