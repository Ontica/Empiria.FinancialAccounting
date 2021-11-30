/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : BalanceUseCases                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to build balances.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.UseCases {

  /// <summary>Use cases used to build balances.</summary>
  public class BalanceUseCases : UseCase {

    #region Constructors and parsers

    protected BalanceUseCases() {
      // no-op
    }

    static public BalanceUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<BalanceUseCases>();
    }



    public BalanceDto BuildBalances(BalanceCommand command) {
      Assertion.AssertObject(command, "command");

      var balanceConstructor = new BalanceConstructor(command);

      Balance balance = balanceConstructor.BuildBalance();

      return BalanceMapper.Map(balance);
    }

    #endregion

    #region Use cases



    #endregion

  } // class BalanceUseCases

} // Empiria.FinancialAccounting.BalanceEngine.UseCases
