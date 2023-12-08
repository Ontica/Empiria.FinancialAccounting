/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Information Holder                      *
*  Type     : VoucherSpecialCaseTypeDto                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : DTO used to return voucher special case types.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  /// <summary>DTO used to return voucher special case types.</summary>
  public class VoucherSpecialCaseTypeDto {

    public string UID {
      get; internal set;
    } = string.Empty;


    public string Name {
      get; internal set;
    } = string.Empty;


    public bool AllowAllLedgersSelection {
      get; internal set;
    }

    public bool AllowAllChildrenLedgersSelection {
      get; internal set;
    }

    public bool AskForCalculationDateField {
      get; internal set;
    }

    public bool AskForVoucherNumberField {
      get; internal set;
    }

    public string CalculationDateFieldName {
      get; internal set;
    } = "Fecha";

  }  // class VoucherSpecialCaseTypeDto

}  // namespace Empiria.FinancialAccounting.Vouchers.Adapters
