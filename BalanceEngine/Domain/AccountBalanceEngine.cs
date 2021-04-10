/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : AccountBalanceEngine                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to calculate the balance for a given account.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Provides services to calculate the balance for a given account.</summary>
  internal class AccountBalanceEngine {

    internal AccountBalanceEngine() {
      // no-op
    }


    internal AccountBalance AccountBalance(string accountNumber) {
      Assertion.AssertObject(accountNumber, "accountNumber");

      var ledgerAccount = LedgerAccount.Parse(accountNumber);

      return CalculateCurrentBalance(ledgerAccount);
    }


    #region Private methods

    private AccountBalance CalculateCurrentBalance(LedgerAccount ledgerAccount) {
      return AccountBalanceDataService.GetCurrentBalance(ledgerAccount);
    }

    #endregion Private methods


  }  // class AccountBalanceEngine

}  // namespace Empiria.FinancialAccounting.BalanceEngine
