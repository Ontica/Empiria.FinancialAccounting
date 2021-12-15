/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Helper methods                          *
*  Type     : VouchersByAccountHelper                       License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Helper methods to build vouchers by account information.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.Reporting.Adapters;
using Empiria.FinancialAccounting.Reporting.Data;
using Empiria.FinancialAccounting.Reporting.Domain;

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Helper methods to build vouchers by account information.</summary>
  internal class VouchersByAccountHelper {

    private readonly AccountStatementCommand AccountStatementCommand;

    internal VouchersByAccountHelper(AccountStatementCommand accountStatementCommand) {
      Assertion.AssertObject(accountStatementCommand, "accountStatementCommand");

      AccountStatementCommand = accountStatementCommand;
    }


    #region Public methods


    internal FixedList<VouchersByAccountEntry> CombineInitialAccountBalanceWithVouchers(
                                                FixedList<VouchersByAccountEntry> orderingVouchers,
                                                VouchersByAccountEntry initialAccountBalance) {

      var totalBalanceAndVouchers = new List<VouchersByAccountEntry>();
      if (initialAccountBalance != null) {
        totalBalanceAndVouchers.Add(initialAccountBalance);
      }
      totalBalanceAndVouchers.AddRange(orderingVouchers);

      return totalBalanceAndVouchers.ToFixedList();
    }


    internal FixedList<VouchersByAccountEntry> GetOrderingVouchers(
                                              FixedList<VouchersByAccountEntry> voucherEntries) {

      List<VouchersByAccountEntry> returnedVouchers = voucherEntries
                                                      .OrderBy(a => a.Ledger.Number)
                                                      .ThenBy(a => a.Currency.Code)
                                                      .ThenBy(a => a.Account.Number)
                                                      .ThenBy(a => a.Sector.Code)
                                                      .ThenBy(a => a.SubledgerAccountNumber)
                                                      .ThenBy(a => a.VoucherNumber).ToList();
      return returnedVouchers.ToFixedList();
    }


    internal VouchersByAccountEntry GetInitialAccountBalance() {

      var initialBalanceEntry = new VouchersByAccountEntry();

      initialBalanceEntry.Ledger = Ledger.Empty;
      initialBalanceEntry.Currency = Currency.Empty;
      initialBalanceEntry.Account = StandardAccount.Empty;
      initialBalanceEntry.Sector = Sector.Empty;
      initialBalanceEntry.SubledgerAccountNumber = "";
      initialBalanceEntry.VoucherNumber = "";
      initialBalanceEntry.Concept = "SALDO INICIAL";
      initialBalanceEntry.CurrentBalance = AccountStatementCommand.Entry.InitialBalance;
      initialBalanceEntry.ItemType = TrialBalanceItemType.Total;

      return initialBalanceEntry;
    }


    internal string GetTitle() {
      if (AccountStatementCommand.Entry.AccountNumber != "" &&
          (AccountStatementCommand.Entry.SubledgerAccountNumber != "" &&
          AccountStatementCommand.Entry.SubledgerAccountNumber != "0")) {

        return $"{AccountStatementCommand.Entry.AccountNumber}" +
               $": {AccountStatementCommand.Entry.AccountName} " +
               $"({AccountStatementCommand.Entry.SubledgerAccountNumber})";

      } else if (AccountStatementCommand.Entry.AccountNumber != "") {

        return $"{AccountStatementCommand.Entry.AccountNumber}" +
               $": {AccountStatementCommand.Entry.AccountName}";

      } else if (AccountStatementCommand.Entry.AccountNumber == string.Empty &&
                 (AccountStatementCommand.Entry.SubledgerAccountNumber != "" &&
                 AccountStatementCommand.Entry.SubledgerAccountNumber != "0")) {

        return $"{AccountStatementCommand.Entry.SubledgerAccountNumber}";
      } else {
        return "";
      }

    }


    internal FixedList<VouchersByAccountEntry> GetVoucherEntries() {

      VouchersByAccountCommandData commandData = VouchersByAccountCommandDataMapped();

      return StoredVoucherDataService.GetVouchersByAccountEntries(commandData);
    }


    private VouchersByAccountCommandData VouchersByAccountCommandDataMapped() {

      var commandExtensions = new VouchersByAccountCommandExtensions(AccountStatementCommand);
      VouchersByAccountCommandData commandData = commandExtensions.MapToVouchersByAccountCommandData();

      return commandData;
    }

    #endregion Public methods


    #region Private methods



    #endregion Private methods

  } // class VouchersByAccountHelper

} // namespace Empiria.FinancialAccounting.Reporting.Domain
