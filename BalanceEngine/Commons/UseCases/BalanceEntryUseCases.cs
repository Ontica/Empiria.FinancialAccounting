/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Use case interactor class               *
*  Type     : TrialBalanceUseCases                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to build balance entry.                                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.Services;

namespace Empiria.FinancialAccounting.BalanceEngine.UseCases {

  /// <summary>Use cases used to build balance entry.</summary>
  public class BalanceEntryUseCases : UseCase {

    #region Constructors and parsers

    protected BalanceEntryUseCases() {
      // no-op
    }

    static public BalanceEntryUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<BalanceEntryUseCases>();
    }

    #endregion Constructors and parsers

    #region UseCases

    public FixedList<BalanceEntryDto> BuildBalanceEntries(TrialBalanceQuery query) {

      var builder = new BalanceEntryBuilder();
      FixedList<BalanceEntry> entries = builder.GetBalanceEntries(query);

      return BalanceEntryMapper.Map(entries);
    }

    #endregion UseCases

  }
}
