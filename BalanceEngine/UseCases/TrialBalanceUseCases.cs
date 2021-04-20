/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : TrialBalanceUseCases                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to retrive the trial balance.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.BalanceEngine.Domain;

namespace Empiria.FinancialAccounting.BalanceEngine.UseCases {

  /// <summary>Use cases used to retrive the trial balance.</summary>
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

    public FixedList<TrialBalanceDto> TrialBalance(TrialBalanceFields fields) {
      Assertion.AssertObject(fields, "fields");


      var trialBalanceEngine = new TrialBalanceEngine();

      var balance = trialBalanceEngine.TrialBalance(fields);

      return TrialBalanceMapper.Map(balance);
    }

    #endregion

  } // class TrialBalanceUseCases 

} // Empiria.FinancialAccounting.BalanceEngine.UseCases 
