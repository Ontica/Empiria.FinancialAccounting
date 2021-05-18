/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : TrialBalance                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains the header and entries of a trial balance                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Contains the header and entries of a trial balance.</summary>
  internal class TrialBalance {

    internal TrialBalance(TrialBalanceCommand command,
                          FixedList<TrialBalanceEntry> entries) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(entries, "entries");

      this.LedgerId = command.LedgerId;
      this.LedgerAccountId = command.LedgerAccountId;
      this.AccountId = command.AccountId;
      //this.SectorId = command.SectorId;
      //this.SubsidiaryAccountId = command.SubsidiaryAccountId;
      this.AccountCatalogueId = command.AccountCatalogueId;
      //this.AccountNumber = command.AccountNumber;
      //this.AccountName = command.AccountName;
      //this.InitialBalance = command.InitialBalance;
      this.StartDate = command.StartDate;
      this.EndDate = command.StartDate;
      //this.SubsidiaryAccountId = command.SubsidiaryAccountId;
      this.Entries = entries;
    }


    #region Constructors and parsers

    #endregion Constructors and parsers

    public int LedgerId {
      get;
    }

    public int LedgerAccountId {
      get;
    }


    public int AccountId {
      get;
    }


    public int SectorId {
      get;
    }


    public int SubsidiaryAccountId {
      get;
    }

    public int AccountCatalogueId {
      get;
    }

    public string AccountNumber {
      get;
    }

    public string AccountName {
      get;
    }

    public decimal InitialBalance{
      get;
    }

    public DateTime StartDate {
      get;
    }

    public DateTime EndDate {
      get;
    }

    public FixedList<TrialBalanceEntry> Entries {
      get;
    }


  } // class TrialBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine
