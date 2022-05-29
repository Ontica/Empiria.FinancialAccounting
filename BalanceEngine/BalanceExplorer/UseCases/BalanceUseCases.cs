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
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;
using Empiria.Services;

namespace Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.UseCases {

  /// <summary>Use cases used to build balances.</summary>
  public class BalanceUseCases : UseCase {

    #region Constructors and parsers

    protected BalanceUseCases() {
      // no-op
    }

    static public BalanceUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<BalanceUseCases>();
    }

    #endregion Constructors and parsers

    public BalanceDto BuildBalanceSearch(BalanceCommand command) {
      Assertion.Require(command, "command");

      var balanceConstructor = new BalanceConstructor(command);

      Balance balance = balanceConstructor.BuildBalance();

      return BalanceMapper.Map(balance);
    }

    #region Use cases



    #endregion Use cases

  } // class BalanceUseCases

} // namespace Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.UseCases
