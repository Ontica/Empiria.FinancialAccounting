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

      if (command.ReportType == OperationalReportType.BalanzaSat) {
        TrialBalanceCommand balanceCommand = command.MapToTrialBalanceCommand();

        using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {

          TrialBalanceDto trialBalance = usecases.BuildTrialBalance(balanceCommand);

          return OperationalReportMapper.MapFromTrialBalance(command, trialBalance);
        }

      } else if (command.ReportType == OperationalReportType.CatalogoDeCuentaSat) {

        //using (var usecases = AccountsChartUseCases.UseCaseInteractor()) {
        //  AccountsSearchCommand searchCommand = command.MapToAccountsSearchCommand();
        //  searchCommand.FromAccount = "1101";
        //  searchCommand.ToAccount = "1199";
        //  AccountsChartDto accountsChart = usecases.SearchAccounts(command.AccountsChartUID, searchCommand);

        //  Assertion.AssertObject(accountsChart, $"No hay datos para mostrar.");
          
        //  return OperationalReportMapper.MapFromAccountsChart(command, accountsChart);
        //}
        throw new NotImplementedException();
      } else {
        throw new NotImplementedException();
      }

    }



    #endregion

  } // class OperationalReportsUseCases

} // Empiria.FinancialAccounting.BanobrasIntegration.SATReports.UseCases
