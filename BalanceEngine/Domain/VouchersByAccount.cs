/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : VouchersByAccount                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains the header and entries of vouchers by account.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Contains the header and entries of vouchers by account.</summary>
  public class VouchersByAccount {

    #region Constructors and parsers

    internal VouchersByAccount(BalanceCommand command, FixedList<IVouchersByAccountEntry> entries) {
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

    public FixedList<IVouchersByAccountEntry> Entries {
      get;
    }


    #endregion

  } // class VouchersByAccount 

} // namespace Empiria.FinancialAccounting.BalanceEngine
