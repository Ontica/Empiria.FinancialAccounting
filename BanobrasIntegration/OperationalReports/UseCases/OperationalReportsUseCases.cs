/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Use cases Layer                      *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Use case interactor class            *
*  Type     : OperationalReportsUseCases                    License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Use cases used to build operational reports and xml files from them.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

using Empiria.Services;

using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.UseCases;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.BanobrasIntegration.ExcelReports;
using Empiria.FinancialAccounting.BanobrasIntegration.XmlReports;

namespace Empiria.FinancialAccounting.BanobrasIntegration.OperationalReports {

  /// <summary>Use cases used to build operational reports and xml files from them.</summary>
  public class OperationalReportsUseCases : UseCase {

    #region Constructors and parsers

    public OperationalReportsUseCases() {
      // no-op
    }

    static public OperationalReportsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<OperationalReportsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FileReportDto ExportOperationalReport(OperationalReportCommand command) {
      Assertion.AssertObject(command, "command");

      OperationalReportDto reportData = GetOperationalReport(command);

      return ExportToFile(reportData, command);
    }


    public OperationalReportDto GetOperationalReport(OperationalReportCommand command) {
      Assertion.AssertObject(command, "command");

      return GetOperationalReportType(command);
    }

    #endregion


    #region Private methods

    private FileReportDto ExportToFile(OperationalReportDto reportData, OperationalReportCommand command) {
      switch (command.FileType) {
        case FileType.Excel:
          var excelExporter = new ExcelExporter();

          return excelExporter.ExportToExcel(reportData, command);


        case FileType.Xml:

          var xmlExporter = new XmlExporter();

          return xmlExporter.ExportToXml(reportData);

        default:

          throw Assertion.AssertNoReachThisCode();
      }
    }


    private OperationalReportDto GetOperationalReportType(OperationalReportCommand command) {

      switch (command.ReportType) {
        case OperationalReportType.BalanzaSAT:
          return GetTrialBalance(command);

        case OperationalReportType.CatalogoSAT:
          return GetAccountsChart(command);

        default:
          throw Assertion.AssertNoReachThisCode();
      }
    }

    private OperationalReportDto GetAccountsChart(OperationalReportCommand command) {

      using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        AccountsSearchCommand searchCommand = command.MapToAccountsSearchCommand();
      
        AccountsChartDto accountsChart = usecases.SearchAccounts(command.AccountsChartUID, searchCommand);

        return OperationalReportMapper.MapFromAccountsChart(command, accountsChart.Accounts);
      }

    }

    private OperationalReportDto GetTrialBalance(OperationalReportCommand command) {

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {
        TrialBalanceCommand balanceCommand = command.MapToTrialBalanceCommand();
       
        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(balanceCommand);

        return OperationalReportMapper.MapFromTrialBalance(command, trialBalance);
      }
    }

    #endregion

  } // class OperationalReportsUseCases

} // Empiria.FinancialAccounting.BanobrasIntegration.OperationalReports
