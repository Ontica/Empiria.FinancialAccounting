/* Empiria Financial *****************************************************************************************
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

      return new ToImportVoucher(header, entries);
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
      var entry = new ToImportVoucherEntry(header) {
        StandardAccount = movimiento.GetStandardAccount(),
        Sector = movimiento.GetSector(),
        SubledgerAccount = movimiento.GetSubledgerAccount(),
        SubledgerAccountNo = movimiento.GetSubledgerAccountNo(),
        ResponsibilityArea = movimiento.GetResponsibilityArea(),
        BudgetConcept = movimiento.GetBudgetConcept(),
        EventType = movimiento.GetEventType(),
        VerificationNumber = movimiento.GetVerificationNumber(),
        VoucherEntryType = movimiento.GetVoucherEntryType(),
        Date = movimiento.GetDate(),
        Concept = movimiento.GetConcept(),
        Currency = movimiento.GetCurrency(),
        Amount = movimiento.GetAmount(),
        ExchangeRate = movimiento.GetExchangeRate(),
        BaseCurrencyAmount = movimiento.GetBaseCurrencyAmount(),
        DataSource = movimiento.GetVoucherUniqueID(),
        Protected = movimiento.GetProtected()
      };

      entry.AddIssues(movimiento.GetIssues());

      return entry;
    }

  }  // class DbVouchersStructurer

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
