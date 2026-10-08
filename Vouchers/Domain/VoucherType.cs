/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Common Storage Items                    *
*  Type     : VoucherType                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes the type of a voucher.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Describes the type of a voucher.</summary>
  public class VoucherType : CommonStorage {

    protected VoucherType() {
      // Required by Empiria Framework.
    }

    static public VoucherType Parse(int id) => ParseId<VoucherType>(id);

    static public VoucherType Parse(string uid) => ParseKey<VoucherType>(uid);

    static public VoucherType ParseWithCode(string code) => ParseWithCode<VoucherType>(code);

    static public FixedList<VoucherType> GetList() => GetStorageObjects<VoucherType>();

    static public VoucherType Empty => ParseEmpty<VoucherType>();

    #region Properties

    public bool SkipEntriesValidation {
      get {
        return base.ExtData.Get("skipEntriesValidation", false);
      }
    }

    #endregion Properties

  } // class VoucherType

}  // namespace Empiria.FinancialAccounting.Vouchers
