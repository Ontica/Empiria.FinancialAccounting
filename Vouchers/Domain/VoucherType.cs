/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Empiria General Object                  *
*  Type     : VoucherType                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes the type of a voucher.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Describes the type of a voucher.</summary>
  public class VoucherType : GeneralObject {

    protected VoucherType() {
      // Required by Empiria Framework.
    }

    static public VoucherType Parse(int id) {
      return BaseObject.ParseId<VoucherType>(id);
    }


    static public VoucherType Parse(string uid) {
      return BaseObject.ParseKey<VoucherType>(uid);
    }


    static public FixedList<VoucherType> GetList() {
      return BaseObject.GetList<VoucherType>(string.Empty, "ObjectName")
                       .ToFixedList();
    }

    static public VoucherType Empty => BaseObject.ParseEmpty<VoucherType>();

  } // class VoucherType

}  // namespace Empiria.FinancialAccounting.Vouchers
