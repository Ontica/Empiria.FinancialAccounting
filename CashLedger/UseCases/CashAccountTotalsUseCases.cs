/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Use case interactor class               *
*  Type     : CashAccountTotalsUseCases                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to retrive cash accounts totals.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Services;

using Empiria.Financial.Adapters;

using Empiria.CashFlow.CashLedger.Adapters;

using Empiria.FinancialAccounting.CashLedger.Adapters;

namespace Empiria.FinancialAccounting.CashLedger.UseCases {

  /// <summary>Use cases used to retrive cash accounts totals.</summary>
  public class CashAccountTotalsUseCases : UseCase {

    #region Constructors and parsers

    protected CashAccountTotalsUseCases() {
      // no-op
    }

    static public CashAccountTotalsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<CashAccountTotalsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<CashEntryExtendedDto> GetCashLedgerEntries(AccountsTotalsQuery query) {
      Assertion.Require(query, nameof(query));

      FixedList<CashEntryExtendedDto> entries = query.ExecuteEntries();

      return entries;
    }


    public FixedList<CashAccountTotalDto> GetCashLedgerTotals(AccountsTotalsQuery query) {
      Assertion.Require(query, nameof(query));

      FixedList<CashAccountTotal> totals = query.Execute();

      return CashAccountTotalsMapper.Map(totals);
    }

    #endregion Use cases

  }  // class CashAccountTotalsUseCases

}  // namespace Empiria.FinancialAccounting.CashLedger.UseCases
