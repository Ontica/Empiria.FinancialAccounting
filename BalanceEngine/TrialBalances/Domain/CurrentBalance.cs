/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : CurrentBalance                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains the header and entries of current balance.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Contains the header and entries of current balance.</summary>
  internal class CurrentBalance {

    internal CurrentBalance(TrialBalanceCommand command,
                            FixedList<CurrentBalanceEntry> entries) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(entries, "entries");

      this.LedgerId = command.LedgerId;
      this.CurrencyId = command.CurrencyId;
      this.LedgerAccountId = command.LedgerAccountId;
      this.AccountId = command.AccountId;
      this.SectorId = command.SectorId;
      this.SubsidiaryAccountId = command.SubsidiaryAccountId;
      this.Debit = command.Debit;
      this.Credit = command.Credit;
      this.Total = command.Balance;
      this.Entries = entries;
    }


    public int LedgerId {
      get;
    }


    public int CurrencyId {
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


    public decimal Debit {
      get;
    }


    public decimal Credit {
      get;
    }


    public decimal Total {
      get;
    }

    public FixedList<CurrentBalanceEntry> Entries {
      get;
    }

  } // class CurrentBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine.Domain
