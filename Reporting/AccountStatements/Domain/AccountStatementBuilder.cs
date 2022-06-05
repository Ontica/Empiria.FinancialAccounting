﻿/* Empiria Financial *****************************************************************************************
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

namespace Empiria.FinancialAccounting.Reporting {

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
      bool? isBalance = true;

      if (_buildQuery.BalancesQuery.TrialBalanceType == TrialBalanceType.BalanzaValorizadaComparativa ||
          _buildQuery.BalancesQuery.TrialBalanceType == TrialBalanceType.BalanzaDolarizada) {
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

      return new AccountStatement(_buildQuery.BalancesQuery, returnedVoucherEntries, title);
    }


    #endregion Methods

  } // class AccountStatementBuilder

} // namespace Empiria.FinancialAccounting.Reporting