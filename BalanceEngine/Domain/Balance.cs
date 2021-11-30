/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : Balance                               License   : Please read LICENSE.txt file                 *
*                                                                                                            *
*  Summary  : Contains the header and entries of a balance                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Contains the header and entries of a balance.</summary>
  public class Balance {

    #region Constructors and parsers

    internal Balance(BalanceCommand command, FixedList<IBalanceEntry> entries) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(entries, "entries");

      Command = command;
      Entries = entries;
    }


    #endregion

    #region Properties

    public BalanceCommand Command {
      get;
    }

    public FixedList<IBalanceEntry> Entries {
      get;
    }


    #endregion

  } // class Balance

} // Empiria.FinancialAccounting.BalanceEngine
