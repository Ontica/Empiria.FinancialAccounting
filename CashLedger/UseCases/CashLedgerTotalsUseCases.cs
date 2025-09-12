/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Use case interactor class               *
*  Type     : CashLedgerTotalsUseCases                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to retrive and manage cash ledger transactions.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Services;

using Empiria.CashFlow.CashLedger.Adapters;

using Empiria.FinancialAccounting.CashLedger.Adapters;

namespace Empiria.FinancialAccounting.CashLedger.UseCases {

  /// <summary>Use cases used to retrive and manage cash ledger transactions.</summary>
  public class CashLedgerTotalsUseCases : UseCase {

    #region Constructors and parsers

    protected CashLedgerTotalsUseCases() {
      // no-op
    }

    static public CashLedgerTotalsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<CashLedgerTotalsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<CashEntryDescriptor> GetCashLedgerEntries(BaseCashLedgerTotalsQuery query) {
      Assertion.Require(query, nameof(query));

      FixedList<CashEntryDescriptor> entries = query.ExecuteEntries();

      return entries;
    }


    public FixedList<CashLedgerTotalEntryDto> GetCashLedgerTotals(BaseCashLedgerTotalsQuery query) {
      Assertion.Require(query, nameof(query));

      FixedList<CashLedgerTotal> totals = query.Execute();

      return CashLedgerTotalsMapper.Map(totals);
    }

    #endregion Use cases

  }  // class CashLedgerTotalsUseCases

}  // namespace Empiria.FinancialAccounting.CashLedger.UseCases
