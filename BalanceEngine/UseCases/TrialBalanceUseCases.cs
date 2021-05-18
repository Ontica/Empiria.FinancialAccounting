/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : TrialBalanceUseCases                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to build trial balances.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.UseCases {

  /// <summary>Use cases used to build trial balances.</summary>
  public class TrialBalanceUseCases : UseCase{

    #region Constructors and parsers

    protected TrialBalanceUseCases() {
      // no-op
    }

    static public TrialBalanceUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TrialBalanceUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public TrialBalanceDto BuildTrialBalance(TrialBalanceCommand command) {
      Assertion.AssertObject(command, "command");

      //string fields = command.MapToFieldString();
      string[] fieldsGrouping = TrialBalanceTypes.MapToFieldString(command.TrialBalanceType);

      string filter = command.MapToFilterString();

      var trialBalanceEngine = new TrialBalanceEngine();

      TrialBalance trialBalance = trialBalanceEngine.BuildTrialBalance(command, fieldsGrouping, filter);

      return TrialBalanceMapper.Map(command, trialBalance);
    }

    #endregion Use cases

  } // class TrialBalanceUseCases

} // Empiria.FinancialAccounting.BalanceEngine.UseCases
