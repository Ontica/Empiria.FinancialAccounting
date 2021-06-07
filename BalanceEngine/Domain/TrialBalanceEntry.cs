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
  public class TrialBalanceEntry {

    #region Constructors and parsers

    internal TrialBalanceEntry() {
      // Required by Empiria Framework.
    }


    #endregion Constructors and parsers


    [DataField("ID_MAYOR", ConvertFrom = typeof(decimal))]
    public Ledger Ledger {
      get;
      internal set;
    }

    [DataField("ID_MONEDA", ConvertFrom = typeof(decimal))]
    public Currency Currency {
      get;
      internal set;
    }


    [DataField("ID_CUENTA_ESTANDAR_HIST", ConvertFrom = typeof(long))]
    public Account Account {
      get;
      internal set;
    }


    [DataField("ID_SECTOR", ConvertFrom = typeof(long))]
    public Sector Sector {
      get;
      internal set;
    }


    [DataField("ID_CUENTA", ConvertFrom = typeof(decimal))]
    public int LedgerAccountId {
      get;
      internal set;
    }


    [DataField("ID_CUENTA_AUXILIAR", ConvertFrom = typeof(decimal))]
    public int SubledgerAccountId {
      get;
      internal set;
    }


    [DataField("SALDO_ANTERIOR")]
    public decimal InitialBalance {
      get;
      internal set;
    }


    [DataField("DEBE")]
    public decimal Debit {
      get;
      internal set;
    }


    [DataField("HABER")]
    public decimal Credit {
      get;
      internal set;
    }


    [DataField("SALDO_ACTUAL")]
    public decimal CurrentBalance {
      get;
      internal set;
    }


    public string ItemType {
      get;
      internal set;
    }


    public int Level {
      get {
        return EmpiriaString.CountOccurences(Account.Number, '-') + 1;
      }
    }


    internal void MultiplyBy(decimal value) {
      this.InitialBalance *= value;
      this.Debit *= value;
      this.Credit *= value;
      this.CurrentBalance *= value;
    }


    internal void Sum(TrialBalanceEntry entry) {
      this.InitialBalance += entry.InitialBalance;
      this.Credit += entry.Credit;
      this.Debit += entry.Debit;
      this.CurrentBalance += entry.CurrentBalance;
    }

  } // class TrialBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine
