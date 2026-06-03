/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification                           Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Use case interactor class               *
*  Type     : VoucherReclassificationServices            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides services that reclassify vouchers.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Vouchers;
using Empiria.FinancialAccounting.Vouchers.Data;

namespace Empiria.FinancialAccounting.Reclassification.Services {

  /// <summary>Provides services that reclassify vouchers.</summary>
  public class VoucherReclassificationServices : Service {

    #region Constructors and parsers

    protected VoucherReclassificationServices() {
      // no-op
    }

    static public VoucherReclassificationServices ServiceInteractor() {
      return Service.CreateInstance<VoucherReclassificationServices>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public void ReclassifyVouchers(DateTime fromDate, DateTime toDate) {
      FixedList<Voucher> vouchers = VoucherData.GetVouchers(fromDate, toDate);

      foreach (Voucher voucher in vouchers) {
        var reclassificator = new VoucherEntriesReclassificator(voucher);

        reclassificator.Execute();
      }
    }

    #endregion Use cases

  } // class VoucherReclassificationServices

} // namespace Empiria.FinancialAccounting.Reclassification.Services
