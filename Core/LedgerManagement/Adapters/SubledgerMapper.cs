/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Mapper class                            *
*  Type     : SubledgerMapper                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for subledger books and subledger accounts.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Mapping methods for subledger books and subledger accounts.</summary>
  static public class SubledgerMapper {

    static internal FixedList<SubledgerDto> Map(FixedList<Subledger> subledgers) {
      return new FixedList<SubledgerDto>(subledgers.Select(x => Map(x)));
    }


    static internal SubledgerDto Map(Subledger subledger) {
      return new SubledgerDto {
        UID = subledger.UID,
        TypeName = subledger.SubledgerType.Name,
        Name = subledger.Name,
        Description = subledger.Description,
        AccountsPrefix = subledger.AccountsPrefix,
      };
    }


    static public FixedList<SubledgerAccountDto> Map(FixedList<SubledgerAccount> list) {
      return new FixedList<SubledgerAccountDto>(list.Select(x => Map(x)));
    }


    static internal SubledgerAccountDto Map(SubledgerAccount subledgerAccount) {
      return new SubledgerAccountDto {
        Id = subledgerAccount.Id,
        Type = subledgerAccount.Subledger.SubledgerType.MapToNamedEntity(),
        AccountsChartUID = subledgerAccount.Ledger.AccountsChart.UID,
        Ledger = subledgerAccount.Ledger.MapToNamedEntity(),
        Name = subledgerAccount.Name,
        Number = subledgerAccount.Number,
        Description = subledgerAccount.Description,
        Suspended = subledgerAccount.Suspended,
        Lists = new FixedList<NamedEntityDto>()
      };
    }


    static public ShortFlexibleEntityDto MapToShortFlexibleEntityDto(SubledgerAccount subledgerAccount) {
      return new ShortFlexibleEntityDto {
        Id = subledgerAccount.Id,
        Number = subledgerAccount.Number,
        Name = subledgerAccount.Name
      };
    }

    static public FixedList<SubledgerAccountDescriptorDto> MapToSubledgerAccountDescriptor(FixedList<SubledgerAccount> list) {
      return new FixedList<SubledgerAccountDescriptorDto>(list.Select(x => MapToSubledgerAccountDescriptor(x)));
    }


    static public SubledgerAccountDescriptorDto MapToSubledgerAccountDescriptor(SubledgerAccount subledgerAccount) {
      return new SubledgerAccountDescriptorDto {
        Id = subledgerAccount.Id,
        LedgerName = subledgerAccount.Ledger.FullName,
        TypeName = subledgerAccount.Subledger.SubledgerType.Name,
        Number = subledgerAccount.Number,
        Name = subledgerAccount.Name,
        Suspended = subledgerAccount.Suspended,
        FullName = $"{subledgerAccount.Number} - {subledgerAccount.Name}"
      };
    }

  }  // class SubledgerMapper

}  // namespace Empiria.FinancialAccounting.Adapters
