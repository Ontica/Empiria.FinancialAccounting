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

    static internal PolizasDto Map(ListadoPolizasBuilder polizas) {

      return new PolizasDto {
        Command = polizas.Command,
        Entries = Map(polizas.Entries)
      };

    }


    static internal PolizaEntry MapToPolizaEntry(PolizaEntry entry) {
      var newEntry = new PolizaEntry();

      newEntry.Ledger = entry.Ledger;
      newEntry.Number = entry.Number;
      newEntry.AccountingDate = entry.AccountingDate;
      newEntry.RecordingDate = entry.RecordingDate;
      newEntry.ElaboratedBy = entry.ElaboratedBy;
      newEntry.Concept = EmpiriaString.Clean(entry.Concept);
      newEntry.Debit = entry.Debit;
      newEntry.Credit = entry.Credit;
      newEntry.ItemType = entry.ItemType;

      return newEntry;
    }

    #endregion Public methods


    #region Private methods

    static private FixedList<IPolizasDto> Map(FixedList<IPolizaEntry> list) {

      var mappedItems = list.Select((x) => MapToPolizas((PolizaEntry) x));
      return new FixedList<IPolizasDto>(mappedItems);

    }


    static private PolizasEntryDto MapToPolizas(PolizaEntry entry) {

      var dto = new PolizasEntryDto();

      dto.LedgerNumber = entry.Ledger.Number ?? Ledger.Empty.Number;
      dto.LedgerName = entry.Ledger.FullName ?? Ledger.Empty.FullName;
      dto.VoucherNumber = entry.Number;
      dto.AccountingDate = entry.AccountingDate;
      dto.RecordingDate = entry.RecordingDate;
      dto.ElaboratedBy = entry.ElaboratedBy.Name;
      dto.Concept = EmpiriaString.Clean(entry.Concept);
      dto.Debit = entry.Debit;
      dto.Credit = entry.Credit;
      dto.VouchersByLedger = entry.VouchersByLedger;
      dto.ItemType = entry.ItemType;

      return dto;
    }

    #endregion Private methods


  } // class PolizasMapper

} // namespace Empiria.FinancialAccounting.Reporting.Adapters
