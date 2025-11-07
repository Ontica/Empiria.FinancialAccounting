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
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte Resumen de ajuste anual</summary>
  internal class ResumenAjusteAnualBuilder {

    private readonly TrialBalanceQuery _query;

    internal ResumenAjusteAnualBuilder(TrialBalanceQuery query) {
      _query = query;
    }


    internal FixedList<ResumenAjusteAnualEntry> Build() {

      FixedList<TrialBalanceEntry> baseAccountEntries = BalancesDataService.GetTrialBalanceEntries(_query);

      return Build(baseAccountEntries);
    }


    internal FixedList<ResumenAjusteAnualEntry> Build(FixedList<TrialBalanceEntry> baseAccountEntries) {



      return new FixedList<ResumenAjusteAnualEntry>();
    }


  } // class ResumenAjusteAnualBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
