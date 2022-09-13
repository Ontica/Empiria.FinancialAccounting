/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Service Layer                        *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Service provider                     *
*  Type     : AccountStatementExcelExporterService          License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Main service to export account statement information to Microsoft Excel.                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Reporting.Adapters;

namespace Empiria.FinancialAccounting.Reporting.AccountStatements {

  /// <summary>Main service to export account statement information to Microsoft Excel.</summary>
  public class AccountStatementExcelExporterService {

    public FileReportDto Export(AccountStatementDto accountStatement) {
      Assertion.Require(accountStatement, nameof(accountStatement));

      var templateUID = $"VouchersByAccountTemplate";

      var templateConfig = FileTemplateConfig.Parse(templateUID);

      var exporter = new AccountStatementExcelExporter(templateConfig);

      ExcelFile excelFile = exporter.CreateExcelFile(accountStatement);

      return excelFile.ToFileReportDto();
    }

  } // class AccountStatementExcelExporterService

} // namespace Empiria.FinancialAccounting.Reporting
