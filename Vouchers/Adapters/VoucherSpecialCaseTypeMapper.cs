/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Mapper class                            *
*  Type     : VoucherSpecialCaseTypeMapper               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to retrive and manage accounting vouchers.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  static internal class VoucherSpecialCaseTypeMapper {

    static internal FixedList<VoucherSpecialCaseTypeDto> Map(FixedList<VoucherSpecialCaseType> list) {
      return new FixedList<VoucherSpecialCaseTypeDto>(list.Select(x => Map(x)));
    }

    private static VoucherSpecialCaseTypeDto Map(VoucherSpecialCaseType x) {
      return new VoucherSpecialCaseTypeDto {
         UID = x.UID,
         Name = x.Name,
         AllowAllLedgersSelection = x.AllowAllLedgersSelection,
         AskForCalculationDateField = x.AskForCalculationDateField,
         AskForVoucherNumberField = x.AskForVoucherNumberField,
         CalculationDateFieldName = x.CalculationDateFieldName
      };
    }

  }  // class VoucherSpecialCaseTypeMapper

}  // namespace Empiria.FinancialAccounting.Vouchers.Adapters
