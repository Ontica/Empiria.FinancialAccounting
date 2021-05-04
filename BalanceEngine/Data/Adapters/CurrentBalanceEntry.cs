/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : CurrentBalanceEntry                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for current balance.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents an entry for current balance.</summary>
  internal class CurrentBalanceEntry {

    #region Constructors and parsers

    private CurrentBalanceEntry() {
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

    [DataField("SectorId")]
    public int SectorId {
      get;
      private set;
    }

    [DataField("SubsidiaryAccountId")]
    public int SubsidiaryAccountId {
      get;
      private set;
    }


    [DataField("Debit")]
    public decimal Debit {
      get;
      private set;
    }

    [DataField("Credit")]
    public decimal Credit {
      get;
      private set;
    }

    [DataField("Balance")]
    public decimal Total {
      get;
      private set;
    }


    public DateTime InitialDate {
      get; private set;
    }

    public DateTime FinalDate {
      get; private set;
    }

  } // class CurrentBalanceEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine

