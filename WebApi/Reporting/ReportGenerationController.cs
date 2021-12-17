/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                           Component : Web Api                               *
*  Assembly : Empiria.FinancialAccounting.WebApi.dll       Pattern   : Controller                            *
*  Type     : ReportGenerationController                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web API used to generate financial accounting reports: financial, operational, fiscal.   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.FinancialAccounting.Reporting;
using Empiria.FinancialAccounting.Reporting.Adapters;
using Empiria.FinancialAccounting.Reporting.UseCases;

namespace Empiria.FinancialAccounting.WebApi.Reporting {

  /// <summary>Query web API used to generate financial accounting reports:
  /// financial, operational, fiscal.</summary>
  public class ReportGenerationController : WebApiController {

    #region Web Apis


    [HttpPost]
    [Route("v2/financial-accounting/reporting/{reportType}/export")]
    public SingleObjectModel ExportReportData([FromUri] string reportType,
                                              [FromBody] BuildReportCommand command) {
      base.RequireBody(command);

      command.ReportType = reportType;

      using (var service = ReportingService.ServiceInteractor()) {
        FileReportDto fileReportDto = service.ExportReport(command);

        return new SingleObjectModel(this.Request, fileReportDto);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/reporting/{reportType}/data")]
    public SingleObjectModel GenerateReport([FromUri] string reportType,
                                            [FromBody] BuildReportCommand command) {
      base.RequireBody(command);

      command.ReportType = reportType;

      using (var service = ReportingService.ServiceInteractor()) {
        ReportDataDto reportData = service.GenerateReport(command);

        return new SingleObjectModel(this.Request, reportData);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/balance-voucher")]
    public SingleObjectModel GetVouchersByAccount(
            [FromBody] AccountStatementCommand accountStatementCommand) {
      base.RequireBody(accountStatementCommand);

      using (var usecases = VouchersByAccountUseCases.UseCaseInteractor()) {

        VouchersByAccountDto vouchers = usecases.BuilVouchersByAccount(accountStatementCommand);

        return new SingleObjectModel(this.Request, vouchers);
      }
    }


    [HttpPost]
    [Route("v2/financial-accounting/balance-voucher/excel")]
    public SingleObjectModel GetExcelVouchersByAccount(
            [FromBody] AccountStatementCommand accountStatementCommand) {
      base.RequireBody(accountStatementCommand);

      using (var usecases = VouchersByAccountUseCases.UseCaseInteractor()) {

        VouchersByAccountDto vouchers = usecases.BuilVouchersByAccount(accountStatementCommand);

        var excelExporter = new ExcelExporterService();

        FileReportDto excelFileDto = excelExporter.Export(vouchers);

        return new SingleObjectModel(this.Request, excelFileDto);
      }
    }


    #endregion Web Apis

  } // class ReportGenerationController

} // namespace Empiria.FinancialAccounting.WebApi.Reporting
