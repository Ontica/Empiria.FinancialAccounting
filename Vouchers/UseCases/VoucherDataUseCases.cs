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

namespace Empiria.FinancialAccounting.Vouchers.UseCases {

  /// <summary>Use cases used to retrive vouchers related data.</summary>
  public class VoucherDataUseCases : UseCase {

    #region Constructors and parsers

    protected VoucherDataUseCases() {
      // no-op
    }

    static public VoucherDataUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<VoucherDataUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<NamedEntityDto> FunctionalAreas() {
      return FunctionalArea.GetList()
                           .MapToNamedEntityList();
    }


    public FixedList<DateTime> OpenedAccountingDates(string ledgerUID) {
      Assertion.AssertObject(ledgerUID, "ledgerUID");

      var ledger = Ledger.Parse(ledgerUID);

      return ledger.OpenedAccountingDates();
    }


    public FixedList<NamedEntityDto> EventTypes() {
      return EventType.GetList()
                      .MapToNamedEntityList();
    }


    public FixedList<NamedEntityDto> SearchAccountsForVoucher(int voucherId, string keywords) {
      throw new NotImplementedException();
    }


    public FixedList<NamedEntityDto> TransactionTypes() {
      return TransactionType.GetList()
                            .MapToNamedEntityList();
    }


    public FixedList<NamedEntityDto> VoucherTypes() {
      return VoucherType.GetList()
                        .MapToNamedEntityList();
    }


    #endregion Use cases

  }  // class VouchersDataUseCases

}  // namespace Empiria.FinancialAccounting.Vouchers.UseCases
