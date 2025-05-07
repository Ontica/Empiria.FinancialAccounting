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


    public AccountsList AccountsList {
      get {
        return base.ExtendedDataField.Get<AccountsList>("accountsListId", AccountsList.Empty);
      }
    }


    public bool AllowAllLedgersSelection {
      get {
        return base.ExtendedDataField.Get("allowAllLedgersSelection", false);
      }
    }

    public bool AllowAllChildrenLedgersSelection {
      get {
        return base.ExtendedDataField.Get("allowAllChildrenLedgersSelection", false);
      }
    }

    public bool AskForCalculationDateField {
      get {
        return base.ExtendedDataField.Get("askForCalculationDateField", false);
      }
    }


    public bool AskForDatePeriodField {
      get {
        return base.ExtendedDataField.Get("askForDatePeriodField", false);
      }
    }


    public bool AskForVoucherNumberField {
      get {
        return base.ExtendedDataField.Get("askForVoucherNumberField", false);
      }
    }


    public bool SkipEntriesValidation {
      get {
        return base.ExtendedDataField.Get("skipEntriesValidation", false);
      }
    }

    public string CalculationDateFieldName {
      get {
        return base.ExtendedDataField.Get("calculationDateFieldName", "Fecha");
      }
    }


    public string DatePeriodFieldName {
      get {
        return base.ExtendedDataField.Get("datePeriodFieldName", "Período");
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
