/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll      Pattern   : Service provider                        *
*  Type     : VouchersByAccountConstructor               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services to generate vouchers by account.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Reporting.Adapters;
using Empiria.FinancialAccounting.Reporting.Data;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Provides services to generate vouchers by account.</summary>
  internal class VouchersByAccountConstructor {

    private readonly AccountStatementCommand AccountStatementCommand;

    internal VouchersByAccountConstructor(AccountStatementCommand accountStatementCommand) {
      Assertion.AssertObject(accountStatementCommand, "accountStatementCommand");

      AccountStatementCommand = accountStatementCommand;
    }



    #region Public methods

    internal VouchersByAccount Build() {

      FixedList<VouchersByAccountEntry> voucherEntries = GetVoucherEntries();

      var returnedVoucherEntries = new FixedList<IVouchersByAccountEntry>(
                                        voucherEntries.Select(x => (IVouchersByAccountEntry) x));
      string title = ""; //GetTitle();

      return new VouchersByAccount(AccountStatementCommand.Command, returnedVoucherEntries, title);
    }

    private string GetTitle() {
      if (AccountStatementCommand.Entry.AccountNumber != string.Empty &&
          AccountStatementCommand.Entry.SubledgerAccountNumber != string.Empty &&
          AccountStatementCommand.Entry.SubledgerAccountNumber != "0") {

        return $"{AccountStatementCommand.Entry.AccountNumber}: " +
               $"{AccountStatementCommand.Entry.AccountName} " +
               $"({AccountStatementCommand.Entry.SubledgerAccountNumber})";

      } else if (AccountStatementCommand.Entry.AccountNumber != string.Empty) {

        return $"{AccountStatementCommand.Entry.AccountNumber}: " +
               $"{AccountStatementCommand.Entry.AccountName}";

      } else if (AccountStatementCommand.Entry.AccountNumber == string.Empty &&
                 AccountStatementCommand.Entry.SubledgerAccountNumber != string.Empty &&
                 AccountStatementCommand.Entry.SubledgerAccountNumber != "0") {

        return $"{AccountStatementCommand.Entry.SubledgerAccountNumber}";
      } else {
        return string.Empty;
      }
      
    }



    #endregion Public methods


    private FixedList<VouchersByAccountEntry> GetVoucherEntries() {

      VouchersByAccountCommandData commandData = VouchersByAccountCommandDataMapped();

      return StoredVoucherDataService.GetVouchersByAccountEntries(commandData);
    }

    private VouchersByAccountCommandData VouchersByAccountCommandDataMapped() {

      var commandExtensions = new VouchersByAccountCommandExtensions(AccountStatementCommand);
      VouchersByAccountCommandData commandData = commandExtensions.MapToVouchersByAccountCommandData();

      return commandData;
    }

    #region Private methods

    #endregion

  } // class VouchersByAccountConstructor

} // namespace Empiria.FinancialAccounting.Reporting
