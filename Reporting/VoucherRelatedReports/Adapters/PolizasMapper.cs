/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Mapper class                            *
*  Type     : PolizasMapper                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map vouchers.                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Reporting.Domain;

namespace Empiria.FinancialAccounting.Reporting.Adapters {

  /// <summary>Methods used to map vouchers.</summary>
  static internal class PolizasMapper {

    #region Public methods

    static internal PolizasDto Map(FixedList<IPolizaEntry> polizas, ListadoPolizasQuery query) {
      return new PolizasDto {
        Query = query,
        Entries = Map(polizas)
      };
    }


    static internal PolizaEntry MapToPolizaEntry(PolizaEntry entry) {
      return new PolizaEntry {
        ItemType = entry.ItemType,
        Ledger = entry.Ledger,
        Number = entry.Number,
        AccountingDate = entry.AccountingDate,
        RecordingDate = entry.RecordingDate,
        ElaboratedBy = entry.ElaboratedBy,
        Concept = EmpiriaString.Clean(entry.Concept),
        Debit = entry.Debit,
        Credit = entry.Credit
      };
    }

    #endregion Public methods


    #region Helpers

    static private FixedList<IPolizasDto> Map(FixedList<IPolizaEntry> list) {
      var mappedItems = list.Select((x) => MapToPolizas((PolizaEntry) x));

      return new FixedList<IPolizasDto>(mappedItems);
    }


    static private PolizasEntryDto MapToPolizas(PolizaEntry entry) {
      return new PolizasEntryDto() {
        LedgerNumber = entry.Ledger.Number ?? Ledger.Empty.Number,
        LedgerName = entry.Ledger.FullName ?? Ledger.Empty.FullName,
        VoucherNumber = entry.Number,
        AccountingDate = entry.AccountingDate,
        RecordingDate = entry.RecordingDate,
        ElaboratedBy = entry.ElaboratedBy.Name,
        Concept = EmpiriaString.Clean(entry.Concept),
        Debit = entry.Debit,
        Credit = entry.Credit,
        VouchersByLedger = entry.VouchersByLedger,
        ItemType = entry.ItemType,
      };
    }

    #endregion Helpers

  } // class PolizasMapper

} // namespace Empiria.FinancialAccounting.Reporting.Adapters
