/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Helper methods                          *
*  Type     : BalanzaTradicionalHelper                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build traditional trial balances.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {


  /// <summary>Helper methods to build traditional trial balances.</summary>
  internal class BalanzaTradicionalHelper {

    private readonly TrialBalanceCommand command;

    public BalanzaTradicionalHelper(TrialBalanceCommand Command) {

      command = Command;
    }


    #region Public methods



    #endregion Public methods


    #region Private methods



    #endregion Private methods


  } // class BalanzaTradicionalHelper

} // namespace Empiria.FinancialAccounting.BalanceEngine
