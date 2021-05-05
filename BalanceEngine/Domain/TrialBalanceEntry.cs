/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : TrialBalanceEntry                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a trial balance.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents an entry for a trial balance.</summary>
  internal class TrialBalanceEntry {

    #region Constructors and parsers

    private TrialBalanceEntry() {
      // Required by Empiria Framework.
    }

    #endregion Constructors and parsers

    [DataField("LedgerId")]
    public int LedgerId {
      get;
      private set;
    }

    [DataField("CurrencyId")]
    public int CurrencyId {
      get;
      private set;
    }

    [DataField("SectorId")]
    public int SectorId {
      get;
      private set;
    }

    [DataField("LedgerAccountId")]
    public int LedgerAccountId {
      get;
      private set;
    }

    [DataField("AccountId")]
    public int AccountId {
      get;
      private set;
    }


    public int AccountCatalogueId {
      get; private set;
    }


    public string InitialDate {
      get; private set;
    }


    public string FinalDate {
      get; private set;
    }


    public int GroupId {
      get; private set;
    }

  } // class TrialBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine
