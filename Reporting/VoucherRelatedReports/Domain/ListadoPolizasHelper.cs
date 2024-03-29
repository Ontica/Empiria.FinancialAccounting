﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Helper methods                          *
*  Type     : ListadoPolizasHelper                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build voucher list information.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;

using Empiria.FinancialAccounting.Reporting.Data;

using Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Adapters;

namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain {


  public enum ItemType {

    Entry,

    Group,

    Total

  }


  /// <summary>Helper methods to build voucher list information.</summary>
  internal class ListadoPolizasHelper {

    private readonly ListadoPolizasQuery _query;

    internal ListadoPolizasHelper(ListadoPolizasQuery query) {
      Assertion.Require(query, nameof(query));

      _query = query;
    }


    #region Public methods

    internal FixedList<PolizaEntry> SetTotalsRows(FixedList<PolizaEntry> vouchers) {
      FixedList<PolizaEntry> totalByLedger = GetTotalByLedger(vouchers);

      FixedList<PolizaEntry> vouchersAndTotalByLedger = CombineTotalByLedgerAndVouchers(vouchers, totalByLedger);

      PolizaEntry totalOfVouchers = GetTotalFromVouchers(vouchers);

      FixedList<PolizaEntry> returnedVouchers = CombineTotalOfVouchersAndVoucherList(
                                                vouchersAndTotalByLedger, totalOfVouchers);

      return returnedVouchers;
    }


    internal FixedList<PolizaEntry> GetPolizaEntries() {
      var builder = new ListadoPolizasSqlClausesBuilder(_query);

      ListadoPolizasSqlClauses sqlClauses = builder.Build();

      return ListadoPolizasDataService.GetPolizasEntries(sqlClauses);
    }


    #endregion Public methods

    #region Private methods

    private FixedList<PolizaEntry> CombineTotalByLedgerAndVouchers(
                                    FixedList<PolizaEntry> list, FixedList<PolizaEntry> totalByLedger) {

      var returnedList = new List<PolizaEntry>();

      foreach (var ledger in totalByLedger) {
        var vouchersWithThisLedger = list.Where(a => a.Ledger.Number == ledger.Ledger.Number).ToList();

        vouchersWithThisLedger.Add(ledger);
        returnedList.AddRange(vouchersWithThisLedger);
      }

      return returnedList.ToFixedList();
    }


    private FixedList<PolizaEntry> CombineTotalOfVouchersAndVoucherList(
                                    FixedList<PolizaEntry> vouchersAndTotalByLedger,
                                    PolizaEntry totalOfVouchers) {

      var returnedVouchers = vouchersAndTotalByLedger.ToList();
      if (totalOfVouchers != null) {
        returnedVouchers.Add(totalOfVouchers);
      }

      return returnedVouchers.ToFixedList();
    }


    private void GenerateOrIncreasePolizaEntry(EmpiriaHashTable<PolizaEntry> summaryByLedger,
                                               PolizaEntry voucher, string hash) {
      PolizaEntry voucherEntry;

      summaryByLedger.TryGetValue(hash, out voucherEntry);

      if (voucherEntry == null) {

        voucherEntry = new PolizaEntry {
          Ledger = voucher.Ledger,
          Number = voucher.Number,
          AccountingDate = voucher.AccountingDate,
          RecordingDate = voucher.RecordingDate,
          ElaboratedBy = voucher.ElaboratedBy,
          Concept = voucher.Concept,
          ItemType = voucher.ItemType
        };
        voucherEntry.Sum(voucher);

        summaryByLedger.Insert(hash, voucherEntry);

      } else {
        voucherEntry.Sum(voucher);
      }
    }


    private FixedList<PolizaEntry> GetTotalByLedger(FixedList<PolizaEntry> list) {
      var vouchersWithTotal = new EmpiriaHashTable<PolizaEntry>(list.Count);

      foreach (var entry in list) {
        SummaryByVoucher(vouchersWithTotal, entry, ItemType.Group);
      }

      return vouchersWithTotal.ToFixedList();
    }


    private PolizaEntry GetTotalFromVouchers(FixedList<PolizaEntry> vouchersList) {
      var vouchersWithTotal = new EmpiriaHashTable<PolizaEntry>();

      foreach (var voucher in vouchersList.Where(a => a.ItemType == ItemType.Entry)) {
        SummaryByVoucher(vouchersWithTotal, voucher, ItemType.Total);
      }

      return vouchersWithTotal.ToFixedList().FirstOrDefault();
    }


    private void SummaryByVoucher(EmpiriaHashTable<PolizaEntry> entriesWithTotal,
                                 PolizaEntry entry, ItemType entryType) {

      PolizaEntry polizaEntry = PolizasMapper.MapToPolizaEntry(entry);
      polizaEntry.ItemType = entryType;

      string hash = entryType == ItemType.Group ?
                    $"{polizaEntry.Ledger.Number}" : entry.ItemType.ToString();

      GenerateOrIncreasePolizaEntry(entriesWithTotal, polizaEntry, hash);

    }

    #endregion Private methods

  } // class ListadoPolizasHelper

} // namespace Empiria.FinancialAccounting.Reporting.VoucherRelatedReports.Domain
