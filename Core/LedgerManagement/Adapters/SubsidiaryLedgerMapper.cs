/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Mapper class                            *
*  Type     : SubsidiaryLedgerMapper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods for subsidiary ledger books.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Mapping methods for subsidiary ledger books.</summary>
  static public class SubsidiaryLedgerMapper {

    static internal FixedList<SubsidiaryLedgerDto> Map(FixedList<SubsidiaryLedger> subledgers) {
      return new FixedList<SubsidiaryLedgerDto>(subledgers.Select(x => Map(x)));
    }


    static internal SubsidiaryLedgerDto Map(SubsidiaryLedger subsidiaryLedger) {
      return new SubsidiaryLedgerDto {
        UID = subsidiaryLedger.UID,
        TypeName = subsidiaryLedger.SubsidiaryLedgerType.Name,
        Name = subsidiaryLedger.Name,
        Description = subsidiaryLedger.Description,
        AccountsPrefix = subsidiaryLedger.AccountsPrefix,
        BaseLedger = subsidiaryLedger.BaseLedger.MapToNamedEntity()
      };
    }


    static public FixedList<SubsidiaryAccountDto> Map(FixedList<SubsidiaryAccount> list) {
      return new FixedList<SubsidiaryAccountDto>(list.Select(x => MapAccount(x)));
    }


    static public SubledgerAccountDescriptorDto MapAccountToDescriptor(SubsidiaryAccount subledgerAccount) {
      return new SubledgerAccountDescriptorDto {
        Id = subledgerAccount.Id,
        Number = subledgerAccount.Number,
        Name = subledgerAccount.Name,
        FullName = $"{subledgerAccount.Number} - {subledgerAccount.Name}"
      };
    }


    static internal SubsidiaryAccountDto MapAccount(SubsidiaryAccount subledgerAccount) {
      return new SubsidiaryAccountDto {
        Id = subledgerAccount.Id,
        Ledger = subledgerAccount.SubsidaryLedger.BaseLedger.MapToNamedEntity(),
        SubsidiaryLedger = subledgerAccount.SubsidaryLedger.MapToNamedEntity(),
        Name = subledgerAccount.Name,
        Keywords = EmpiriaString.BuildKeywords(subledgerAccount.Name,
                                               subledgerAccount.Number,
                                               subledgerAccount.SubsidaryLedger.Name),
        Number = subledgerAccount.Number,
        Description = subledgerAccount.Description
      };
    }

  }  // class SubsidiaryLedgerMapper

}  // namespace Empiria.FinancialAccounting.Adapters
