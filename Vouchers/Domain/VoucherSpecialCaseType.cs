/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Empiria General Object                  *
*  Type     : VoucherSpecialCaseType                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes the coniguration of  a voucher special case type.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Describes a voucher special case type with its configuration.</summary>
  internal class VoucherSpecialCaseType : GeneralObject {

    protected VoucherSpecialCaseType() {
      // Required by Empiria Framework.
    }

    static public VoucherSpecialCaseType Parse(int id) {
      return BaseObject.ParseId<VoucherSpecialCaseType>(id);
    }


    static public VoucherSpecialCaseType Parse(string uid) {
      return BaseObject.ParseKey<VoucherSpecialCaseType>(uid);
    }


    static internal FixedList<VoucherSpecialCaseType> GetList() {
      return BaseObject.GetList<VoucherSpecialCaseType>(string.Empty, "ObjectName")
                       .ToFixedList();
    }

    #region Properties


    public bool AskForCalculationDateField {
      get {
        return base.ExtendedDataField.Get("askForCalculationDateField", false);
      }
    }


    public bool AskForVoucherNumberField {
      get {
        return base.ExtendedDataField.Get("askForVoucherNumberField", false);
      }
    }


    public string CalculationDateFieldName {
      get {
        return base.ExtendedDataField.Get("calculationDateFieldName", "Fecha");
      }
    }


    public VoucherType VoucherType {
      get {
        return base.ExtendedDataField.Get<VoucherType>("voucherTypeId");
      }
    }

    #endregion Properties

  }  // class VoucherSpecialCaseType

}  // namespace Empiria.FinancialAccounting.Vouchers
