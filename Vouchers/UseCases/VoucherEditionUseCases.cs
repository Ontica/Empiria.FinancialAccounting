/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Use case interactor class               *
*  Type     : VoucherEditionUseCases                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to edit vouchers and their postings.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.UseCases {

  /// <summary>Use cases used to edit vouchers and their postings.</summary>
  public class VoucherEditionUseCases : UseCase {

    #region Constructors and parsers

    protected VoucherEditionUseCases() {
      // no-op
    }

    static public VoucherEditionUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<VoucherEditionUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public VoucherDto CreateVoucher(VoucherFields fields) {
      throw new NotImplementedException();
    }


    public void DeleteVoucher(int voucherId) {
      throw new NotImplementedException();
    }


    public VoucherDto UpdateVoucher(int voucherId, VoucherFields fields) {
      throw new NotImplementedException();
    }


    #endregion Use cases

  }  // class VoucherEditionUseCases

}  // namespace Empiria.FinancialAccounting.Vouchers.UseCases
