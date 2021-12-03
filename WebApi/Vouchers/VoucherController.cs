/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                          Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Query Controller                      *
*  Type     : VoucherController                            License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to retrive accounting vouchers.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;
using System.Web.Http;

using System.Net.Http.Headers;
using System.Net.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Vouchers.UseCases;
using Empiria.FinancialAccounting.Vouchers.Adapters;


namespace Empiria.FinancialAccounting.WebApi.Vouchers {

  /// <summary>Query web API used to retrive accounting vouchers.</summary>
  public class VoucherController : WebApiController {

    #region Web Apis

    [HttpGet]
    [AllowAnonymous]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/pdf")]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/print")]
    public SingleObjectModel GetVoucherAsPdfFile([FromUri] long voucherId) {
      using (var usecases = VoucherUseCases.UseCaseInteractor()) {
        string html = usecases.GetVoucherAsHtmlString(voucherId);

        FileReportDto pdfFile = new FileReportDto(FileType.Pdf, html);

        return new SingleObjectModel(this.Request, pdfFile);
      }
    }


    //[HttpGet]
    //[AllowAnonymous]
    //[Route("v2/financial-accounting/vouchers/{voucherId:int}/pdf")]
    //[Route("v2/financial-accounting/vouchers/{voucherId:int}/print")]
    //public async Task<SingleObjectModel> GetVoucherAsPdfFileAsync([FromUri] int voucherId) {
    //  HttpResponseMessage responseMessage = GetVoucherAsHtml(voucherId);

    //  string html = await responseMessage.Content.ReadAsStringAsync();

    //  using (var service = PDFExporterService.ServiceInteractor()) {

    //    FileReportDto pdfFileDto = service.ExportVoucherHtmlToPdf(voucherId, html);

    //    return new SingleObjectModel(base.Request, pdfFileDto);
    //  }
    //}


    [HttpGet]
    [AllowAnonymous]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/html")]
    public HttpResponseMessage GetVoucherAsHtml([FromUri] long voucherId) {
      var response = new HttpResponseMessage();

      response.Content = new StringContent($"<div>Hello World voucher {voucherId}.</div>");

      response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

      return response;
    }


    [HttpPost]
    [Route("v2/financial-accounting/vouchers")]
    public CollectionModel SearchVouchers([FromBody] SearchVouchersCommand command) {
      base.RequireBody(command);

      using (var usecases = VoucherUseCases.UseCaseInteractor()) {
        FixedList<VoucherDescriptorDto> vouchers = usecases.SearchVouchers(command);

        return new CollectionModel(base.Request, vouchers);
      }
    }

    #endregion Web Apis

  }  // class VoucherController

}  // namespace Empiria.FinancialAccounting.WebApi.Vouchers
