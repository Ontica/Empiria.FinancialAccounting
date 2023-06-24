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
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Vouchers.UseCases;
using Empiria.FinancialAccounting.Vouchers.Adapters;

using Empiria.FinancialAccounting.Reporting;

namespace Empiria.FinancialAccounting.WebApi.Vouchers {

  /// <summary>Query web API used to retrive accounting vouchers.</summary>
  public class VoucherController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/pdf")]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/print")]
    public SingleObjectModel GetVoucherAsPdfFile([FromUri] int voucherId) {

      using (var usecases = VoucherUseCases.UseCaseInteractor()) {
        VoucherDto voucher = usecases.GetVoucher(voucherId);

        var exporter = new PdfExporterService();

        FileReportDto pdfFileDto = exporter.Export(voucher);

        return new SingleObjectModel(base.Request, pdfFileDto);
      }
    }


    [HttpGet]
    [Route("v2/financial-accounting/vouchers/{voucherId:int}/excel")]
    public SingleObjectModel GetVoucherAsExcelFile([FromUri] int voucherId) {

      using (var usecases = VoucherUseCases.UseCaseInteractor()) {
        VoucherDto voucher = usecases.GetVoucher(voucherId);

        var exporter = new ExcelExporterService();

        FileReportDto pdfFileDto = exporter.Export(voucher);

        return new SingleObjectModel(base.Request, pdfFileDto);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/vouchers")]
    public CollectionModel SearchVouchers([FromBody] VouchersQuery query) {
      base.RequireBody(query);

      using (var usecases = VoucherUseCases.UseCaseInteractor()) {
        FixedList<VoucherDescriptorDto> vouchers = usecases.SearchVouchers(query);

        return new CollectionModel(base.Request, vouchers);
      }
    }

    #endregion Web Apis

  }  // class VoucherController

}  // namespace Empiria.FinancialAccounting.WebApi.Vouchers
