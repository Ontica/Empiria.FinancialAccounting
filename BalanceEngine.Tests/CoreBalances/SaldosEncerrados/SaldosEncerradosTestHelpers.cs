/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test cases                              *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Service provider                        *
*  Type     : BalanzaTradicionalTestHelpers              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services for SaldosEncerradosVsCoreBalancesTests tests.                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Threading.Tasks;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.UseCases;

using Empiria.FinancialAccounting.Tests;

namespace Empiria.Tests.FinancialAccounting.BalanceEngine {

  /// <summary>Provides services for SaldosEncerradosVsCoreBalancesTests tests</summary>
  static internal class SaldosEncerradosTestHelpers {

    #region Methods

    static internal async Task<FixedList<SaldosEncerradosBaseEntryDto>> GetSaldosEncerrados(
                                DateTime fromDate, DateTime toDate) {

      var query = new SaldosEncerradosQuery() {
        AccountsChartUID = TestingConstants.IFRS_ACCOUNTS_CHART.UID,
        FromDate = fromDate,
        ToDate = toDate
      };

      return await ExecuteLockedUpBalances(query);
    }

    #endregion Methods


    #region Private methods

    static private async Task<FixedList<SaldosEncerradosBaseEntryDto>> ExecuteLockedUpBalances(SaldosEncerradosQuery query) {

      using (var usecase = TrialBalanceUseCases.UseCaseInteractor()) {

        var dto = await usecase.BuildSaldosEncerrados(query);

        return dto.Entries;
      }
    }

    #endregion Private methods

  } // class SaldosEncerradosTestHelpers

} // namespace Empiria.Tests.FinancialAccounting.BalanceEngine
