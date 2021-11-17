/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Use case interactor class               *
*  Type     : LedgerUseCases                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for accounting ledger management.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.UseCases {

  /// <summary>Use cases for accounts chart management.</summary>
  public class LedgerUseCases : UseCase {

    #region Constructors and parsers

    protected LedgerUseCases() {
      // no-op
    }

    static public LedgerUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<LedgerUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public LedgerDto GetLedger(string ledgerUID) {
      Assertion.AssertObject(ledgerUID, "ledgerUID");

      var ledger = Ledger.Parse(ledgerUID);

      return LedgerMapper.Map(ledger);
    }


    public LedgerAccountDto GetLedgerAccount(string ledgerUID, int accountId) {
      Assertion.AssertObject(ledgerUID, "ledgerUID");
      Assertion.Assert(accountId > 0, "accountId");

      var ledger = Ledger.Parse(ledgerUID);

      LedgerAccount ledgerAccount = ledger.GetAccountWithId(accountId);

      return LedgerMapper.MapAccount(ledgerAccount, DateTime.Today);
    }


    public FixedList<SubledgerDto> GetSubledgers(string ledgerUID) {
      Assertion.AssertObject(ledgerUID, "ledgerUID");

      var ledger = Ledger.Parse(ledgerUID);

      FixedList<Subledger> subledgers = ledger.Subledgers();

      return SubledgerMapper.Map(subledgers);
    }


    #endregion Use cases

  }  // class LedgerUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
