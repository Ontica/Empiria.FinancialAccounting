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
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.BalanceEngine;
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
      var helper = new VouchersByAccountHelper(AccountStatementCommand);
      bool? isBalance = true;

      if (AccountStatementCommand.Command.TrialBalanceType == TrialBalanceType.BalanzaValorizadaComparativa ||
          AccountStatementCommand.Command.TrialBalanceType == TrialBalanceType.BalanzaValorizadaEnDolares) {
        isBalance = null;
      }

      Assertion.AssertObject(isBalance, $"Funcionalidad en proceso de desarrollo.");

      FixedList<VouchersByAccountEntry> voucherEntries = helper.GetVoucherEntries();

      FixedList<VouchersByAccountEntry> orderingVouchers = helper.GetOrderingVouchers(voucherEntries);

      VouchersByAccountEntry initialAccountBalance = helper.GetInitialOrCurrentAccountBalance(
                                                      AccountStatementCommand.Entry.InitialBalance);

      FixedList<VouchersByAccountEntry> vouchersWithCurrentBalance =
                                        helper.GetVouchersListWithCurrentBalance(orderingVouchers);

      FixedList<VouchersByAccountEntry> vouchers = helper.CombineInitialAccountBalanceWithVouchers(
                                                            vouchersWithCurrentBalance, initialAccountBalance);

      var returnedVoucherEntries = new FixedList<IVouchersByAccountEntry>(
                                        vouchers.Select(x => (IVouchersByAccountEntry) x));

      string title = helper.GetTitle();

      return new VouchersByAccount(AccountStatementCommand.Command, returnedVoucherEntries, title);
    }


    #endregion Public methods

  } // class VouchersByAccountConstructor

} // namespace Empiria.FinancialAccounting.Reporting
