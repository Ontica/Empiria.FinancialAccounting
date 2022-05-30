/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Service provider                        *
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

namespace Empiria.FinancialAccounting.Reporting {

  /// <summary>Provides services to generate vouchers by account.</summary>
  internal class AccountStatementConstructor {

    private readonly AccountStatementQuery AccountStatementCommand;

    internal AccountStatementConstructor(AccountStatementQuery accountStatementCommand) {
      Assertion.Require(accountStatementCommand, "accountStatementCommand");

      AccountStatementCommand = accountStatementCommand;
    }



    #region Public methods


    internal AccountStatement Build() {
      if (!AccountStatementCommand.BalancesQuery.UseCache) {
        return GenerateAccountStatement();
      }

      string hash = AccountStatementCache.GenerateHash(AccountStatementCommand);

      AccountStatement accountStatement = AccountStatementCache.TryGet(hash);
      if (accountStatement == null) {
        accountStatement = GenerateAccountStatement();
        AccountStatementCache.Store(hash, accountStatement);
      }

      return accountStatement;
    }


    internal AccountStatement GenerateAccountStatement() {
      var helper = new AccountStatementHelper(AccountStatementCommand);
      bool? isBalance = true;

      if (AccountStatementCommand.BalancesQuery.TrialBalanceType == TrialBalanceType.BalanzaValorizadaComparativa ||
          AccountStatementCommand.BalancesQuery.TrialBalanceType == TrialBalanceType.BalanzaDolarizada) {
        isBalance = null;
      }

      Assertion.Require(isBalance, $"Funcionalidad en proceso de desarrollo.");

      FixedList<AccountStatementEntry> voucherEntries = helper.GetVoucherEntries();

      FixedList<AccountStatementEntry> orderingVouchers = helper.GetOrderingVouchers(voucherEntries);

      AccountStatementEntry initialAccountBalance = helper.GetInitialAccountBalance(orderingVouchers);

      FixedList<AccountStatementEntry> vouchersWithCurrentBalance =
                                        helper.GetVouchersListWithCurrentBalance(orderingVouchers, initialAccountBalance);

      FixedList<AccountStatementEntry> vouchers = helper.CombineInitialAccountBalanceWithVouchers(
                                                            vouchersWithCurrentBalance, initialAccountBalance);

      var returnedVoucherEntries = new FixedList<IVouchersByAccountEntry>(
                                        vouchers.Select(x => (IVouchersByAccountEntry) x));

      string title = helper.GetTitle();

      return new AccountStatement(AccountStatementCommand.BalancesQuery, returnedVoucherEntries, title);
    }


    #endregion Public methods

  } // class VouchersByAccountConstructor

} // namespace Empiria.FinancialAccounting.Reporting
