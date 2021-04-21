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

    [DataField("StandardAccountNumber")]
    public string StandardAccountNumber {
      get;
      private set;
    } = string.Empty;


    [DataField("StandardAccountName")]
    public string StandardAccountName {
      get;
      private set;
    } = string.Empty;


    [DataField("InitialBalance")]
    public decimal InitialBalance {
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


    [DataField("CurrentBalance")]
    public decimal CurrentBalance {
      get;
      private set;
    }


    public int GeneralLedgerId {
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
