/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : InitialBalanceEntry                          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Represents an entry for a initial balance.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents an entry for a initial balance.</summary>
  internal class InitialBalanceEntry {

    #region Constructors and parsers

    private InitialBalanceEntry() {
      // Required by Empiria Framework.
    }

    #endregion Constructors and parsers


    [DataField("LedgerId")]
    public int LedgerId {
      get;
      private set;
    }

    [DataField("LedgerAccountId")]
    public int LedgerAccountId {
      get;
      private set;
    }

    [DataField("AccountId") ]
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

    [DataField("CurrencyId")]
    public int CurrencyId {
      get;
      private set;
    }

    [DataField("InitialBalance")]
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

  } // class InitialBalanceEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine

