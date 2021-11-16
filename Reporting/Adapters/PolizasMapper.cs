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

    static internal PolizasDto Map(PolizasBuilder polizas) {

      return new PolizasDto {
        Command = polizas.Command,
        Entries = Map(polizas.Command, polizas.Entries)
      };

    }

    static private FixedList<IPolizasDto> Map(PolizasCommand command, 
                                              FixedList<IPolizaEntry> list) {

      var mappedItems = list.Select((x) => MapToPolizas((PolizaEntry) x, command));
      return new FixedList<IPolizasDto>(mappedItems);

    }

    static private PolizasEntryDto MapToPolizas(PolizaEntry entry, PolizasCommand command) {

      var dto = new PolizasEntryDto();

      dto.LedgerNumber = entry.Ledger.Number;
      dto.LedgerName = entry.Ledger.Name;
      dto.VoucherNumber = entry.Voucher.Number;
      dto.AccountingDate = entry.Voucher.AccountingDate;
      dto.RecordingDate = entry.Voucher.RecordingDate;
      dto.ElaboratedBy = entry.Voucher.ElaboratedBy.Name;
      dto.Concept = entry.Voucher.Concept;
      dto.Debit = entry.Debit;
      dto.Credit = entry.Credit;

      return dto;
    }
  } // class PolizasMapper

} // namespace Empiria.FinancialAccounting.Reporting.Adapters
