/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Builder                                 *
*  Type     : AnaliticoDeCuentasBuilder                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte Resumen de ajuste anual.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte Resumen de ajuste anual</summary>
  internal class ResumenAjusteAnualBuilder {

    private readonly TrialBalanceQuery _query;

    internal ResumenAjusteAnualBuilder(TrialBalanceQuery query) {
      _query = query;
    }


    #region Public methods

    internal FixedList<ResumenAjusteAnualEntry> Build() {

      var helper = new ResumenAjusteAnualHelper(_query);

      FixedList<ResumenAjusteEntry> resumenAjusteEntries = helper.GetResumenAjusteEntriesByMonths();

      FixedList<ResumenAjusteAnualEntry> resumenAjusteAnual = helper.MapToResumenAjusteAnual(
                                                                          resumenAjusteEntries);

      return resumenAjusteAnual;
    }

    #endregion Public methods

  } // class ResumenAjusteAnualBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
