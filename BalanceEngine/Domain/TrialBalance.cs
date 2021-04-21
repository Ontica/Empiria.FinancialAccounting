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

      this.GeneralLedgerId = command.GeneralLedgerId;
      this.InitialDate = command.InitialDate;
      this.FinalDate = command.FinalDate;
      this.Entries = entries;
    }

    #region Constructors and parsers

    #endregion Constructors and parsers

    public int GeneralLedgerId {
      get;
    }

    public DateTime InitialDate {
      get;
    }

    public DateTime FinalDate {
      get;
    }

    public FixedList<TrialBalanceEntry> Entries {
      get;
    }

  } // class TrialBalance

} // namespace Empiria.FinancialAccounting.BalanceEngine
