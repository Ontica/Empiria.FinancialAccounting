﻿/* Empiria Financial *****************************************************************************************
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

using Empiria.FinancialAccounting.Adapters;

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

    public FixedList<NamedEntityDto> EventTypes() {
      FixedList<EventType> list = EventType.GetList();

      return new FixedList<NamedEntityDto>(list.Select(x => x.MapToNamedEntity()));
    }


    public FixedList<NamedEntityDto> FunctionalAreas() {
      FixedList<FunctionalArea> list = FunctionalArea.GetList();

      return new FixedList<NamedEntityDto>(list.Select(x => x.MapToNamedEntity()));
    }


    public FixedList<DateTime> OpenedAccountingDates(string accountsChartUID) {
      Assertion.Require(accountsChartUID, "accountsChartUID");

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      return accountsChart.OpenedAccountingDates();
    }


    public FixedList<LedgerAccountDto> SearchAccountsForVoucherEdition(long voucherId, string keywords) {
      Assertion.Require(voucherId > 0, "voucherId must be a positive number.");
      Assertion.Require(keywords, "keywords");

      var voucher = Voucher.Parse(voucherId);

      Assertion.Require(voucher.IsOpened, "No se pueden regresar cuentas para edición porque la póliza ya está cerrada.");

      FixedList<LedgerAccount> accounts = voucher.Ledger.SearchAssignedAccounts(keywords, voucher.AccountingDate);

      FixedList<Account> unassignedAccounts = voucher.Ledger.SearchUnassignedAccounts(keywords, voucher.AccountingDate);

      return LedgerMapper.MapAccountsForVoucherEdition(accounts, unassignedAccounts, voucher.AccountingDate);
    }


    public FixedList<SubledgerAccountDescriptorDto> SearchSubledgerAccountsForVoucherEdition(long voucherId,
                                                                                             int accountId,
                                                                                             string keywords) {
      Assertion.Require(voucherId > 0, "voucherId must be a positive number.");
      Assertion.Require(accountId > 0, "accountId must be a positive number.");
      Assertion.Require(keywords, "keywords");

      var voucher = Voucher.Parse(voucherId);

      var ledgerAccount = LedgerAccount.Parse(accountId);

      FixedList<SubledgerAccount> subledgerAccounts =
                            voucher.Helper.SearchSubledgerAccountsForEdition(ledgerAccount, keywords);

      return SubledgerMapper.MapToSubledgerAccountDescriptor(subledgerAccounts);
    }


    public FixedList<NamedEntityDto> TransactionalSystems() {
      var systemList = TransactionalSystem.GetList();

      return systemList.MapToNamedEntityList();
    }


    public FixedList<NamedEntityDto> TransactionTypes() {
      return TransactionType.GetList()
                            .MapToNamedEntityList();
    }


    public FixedList<NamedEntityDto> VoucherEditors(string keywords) {
      Assertion.Require(keywords, "keywords");

      FixedList<Participant> list = Participant.GetList(keywords);

      return new FixedList<NamedEntityDto>(list.Select(x => x.MapToNamedEntity()));
    }


    public FixedList<NamedEntityDto> VoucherTypes() {
      return VoucherType.GetList()
                        .MapToNamedEntityList();
    }

    #endregion Use cases

  }  // class VouchersDataUseCases

}  // namespace Empiria.FinancialAccounting.Vouchers.UseCases
