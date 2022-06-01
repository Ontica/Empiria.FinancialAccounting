/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : SaldosPorCuentaBuilder                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de saldos por cuenta.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de saldos por cuenta.</summary>
  internal class SaldosPorCuentaBuilder {

    private readonly TrialBalanceQuery _query;

    internal SaldosPorCuentaBuilder(TrialBalanceQuery query) {
      _query = query;
    }


    internal TrialBalance Builder() {

      throw new NotImplementedException();
    }

  } // class SaldosPorCuentaBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
