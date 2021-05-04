/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : DebitCreditBalanceEntry                          License   : Please read LICENSE.txt file      *
*                                                                                                            *
*  Summary  : Represents an entry for debit and credit balance.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents an entry for debit and credit balance.</summary>
  internal class DebitCreditBalanceEntry {

    #region Constructors and parsers

    private DebitCreditBalanceEntry() {
      // Required by Empiria Framework.
    }

    #endregion Constructors and parsers


    [DataField("LedgerId")]
    public int LedgerId {
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

    
    [DataField("Balance")]
    public decimal Balance {
      get;
      private set;
    }

  } // class DebitCreditBalanceEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
