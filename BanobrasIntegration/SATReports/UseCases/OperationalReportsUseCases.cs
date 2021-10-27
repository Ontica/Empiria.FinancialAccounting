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

using Empiria.FinancialAccounting.BanobrasIntegration.SATReports.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;
using Empiria.FinancialAccounting.Adapters;
using Empiria.FinancialAccounting.UseCases;

namespace Empiria.FinancialAccounting.BanobrasIntegration.SATReports.UseCases {

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

    public OperationalReportDto GetOperationalReport(OperationalReportCommand command) {
      Assertion.AssertObject(command, "command");

      return GetOperationalReportType(command);
    }

    #endregion


    #region Private methods

    internal OperationalReportDto GetOperationalReportType(OperationalReportCommand command) {

      switch (command.ReportType) {
        case OperationalReportType.BalanzaSat:
          return GetTrialBalance(command);

        case OperationalReportType.CatalogoDeCuentaSat:
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

} // Empiria.FinancialAccounting.BanobrasIntegration.SATReports.UseCases
