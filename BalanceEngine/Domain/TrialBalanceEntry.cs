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

    [DataField("BudgetKey")]
    public string BudgetKey {
      get;
      private set;
    }

    [DataField("AccountNumber")]
    public string AccountNumber {
      get;
      private set;
    }

    //[DataField("AccountName")]
    //public string AccountName {
    //  get;
    //  private set;
    //}

    //[DataField("SubsidiaryAccountNumber")]
    //public string SubsidiaryAccountNumber {
    //  get;
    //  private set;
    //}

    //[DataField("SubsidiaryAccountName")]
    //public string SubsidiaryAccountName {
    //  get;
    //  private set;
    //}

    //[DataField("AccountTypeId")]
    //public int AccountTypeId {
    //  get;
    //  private set;
    //}



  } // class TrialBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine
