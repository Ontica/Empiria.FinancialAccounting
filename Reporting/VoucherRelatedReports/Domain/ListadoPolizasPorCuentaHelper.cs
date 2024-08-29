/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Helper methods                          *
*  Type     : ListadoPolizasPorCuentaHelper              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build voucher list by account information.                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;

using Empiria.FinancialAccounting.BalanceEngine;
using Empiria.FinancialAccounting.Reporting.Data;
using Empiria.FinancialAccounting.Reporting.AccountStatements;
using Empiria.FinancialAccounting.Reporting.AccountStatements.Domain;

namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain {

  /// <summary>Helper methods to build voucher list by account information.</summary>
  internal class ListadoPolizasPorCuentaHelper {

    private readonly ReportBuilderQuery _buildQuery;

    public ListadoPolizasPorCuentaHelper(ReportBuilderQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      _buildQuery = buildQuery;
    }


    #region Public methods

    internal FixedList<AccountStatementEntry> CombineVouchersWithTotalByCurrency(
                                              FixedList<AccountStatementEntry> orderingVouchers,
                                              FixedList<AccountStatementEntry> totalsByCurrency) {
      var returnedEntries = new List<AccountStatementEntry>();

      foreach (var total in totalsByCurrency) {

        var vouchers = orderingVouchers.FindAll(a => a.Currency.Equals(total.Currency));

        if (vouchers.Count > 0) {
          returnedEntries.AddRange(vouchers);
          returnedEntries.Add(total);
        }
      }
      return returnedEntries.ToFixedList();
    }


    internal FixedList<AccountStatementEntry> GenerateTotalSummaryByCurrency(
                                              FixedList<AccountStatementEntry> vouchers) {

      var totalSummaryByCurrency = new EmpiriaHashTable<AccountStatementEntry>();

      foreach (var entry in vouchers.Where(a => !a.HasParentPostingEntry)) {
        SummaryEntriesByCurrency(totalSummaryByCurrency, entry);
      }

      return totalSummaryByCurrency.ToFixedList();
    }


    internal FixedList<AccountStatementEntry> GetSummaryToParentVouchers(
                                              FixedList<AccountStatementEntry> vouchers) {
      var returnedEntries = new List<AccountStatementEntry>(vouchers);

      foreach (var entry in vouchers) {
        StandardAccount currentParent = StandardAccount.Parse(entry.StandardAccountId).GetParent();

        var entryParent = returnedEntries.Find(a => a.AccountNumber == currentParent.Number &&
                                                    a.Currency.Equals(entry.Currency) &&
                                                    a.Ledger.Number == entry.Ledger.Number &&
                                                    a.Sector.Code == entry.Sector.Code &&
                                                    a.DebtorCreditor == entry.DebtorCreditor);
        if (entryParent != null) {
          entry.HasParentPostingEntry = true;
          entryParent.IsParentPostingEntry = true;
          entryParent.Sum(entry);
        }
      }

      return returnedEntries.ToFixedList();
    }


    internal FixedList<AccountStatementEntry> GetVoucherEntries() {
      var builder = new PolizasPorCuentaSqlClausesBuilder(_buildQuery);

      ListadoPolizasSqlClauses sqlClauses = builder.Build();

      return ListadoPolizasPorCuentaDataService.GetVouchersByAccountEntries(sqlClauses);
    }


    internal FixedList<AccountStatementEntry> OrderingVouchers(FixedList<AccountStatementEntry> vouchers) {

      if (_buildQuery.ReportType.Equals(ReportTypes.MovimientosPorNumeroDeVerificacion)) {

        var ordering = vouchers.OrderBy(a => a.VerificationNumber)
                             .ThenBy(a => a.Currency.Code)
                             .ThenBy(a => a.Ledger.Number)
                             .ThenBy(a => a.AccountingDate)
                             .ThenBy(a => a.VoucherNumber)
                             .ThenBy(a => a.AccountNumber)
                             .ToList();
        return ordering.ToFixedList();
      } else {

        var ordering = vouchers.OrderBy(a => a.Currency.Code)
                             .ThenBy(a => a.Ledger.Number)
                             .ThenBy(a => a.AccountingDate)
                             .ThenBy(a => a.VoucherNumber)
                             .ThenBy(a => a.AccountNumber)
                             .ThenBy(a => a.SubledgerAccountNumber)
                             .ToList();
        return ordering.ToFixedList();
      }
    }


    #endregion Public methods


    #region Private methods


    private void GenerateOrIncreaseTotals(EmpiriaHashTable<AccountStatementEntry> summaryEntries,
                  AccountStatementEntry entry, string hash, TrialBalanceItemType itemType) {
      AccountStatementEntry summaryEntry;

      summaryEntries.TryGetValue(hash, out summaryEntry);

      if (summaryEntry == null) {

        summaryEntry = new AccountStatementEntry {
          Ledger = Ledger.Empty,
          Currency = entry.Currency,
          Sector = Sector.Empty,
          VoucherId = entry.VoucherId,
          VoucherEntryId = entry.VoucherEntryId,
          AccountNumber = entry.AccountNumber,
          AccountName = entry.AccountName,
          DebtorCreditor = entry.DebtorCreditor,
          AccountingDate = entry.AccountingDate,
          RecordingDate = entry.RecordingDate,
          Concept = entry.Concept,
          IsCurrentBalance = entry.IsCurrentBalance,
          ItemType = itemType
        };

        summaryEntry.Sum(entry);

        summaryEntries.Insert(hash, summaryEntry);

      } else {

        summaryEntry.Sum(entry);
      }
    }


    private void SummaryEntriesByCurrency(EmpiriaHashTable<AccountStatementEntry> totalSummaryByCurrency,
                                          AccountStatementEntry entry) {

      AccountStatementEntry newEntry = AccountStatementEntry.MapToAccountStatementEntry(entry);

      newEntry.AccountName = "Suma de movimientos moneda " + newEntry.Currency.Code;

      if (newEntry.DebtorCreditor.ToString() == "A") {
        newEntry.Debit = -1 * entry.Debit;
        newEntry.Credit = -1 * entry.Credit;
        newEntry.CurrentBalance = -1 * entry.CurrentBalance;
      }

      string hash = $"{newEntry.Currency.Code}";

      GenerateOrIncreaseTotals(totalSummaryByCurrency, newEntry, hash, TrialBalanceItemType.Total);
    }


    #endregion Private methods

  } // class ListadoPolizasPorCuentaHelper

} // namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain
