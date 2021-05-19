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

      this.LedgerGroupId = command.LedgerGroupId;
      this.TrialBalanceType = command.TrialBalanceType;
      this.FromLedgerId = command.FromLedgerId;
      this.ToLedgerId = command.ToLedgerId;
      this.StartDate = command.StartDate;
      this.EndDate = command.StartDate;
      this.Sectors = command.Sectors;
      this.FromAccount = command.FromAccount;
      this.ToAccount = command.ToAccount;
      this.Entries = entries;
    }


    #region Constructors and parsers

    #endregion Constructors and parsers

    public int LedgerGroupId {
      get;
    }

    public int TrialBalanceType {
      get;
    }
    public int FromLedgerId {
      get;
    }

    public int ToLedgerId {
      get;
    }

    public DateTime StartDate {
      get;
    }

    public DateTime EndDate {
      get;
    }

    public string[] Sectors {
      get;
    }

    public string FromAccount {
      get;
    }

    public string ToAccount {
      get;
    }

    
    public FixedList<TrialBalanceEntry> Entries {
      get;
    }


  } // class TrialBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine
