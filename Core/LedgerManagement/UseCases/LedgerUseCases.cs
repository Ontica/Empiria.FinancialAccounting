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

    public LedgerDto GetLedger(object lEDGER_UID) {
      throw new NotImplementedException();
    }


    #endregion Use cases

  }  // class LedgerUseCases

}  // namespace Empiria.FinancialAccounting.UseCases
