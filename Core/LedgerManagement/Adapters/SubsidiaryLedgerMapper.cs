﻿/* Empiria Financial *****************************************************************************************
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
  static internal class SubsidiaryLedgerMapper {

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


    static internal SubsidiaryAccountDto MapAccount(SubsidiaryAccount subsidiaryAccount) {
      return new SubsidiaryAccountDto {
        Id = subsidiaryAccount.Id,
        SubsidiaryLedger = subsidiaryAccount.SubsidaryLedger.MapToNamedEntity(),
        Name = subsidiaryAccount.Name,
        Number = subsidiaryAccount.Number,
        Description = subsidiaryAccount.Description
      };
    }

  }  // class SubsidiaryLedgerMapper

}  // namespace Empiria.FinancialAccounting.Adapters