/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Balances Exporter                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Use case interactor class            *
*  Type     : ExportBalancesUseCases                        License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Use cases used to export balances to other Banobras' systems.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

using Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.Adapters;
using Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.Data;

namespace Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.UseCases {

  /// <summary>Use cases used to export balances to other Banobras' systems.</summary>
  public class ExportBalancesUseCases : UseCase {

    #region Constructors and parsers

    protected ExportBalancesUseCases() {
      // no-op
    }

    static public ExportBalancesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<ExportBalancesUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public FixedList<ExportedBalancesDto> Export(ExportBalancesCommand command) {
      Assertion.Require(command, "command");

      using (var usecases = TrialBalanceUseCases.UseCaseInteractor()) {
        TrialBalanceQuery query = command.MapToTrialBalanceQuery();

        TrialBalanceDto trialBalance = usecases.BuildTrialBalance(query);

        FixedList<ExportedBalancesDto> balances =
                      ExportBalancesMapper.MapToExportedBalances(command, trialBalance);

        if (command.StoreInto != StoreBalancesInto.None) {
          ExportBalancesDataService.StoreBalances(command, balances);
        }

        return balances;
      }
    }


    #endregion Use cases

  }  // class ExportBalancesUseCases

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.BalancesExporter.UseCases
