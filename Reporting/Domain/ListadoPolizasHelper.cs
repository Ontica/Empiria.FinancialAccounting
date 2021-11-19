/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Helper methods                          *
*  Type     : ListadoPolizasHelper                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods to build vouchers information.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.Collections;
using Empiria.FinancialAccounting.Reporting.Adapters;
using Empiria.FinancialAccounting.Reporting.Data;
using Empiria.FinancialAccounting.Reporting.Domain;

namespace Empiria.FinancialAccounting.Reporting {


  public enum EntryType {

    PolizaNormal,

    TotalPorContabilidad,

    TotalGeneral

  }


  /// <summary>Helper methods to build vouchers information.</summary>
  internal class ListadoPolizasHelper {

    private readonly ListadoPolizasCommand _command;

    internal ListadoPolizasHelper(ListadoPolizasCommand command) {
      _command = command;
    }


    #region Public methods

    internal FixedList<PolizaEntry> GetPolizaEntries() {
      var commandExtensions = new PolizasCommandExtensions();

      PolizaCommandData commandData = commandExtensions.MapToPolizaCommandData(_command);

      FixedList<PolizaEntry> polizas = ListadoPolizasDataService.GetPolizasEntries(commandData);

      return polizas;
    }

    internal FixedList<PolizaEntry> GetListadoPolizasConTotales(FixedList<PolizaEntry> vouchers) {
      
      FixedList<PolizaEntry> totalByLedger = GetTotalByLedger(vouchers);

      FixedList<PolizaEntry> vouchersAndTotalByLedger = CombineTotalByLedgerAndVouchers(vouchers, totalByLedger);

      PolizaEntry totalOfVouchers = GetTotalFromVouchers(vouchers);

      FixedList<PolizaEntry> returnedVouchers = CombineTotalOfVouchersAndVoucherList(
                                                vouchersAndTotalByLedger, totalOfVouchers);

      return returnedVouchers;
    }


    #endregion


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
      returnedVouchers.Add(totalOfVouchers);

      return returnedVouchers.ToFixedList();
    }


    private void GenerateOrIncreasePolizaEntry(EmpiriaHashTable<PolizaEntry> summaryByLedger,
                                               PolizaEntry voucher, string hash) {
      PolizaEntry voucherEntry;

      summaryByLedger.TryGetValue(hash, out voucherEntry);

      if (voucherEntry == null) {

        voucherEntry = new PolizaEntry {
          Ledger = voucher.Ledger,
          Voucher = voucher.Voucher,
          EntryType = voucher.EntryType
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
        SummaryByVoucher(vouchersWithTotal, entry, EntryType.TotalPorContabilidad);
      }

      return vouchersWithTotal.ToFixedList();
    }


    private PolizaEntry GetTotalFromVouchers(FixedList<PolizaEntry> vouchersList) {
      var vouchersWithTotal = new EmpiriaHashTable<PolizaEntry>();

      foreach (var voucher in vouchersList.Where(a=>a.EntryType == EntryType.PolizaNormal)) {
        SummaryByVoucher(vouchersWithTotal, voucher, EntryType.TotalGeneral);
      }

      return vouchersWithTotal.ToFixedList().FirstOrDefault();
    }


    private void SummaryByVoucher(EmpiriaHashTable<PolizaEntry> entriesWithTotal,
                                 PolizaEntry entry, EntryType entryType) {

      PolizaEntry polizaEntry = PolizasMapper.MapToPolizaEntry(entry);
      polizaEntry.EntryType = entryType;

      string hash = entryType == EntryType.TotalPorContabilidad ?
                    $"{polizaEntry.Ledger.Number}" : entry.EntryType.ToString();

      GenerateOrIncreasePolizaEntry(entriesWithTotal, polizaEntry, hash);

    }


    #endregion


  } // class ListadoPolizasHelper

} // namespace Empiria.FinancialAccounting.Reporting
