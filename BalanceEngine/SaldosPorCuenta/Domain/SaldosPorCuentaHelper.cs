/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : SaldosPorCuentaHelper                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build balances by account report.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using Empiria.Collections;
using System.Linq;

using Empiria.FinancialAccounting.BalanceEngine.Data;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Helper methods to build balances by account report.</summary>
  internal class SaldosPorCuentaHelper {

    private readonly TrialBalanceQuery _query;

    internal SaldosPorCuentaHelper(TrialBalanceQuery query) {
      _query = query;
    }


    #region Public methods



    #endregion Public methods


    #region Private methods



    #endregion Private methods

  } // class SaldosPorCuentaHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
