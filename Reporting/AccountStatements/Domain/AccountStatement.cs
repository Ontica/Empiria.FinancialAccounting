/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Information Holder                      *
*  Type     : VouchersByAccount                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains the header and entries of vouchers by account.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.BalanceExplorer.Adapters;
using Empiria.FinancialAccounting.Reporting.Adapters;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Contains the header and entries of vouchers by account.</summary>
  public class AccountStatement {

    #region Constructors and parsers

    internal AccountStatement(BalanceCommand command, FixedList<IVouchersByAccountEntry> entries, string title) {
      Assertion.AssertObject(command, "command");
      Assertion.AssertObject(entries, "entries");
      //Assertion.AssertObject(title, "title");

      Command = command;
      Entries = entries;
      Title = title;
    }


    #endregion

    #region Properties

    public BalanceCommand Command {
      get;
    }

    public FixedList<IVouchersByAccountEntry> Entries {
      get;
    }


    public string Title {
      get;
    }


    #endregion

  } // class VouchersByAccount 

} // namespace Empiria.FinancialAccounting.Reporting
