/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Use case interactor class               *
*  Type     : VouchersDataUseCases                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to retrive vouchers related data.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers.UseCases {

  /// <summary>Use cases used to retrive vouchers related data.</summary>
  public class VouchersDataUseCases : UseCase {

    #region Constructors and parsers

    protected VouchersDataUseCases() {
      // no-op
    }

    static public VouchersDataUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<VouchersDataUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<NamedEntityDto> TransactionTypes() {
      return TransactionType.GetList().MapToNamedEntityList();
    }


    public FixedList<NamedEntityDto> VoucherTypes() {
      return VoucherType.GetList().MapToNamedEntityList();
    }

    #endregion Use cases

  }  // class VouchersDataUseCases

}  // namespace Empiria.FinancialAccounting.Vouchers.UseCases
