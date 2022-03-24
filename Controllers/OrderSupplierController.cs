using ClosedXML.Excel;
using InvictaPartnersAPI.Exceptions;
using InvictaPartnersAPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaPartnersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderSupplierController : ControllerBase
    {
        private readonly ISupplierService _service;
        private readonly ILogger<OrderSupplierController> _logger;
        private readonly ICosmosDbService _cosmosDbService;

        public OrderSupplierController(ISupplierService service, ILogger<OrderSupplierController> logger, ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
            _logger = logger;
            _service = service;
        }

        [Authorize]
        [HttpGet("{supplierId}")]
        public async Task<IActionResult> GetOrdersExcelFile(int supplierId)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            _logger.LogInformation("Updating inventory");
            var jti = GetIdFromToken();
            Models.User _user = await _cosmosDbService.GetUserAsync(jti);
            if (_user.firstName == "LEGSONS" && supplierId == 8)
            {
                _logger.LogInformation("Service starting, execution performed by LEG&SONS");
                var list = await _service.GettingExcelOrders(supplierId);
                _logger.LogInformation("Creating xlsx file");
                using (MemoryStream stream = new MemoryStream())
                {
                    var workbook = new XLWorkbook();
                    var cont = 2;
                    var worksheet = workbook.Worksheets.Add("Cancellation Orders");
                    worksheet.Cell("A1").Value = "ID";
                    worksheet.Cell("A1").Style.Font.Bold = true;
                    worksheet.Cell("B1").Value = "OrderNumber";
                    worksheet.Cell("B1").Style.Font.Bold = true;
                    worksheet.Cell("C1").Value = "EntryID";
                    worksheet.Cell("C1").Style.Font.Bold = true;
                    worksheet.Cell("D1").Value = "Status";
                    worksheet.Cell("D1").Style.Font.Bold = true;
                    worksheet.Cell("E1").Value = "ItemLookupCode";
                    worksheet.Cell("E1").Style.Font.Bold = true;
                    worksheet.Cell("F1").Value = "SimpleProdLineNo";
                    worksheet.Cell("F1").Style.Font.Bold = true;
                    worksheet.Cell("G1").Value = "isPendingForwarding";
                    worksheet.Cell("G1").Style.Font.Bold = true;
                    worksheet.Cell("H1").Value = "wasForwarded";
                    worksheet.Cell("H1").Style.Font.Bold = true;
                    worksheet.Cell("I1").Value = "RealQty";
                    worksheet.Cell("I1").Style.Font.Bold = true;
                    worksheet.Cell("J1").Value = "QtyOrdered";
                    worksheet.Cell("J1").Style.Font.Bold = true;
                    worksheet.Cell("K1").Value = "QtyCancelled";
                    worksheet.Cell("K1").Style.Font.Bold = true;
                    worksheet.Cell("L1").Value = "QtyRefunded";
                    worksheet.Cell("L1").Style.Font.Bold = true;
                    worksheet.Cell("M1").Value = "QtyShipped";
                    worksheet.Cell("M1").Style.Font.Bold = true;
                    worksheet.Cell("N1").Value = "City";
                    worksheet.Cell("N1").Style.Font.Bold = true;
                    worksheet.Cell("O1").Value = "Country";
                    worksheet.Cell("O1").Style.Font.Bold = true;
                    worksheet.Cell("P1").Value = "Region";
                    worksheet.Cell("P1").Style.Font.Bold = true;
                    worksheet.Cell("Q1").Value = "PostCode";
                    worksheet.Cell("Q1").Style.Font.Bold = true;
                    worksheet.Cell("R1").Value = "Street";
                    worksheet.Cell("R1").Style.Font.Bold = true;
                    worksheet.Cell("S1").Value = "Street2";
                    worksheet.Cell("S1").Style.Font.Bold = true;
                    worksheet.Cell("T1").Value = "Telephone";
                    worksheet.Cell("T1").Style.Font.Bold = true;
                    worksheet.Cell("U1").Value = "ContactEmail";
                    worksheet.Cell("U1").Style.Font.Bold = true;
                    worksheet.Cell("V1").Value = "FirstName";
                    worksheet.Cell("V1").Style.Font.Bold = true;
                    worksheet.Cell("W1").Value = "LastName";
                    worksheet.Cell("W1").Style.Font.Bold = true;
                    worksheet.Cell("X1").Value = "RegionID";
                    worksheet.Cell("X1").Style.Font.Bold = true;
                    foreach (Models.Excel xl in list)
                    {
                        worksheet.Cell($"A{cont}").Value = xl.ID;
                        worksheet.Cell($"B{cont}").Value = xl.OrderNumber;
                        worksheet.Cell($"C{cont}").Value = xl.EntryID;
                        worksheet.Cell($"D{cont}").Value = xl.Status;
                        worksheet.Cell($"E{cont}").Value = xl.ItemLookupCode;
                        worksheet.Cell($"F{cont}").Value = xl.SimpleProdLineNo;
                        worksheet.Cell($"G{cont}").Value = xl.isPendingForwarding;
                        worksheet.Cell($"H{cont}").Value = xl.wasForwarded;
                        worksheet.Cell($"I{cont}").Value = xl.RealQty;
                        worksheet.Cell($"J{cont}").Value = xl.QtyOrdered;
                        worksheet.Cell($"K{cont}").Value = xl.QtyCancelled;
                        worksheet.Cell($"L{cont}").Value = xl.QtyRefunded;
                        worksheet.Cell($"M{cont}").Value = xl.QtyShipped;
                        worksheet.Cell($"N{cont}").Value = xl.City;
                        worksheet.Cell($"O{cont}").Value = xl.Country;
                        worksheet.Cell($"P{cont}").Value = xl.Region;
                        worksheet.Cell($"Q{cont}").Value = xl.PostCode;
                        worksheet.Cell($"R{cont}").Value = xl.Street;
                        worksheet.Cell($"S{cont}").Value = xl.Street2;
                        worksheet.Cell($"T{cont}").Value = xl.Telephone;
                        worksheet.Cell($"U{cont}").Value = xl.ContactEmail;
                        worksheet.Cell($"V{cont}").Value = xl.FirstName;
                        worksheet.Cell($"W{cont}").Value = xl.LastName;
                        worksheet.Cell($"X{cont}").Value = xl.RegionID;
                        cont++;
                    }

                    workbook.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    _logger.LogInformation("Xlsx file ready to download");
                    return this.File(
                        fileContents: stream.ToArray(),
                        contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileDownloadName: $"Jeg&SonsOrders-{DateTime.Now.ToString("yyyyMMdd")}.xlsx"
                    );
                }
            }
            else
            {
                _logger.LogError("You do not have access to this service");
                throw new BusinessException("You do not have access to this service");
            }
        }

        private string GetIdFromToken()
        {
            string accessToken = HttpContext.Request.Headers["authorization"];
            var newToken = accessToken.Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(newToken);
            var tokenS = handler.ReadToken(newToken) as JwtSecurityToken;
            var jti = tokenS.Claims.First(claim => claim.Type == "id").Value;
            _logger.LogInformation("jti: " + jti);
            return jti;
        }
    }
}
