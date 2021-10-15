﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Structurer                           *
*  Type     : DbVouchersStructurer                          License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Holds a voucher's structure coming from database tables.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  /// <summary>Holds a voucher's structure coming from database tables.</summary>
  internal class DbVouchersStructurer {

    private readonly FixedList<Encabezado> _encabezados;
    private readonly FixedList<Movimiento> _movimientos;

    internal DbVouchersStructurer(FixedList<Encabezado> encabezados, FixedList<Movimiento> movimientos) {
      Assertion.AssertObject(encabezados, "encabezados");
      Assertion.AssertObject(movimientos, "movimientos");

      _encabezados = encabezados;
      _movimientos = movimientos;
    }


    internal FixedList<ToImportVoucher> GetToImportVouchersList() {
      var vouchersListToImport = new List<ToImportVoucher>(_encabezados.Count);

      foreach (Encabezado encabezado in _encabezados) {
        var voucherToImport = BuildVoucherToImport(encabezado);

        vouchersListToImport.Add(voucherToImport);
      }

      return vouchersListToImport.ToFixedList();
    }


    private ToImportVoucher BuildVoucherToImport(Encabezado encabezado) {
      ToImportVoucherHeader header = MapEncabezadoToImportVoucherHeader(encabezado);
      FixedList<ToImportVoucherEntry> entries = MapMovimientosToToImportVoucherEntries(encabezado, header);

      var toImportVoucher = new ToImportVoucher(header, entries);

      //if (entries.Count < 2) {
      //  toImportVoucher.AddIssue(VoucherIssueType.Error,
      //                           "La póliza no tiene movimientos. No puede importarse.");
      //}

      return toImportVoucher;
    }


    private FixedList<ToImportVoucherEntry> MapMovimientosToToImportVoucherEntries(Encabezado encabezado,
                                                                                   ToImportVoucherHeader header) {
      var entries = _movimientos.FindAll(x => x.GetVoucherUniqueID() == header.UniqueID);

      entries = new FixedList<Movimiento>(entries.Select<Movimiento>(x => { x.SetEncabezado(encabezado); return x; } ));

      var mapped = entries.Select(x => MapMovimientoToStandardVoucherEntry(header, x));

      return new List<ToImportVoucherEntry>(mapped).ToFixedList();
    }


    private ToImportVoucherHeader MapEncabezadoToImportVoucherHeader(Encabezado encabezado) {
      var header = new ToImportVoucherHeader();

      header.ImportationSet = encabezado.GetImportationSet();
      header.UniqueID = encabezado.GetUniqueID();
      header.Ledger = encabezado.GetLedger();
      header.Concept = encabezado.GetConcept();
      header.AccountingDate = encabezado.GetAccountingDate();
      header.VoucherType = encabezado.GetVoucherType();
      header.TransactionType = encabezado.GetTransactionType();
      header.FunctionalArea = encabezado.GetFunctionalArea();
      header.RecordingDate = encabezado.GetRecordingDate();
      header.ElaboratedBy = encabezado.GetElaboratedBy();

      header.Issues = encabezado.GetIssues();

      return header;
    }


    private ToImportVoucherEntry MapMovimientoToStandardVoucherEntry(ToImportVoucherHeader header,
                                                                     Movimiento movimiento) {
      var entry = new ToImportVoucherEntry(header);

      entry.LedgerAccount = movimiento.GetLedgerAccount();
      entry.StandardAccount = movimiento.GetStandardAccount();
      entry.Sector = movimiento.GetSector();
      entry.SubledgerAccount = movimiento.GetSubledgerAccount();
      entry.SubledgerAccountNo = movimiento.GetSubledgerAccountNo();
      entry.ResponsibilityArea = movimiento.GetResponsibilityArea();
      entry.BudgetConcept = movimiento.GetBudgetConcept();
      entry.EventType = movimiento.GetEventType();
      entry.VerificationNumber = movimiento.GetVerificationNumber();
      entry.VoucherEntryType = movimiento.GetVoucherEntryType();
      entry.Date = movimiento.GetDate();
      entry.Concept = movimiento.GetConcept();
      entry.Currency = movimiento.GetCurrency();
      entry.Amount = movimiento.GetAmount();
      entry.ExchangeRate = movimiento.GetExchangeRate();
      entry.BaseCurrencyAmount = movimiento.GetBaseCurrencyAmount();
      entry.Protected = movimiento.GetProtected();

      entry.Issues = movimiento.GetIssues();

      return entry;
    }

  }  // class DbVouchersStructurer

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter