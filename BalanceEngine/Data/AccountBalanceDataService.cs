/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Service                            *
*  Type     : AccountBalanceDataService                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read methods for ledger account balances.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Provides data read methods for ledger account balances.</summary>
  static internal class AccountBalanceDataService {

    static internal AccountBalance GetCurrentBalance(LedgerAccount ledgerAccount) {
      var op = DataOperation.Parse("getAccountBalance", ledgerAccount.AccountNumber);

      return DataReader.GetPlainObject<AccountBalance>(op);
    }

  }  // class AccountBalanceDataService

}  // namespace Empiria.FinancialAccounting.BalanceEngine
