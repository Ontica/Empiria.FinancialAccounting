/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Input Data Holder                       *
*  Type     : VoucherSpecialCaseFields                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data structure that serves as an adapter to create special case vouchers.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers.Adapters {

  /// <summary>Data structure that serves as an adapter to create special case vouchers.</summary>
  public class VoucherSpecialCaseFields : VoucherFields {

    public string AccountsChartUID {
      get; set;
    } = string.Empty;


    public DateTime CalculationDate {
      get; set;
    } = ExecutionServer.DateMinValue;


    public string OnVoucherNumber {
      get; set;
    } = string.Empty;


    internal VoucherSpecialCaseFields Copy() {
      return (VoucherSpecialCaseFields) this.MemberwiseClone();
    }

  }  // class VoucherSpecialCaseFields

}  // namespace Empiria.FinancialAccounting.Vouchers.Adapters
