/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Empiria Plain Object                    *
*  Type     : BalanceEntry                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for balance.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  public interface IBalanceEntry {
  
  }

  /// <summary>Represents an entry for balance.</summary>
  public class BalanceEntry : IBalanceEntry {

    #region Constructors and parsers

    internal BalanceEntry() {
      // Required by Empiria Framework.
    }

    #endregion


    public Ledger Ledger {
      get; internal set;
    }


    public Currency Currency {
      get;  internal set;
    }


    public StandardAccount Account {
      get; internal set;
    }


    public Sector Sector {
      get;  internal set;
    }


    public int SubledgerAccountId {
      get;  internal set;
    }


    public decimal CurrentBalance {
      get;  internal set;
    }


    public DateTime LastChangeDate {
      get; internal set;
    }


    public int SubledgerAccountIdParent {
      get; internal set;
    } = -1;


    public int SubledgerNumberOfDigits {
      get; internal set;
    } = 0;


    public string GroupName {
      get; internal set;
    } = string.Empty;


    public string GroupNumber {
      get; internal set;
    } = string.Empty;


    public DebtorCreditorType DebtorCreditor {
      get; internal set;
    } = DebtorCreditorType.Deudora;


    public TrialBalanceItemType ItemType {
      get; internal set;
    } = TrialBalanceItemType.BalanceEntry;


    public string SubledgerAccountNumber {
      get; internal set;
    } = string.Empty;


  } // class BalanceEntry 

} // Empiria.FinancialAccounting.BalanceEngine
