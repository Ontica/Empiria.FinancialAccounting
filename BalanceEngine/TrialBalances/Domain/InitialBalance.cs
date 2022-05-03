/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : InitialBalance                               License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Contains the header and entries of initial balance.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Contains the header and entries of initial balance.</summary>
  internal class InitialBalance {

    internal InitialBalance(TrialBalanceCommand command,
                            FixedList<InitialBalanceEntry> entries) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(entries, "entries");

      this.LedgerId = command.LedgerId;
      this.LedgerAccountId = command.LedgerAccountId;
      this.AccountId = command.AccountId;
      this.SectorId = command.SectorId;
      this.SubsidiaryAccountId = command.SubsidiaryAccountId;
      this.CurrencyId = command.CurrencyId;
      this.Total = command.InitialBalance;
      this.Entries = entries;
    }


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

    public int CurrencyId {
      get;
    }

    public decimal Total {
      get;
    }

    public FixedList<InitialBalanceEntry> Entries {
      get;
    }

  } // class InitialBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine.Domain
