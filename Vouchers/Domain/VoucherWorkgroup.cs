/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Empiria General Object                  *
*  Type     : VoucherWorkgroup                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents a group of users that can work on vouchers that meet certain conditions.            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Represents a group of users that can work on vouchers that meet certain conditions.</summary>
  internal class VoucherWorkgroup : GeneralObject {

    #region Constructors and parsers

    protected VoucherWorkgroup() {
      // Required by Empiria Framework.
    }

    static internal VoucherWorkgroup Parse(int id) {
      return BaseObject.ParseId<VoucherWorkgroup>(id);
    }


    static internal VoucherWorkgroup Parse(string uid) {
      return BaseObject.ParseKey<VoucherWorkgroup>(uid);
    }


    static internal FixedList<VoucherWorkgroup> GetList() {
      return BaseObject.GetList<VoucherWorkgroup>(string.Empty, "ObjectName")
                       .ToFixedList();
    }


    static internal FixedList<VoucherWorkgroup> GetListFor(Participant participant) {
      FixedList<VoucherWorkgroup> allWorkgroups = GetList();

      return allWorkgroups.FindAll(x => x.Members.Contains(participant));
    }


    static internal VoucherWorkgroup Empty => BaseObject.ParseEmpty<VoucherWorkgroup>();


    #endregion Constructors and parsers

    #region Properties

    internal FixedList<Participant> Members {
      get {
        return base.ExtendedDataField.GetFixedList<Participant>("members");
      }
    }


    internal string VouchersCondition {
      get {
        return base.ExtendedDataField.Get<string>("vouchersCondition", string.Empty);
      }
    }

    #endregion Properties

  } // class VoucherWorkgroup

}  // namespace Empiria.FinancialAccounting.Vouchers
