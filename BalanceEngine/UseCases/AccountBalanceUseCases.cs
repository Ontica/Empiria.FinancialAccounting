/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : AccountBalanceUseCases                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to retrive the balance for a given account.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.UseCases {

  /// <summary>Use cases used to retrive the balance for a given account.</summary>
  public class AccountBalanceUseCases : UseCase {

    #region Constructors and parsers

    protected AccountBalanceUseCases() {
      // no-op
    }

    static public AccountBalanceUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<AccountBalanceUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public AccountBalanceDto AccountBalance(string accountNumber) {
      Assertion.AssertObject(accountNumber, "accountNumber");

      var balanceEngine = new AccountBalanceEngine();

      AccountBalance balance = balanceEngine.AccountBalance(accountNumber);

      return AccountBalanceMapper.Map(balance);
    }

    #endregion Use cases

  }  // class AccountBalanceUseCases

}  // namespace Empiria.FinancialAccounting.BalanceEngine.UseCases
