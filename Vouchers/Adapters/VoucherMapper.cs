/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Mapper class                            *
*  Type     : VoucherMapper                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for accounting vouchers.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  /// <summary>Mapping methods for accounting vouchers.</summary>
  static public class VoucherMapper {

    static internal FixedList<VoucherDescriptorDto> MapToVoucherDescriptor(FixedList<Voucher> list) {
      return new FixedList<VoucherDescriptorDto>(list.Select((x) => MapToVoucherDescriptor(x)));
    }

    static internal VoucherDescriptorDto MapToVoucherDescriptor(Voucher voucher) {
      return new VoucherDescriptorDto {
        Id = voucher.Id,
        LedgerName = voucher.Ledger.FullName,
        Number = voucher.Number,
        Concept = voucher.Concept,
        TransactionTypeName = voucher.TransactionType.Name,
        VoucherTypeName = voucher.VoucherType.Name,
        SourceName = "Banobras",
        AccountingDate = voucher.AccountingDate,
        RecordingDate = voucher.RecordingDate,
        ElaboratedBy = "María Luisa Jiménez",
        AuthorizedBy = "Rebeca Martínez Solís",
        Status = voucher.IsOpened ? "Pediente" : "Cerrado"
      };
    }

  }  // class VoucherMapper

}  // namespace Empiria.FinancialAccounting.Vouchers.Adapters
