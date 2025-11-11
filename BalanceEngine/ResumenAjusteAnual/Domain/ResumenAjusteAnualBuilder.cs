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


    internal FixedList<ResumenAjusteEntry> Build() {

      var helper = new ResumenAjusteAnualHelper(_query);

      return helper.GetBalancesByMonths();
    }


  } // class ResumenAjusteAnualBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
