/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : BalanzaDiferenciaDiariaMonedaBuilder       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de balanza diferencia diaria por moneda.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.AccountsLists;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Data;
using Empiria.Time;

namespace Empiria.FinancialAccounting.BalanceEngine {


  /// <summary>Genera los datos para el reporte de balanza diferencia diaria por moneda.</summary>
  internal class BalanzaDiferenciaDiariaMonedaBuilder {

    #region Constructors and parsers

    private readonly TrialBalanceQuery Query;

    internal BalanzaDiferenciaDiariaMonedaBuilder(TrialBalanceQuery query) {
      Query = query;
    }

    #endregion Constructors and parsers


    #region Public methods

    internal FixedList<BalanzaDiferenciaDiariaMonedaEntry> Build() {

      var helper = new BalanzaDiferenciaDiariaMonedaHelper(Query);

      FixedList<BalanzaColumnasMonedaEntry> balanzaColumnas = helper.GetBalanceInColumnByCurrency();

      FixedList<BalanzaColumnasMonedaEntry> entriesByAccountAndDate = helper.GetOrderingByAccountThenDate(
                                                                      balanzaColumnas);

      FixedList<BalanzaDiferenciaDiariaMonedaEntry> diffByDayEntries =
        helper.MergeBalanceByCurrencyIntoCurrencyDiffByDay(entriesByAccountAndDate);

      helper.CalculateBalancesForEntries(diffByDayEntries);

      FixedList<BalanzaDiferenciaDiariaMonedaEntry> entriesByDateAndAccount =
        helper.GetOrderingByDateThenAccount(diffByDayEntries);

      helper.AssignTagsToEntries(entriesByDateAndAccount);

      return entriesByDateAndAccount;
    }

    #endregion Public methods


    #region Private methods

    #endregion Private methods

  } // class BalanzaDiferenciaDiariaMonedaBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
