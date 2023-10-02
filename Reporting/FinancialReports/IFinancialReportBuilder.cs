/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Service Layer                        *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Service provider                     *
*  Type     : FinancialReportExportService                  License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides services used to export financial reports to files.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Storage;

using Empiria.FinancialAccounting.FinancialReports.Adapters;

namespace Empiria.FinancialAccounting.Reporting.FinancialReports {

  internal interface IFinancialReportBuilder {

    FileReportDto Build(FinancialReportDto financialReportDto);

  }  // interface IFinancialReportBuilder

}  // namespace Empiria.FinancialAccounting.Reporting.FinancialReports
