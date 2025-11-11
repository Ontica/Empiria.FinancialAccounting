/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : ResumenAjusteEntry                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for resumen de ajuste entry.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  internal enum KeyAdjustmentTypes {

    SI,

    SIINT,

    NO
  }


  /// <summary>Represents an entry for resumen de ajuste entry</summary>
  internal class ResumenAjusteEntry : TrialBalanceEntry, ITrialBalanceEntry {

    #region Properties

    public KeyAdjustmentTypes KeyAdjustment {
      get; internal set;
    } = KeyAdjustmentTypes.NO;


    /// <summary>KeyAdjustment == "SI" ? CurrentBalance : 0</summary>
    public decimal DetailAccountBalanceMonth {
      get; internal set;
    }


    /// <summary>KeyAdjustment == "SIINT" ? InitialBalance : 0</summary>
    public decimal DeferredDetailAccountBalance {
      get; internal set;
    }


    /// <summary>DetailAccountBalanceMonth + DeferredDetailAccountBalance</summary>
    public decimal TotalBalanceAccountAdjustment {
      get; internal set;
    }


    public DateTime ConsultingDate {
      get; internal set;
    }

    #endregion Properties


    #region Methods

    internal void CalculateFields() {
      this.DetailAccountBalanceMonth = this.KeyAdjustment == KeyAdjustmentTypes.SI ? this.CurrentBalance : 0;
      this.DeferredDetailAccountBalance = this.KeyAdjustment == KeyAdjustmentTypes.SIINT ? this.InitialBalance : 0;
      this.TotalBalanceAccountAdjustment = this.DetailAccountBalanceMonth + this.DeferredDetailAccountBalance;
      this.TotalValorized = this.TotalBalanceAccountAdjustment * this.ExchangeRate;
    }


    internal void MapFromTrialBalanceEntry(TrialBalanceEntry entry) {

      this.ItemType = entry.ItemType;
      this.Ledger = entry.Ledger;
      this.Currency = entry.Currency;
      this.Account = entry.Account;
      this.Sector = entry.Sector;
      this.InitialBalance = entry.InitialBalance;
      this.Debit = entry.Debit;
      this.Credit = entry.Credit;
      this.CurrentBalance = entry.CurrentBalance;
      this.DebtorCreditor = entry.DebtorCreditor;
      this.LastChangeDate = entry.LastChangeDate;
    }

    #endregion Methods

  } // class ResumenAjusteEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
