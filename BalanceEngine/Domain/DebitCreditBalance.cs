/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : DebitCreditBalance                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains the header and entries of a credit and debit balance.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Contains the header and entries of a credit and debit balance.</summary>
  internal class DebitCreditBalance {


    internal DebitCreditBalance(TrialBalanceCommand command,
                                FixedList<DebitCreditBalanceEntry> entries) {

      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(entries, "entries");

      this.LedgerId = command.LedgerId;
      this.AccountId = command.AccountId;
      this.SectorId = command.SectorId;
      this.SubsidiaryAccountId = command.SubsidiaryAccountId;
      this.Balance = command.Balance;
      this.Entries = entries;
    }


    public int LedgerId {
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


    public decimal Balance {
      get;
    }

    public FixedList<DebitCreditBalanceEntry> Entries {
      get;
    }

  } // class DebitCreditBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine
