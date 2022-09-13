/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Service provider                        *
*  Type     : AccountStatementBuilder                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides the services that is used to generate account statements (estados de cuenta).         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.Reporting.Adapters;

namespace Empiria.FinancialAccounting.Reporting.AccountStatements {

  /// <summary>Provides the services that is used to generate account statements (estados de cuenta).</summary>
  internal class AccountStatementBuilder {

    private readonly AccountStatementQuery _buildQuery;

    internal AccountStatementBuilder(AccountStatementQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      _buildQuery = buildQuery;
    }


    #region Methods

    internal AccountStatement Build() {

      if (!_buildQuery.BalancesQuery.UseCache) {
        return GenerateAccountStatement();
      }

      string hash = AccountStatementCache.GenerateHash(_buildQuery);

      AccountStatement accountStatement = AccountStatementCache.TryGet(hash);
      if (accountStatement == null) {
        accountStatement = GenerateAccountStatement();
        AccountStatementCache.Store(hash, accountStatement);
      }

      return accountStatement;
    }


    private AccountStatement GenerateAccountStatement() {
      var helper = new AccountStatementHelper(_buildQuery);
      
      FixedList<AccountStatementEntry> voucherEntries = helper.GetVoucherEntries();

      if (voucherEntries.Count == 0) {
        return new AccountStatement(_buildQuery.BalancesQuery, new FixedList<IVouchersByAccountEntry>(), "");
      }

      FixedList<AccountStatementEntry> orderingVouchers = helper.GetOrderingVouchers(voucherEntries);

      AccountStatementEntry initialBalance = helper.GetInitialBalance(orderingVouchers);

      FixedList<AccountStatementEntry> vouchersWithCurrentBalance =
                                        helper.GetVouchersListWithCurrentBalance(orderingVouchers, initialBalance);

      FixedList<AccountStatementEntry> vouchers = helper.CombineInitialBalanceWithVouchers(
                                                            vouchersWithCurrentBalance, initialBalance);

      var returnedVoucherEntries = new FixedList<IVouchersByAccountEntry>(
                                        vouchers.Select(x => (IVouchersByAccountEntry) x));

      string title = helper.GetTitle();

      return new AccountStatement(_buildQuery.BalancesQuery, returnedVoucherEntries, title);
    }


    #endregion Methods

  } // class AccountStatementBuilder

} // namespace Empiria.FinancialAccounting.Reporting
