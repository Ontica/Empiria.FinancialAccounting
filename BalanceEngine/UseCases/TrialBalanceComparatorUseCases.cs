/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : TrialBalanceComparatorUseCases             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to compare trial balances.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.Services;

namespace Empiria.FinancialAccounting.BalanceEngine.UseCases {

  /// <summary>Use cases used to compare trial balances.</summary>
  public class TrialBalanceComparatorUseCases : UseCase {

    #region Constructors and parsers

    protected TrialBalanceComparatorUseCases() {
      // no-op
    }

    static public TrialBalanceComparatorUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TrialBalanceComparatorUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public TrialBalanceDto BuildBalancesByAccount(TrialBalanceCommand command) {
      Assertion.AssertObject(command, "command");

      var trialBalanceEngine = new TrialBalanceEngine(command);

      TrialBalance trialBalance = trialBalanceEngine.BuildTrialBalance();

      return TrialBalanceMapper.Map(trialBalance);
    }

    #endregion

  } // class TrialBalanceComparatorUseCases

} // namespace Empiria.FinancialAccounting.BalanceEngine.UseCases
