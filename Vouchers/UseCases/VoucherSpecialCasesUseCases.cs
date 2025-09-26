/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Use case interactor class               *
*  Type     : VoucherSpecialCasesUseCases                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases used to generate special case vouchers.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.FinancialAccounting.Vouchers.Adapters;
using Empiria.Services;

namespace Empiria.FinancialAccounting.Vouchers.UseCases {

  /// <summary>Use cases used to generate special case vouchers.</summary>
  public class VoucherSpecialCasesUseCases : UseCase {

    #region Constructors and parsers

    protected VoucherSpecialCasesUseCases() {
      // no-op
    }

    static public VoucherSpecialCasesUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<VoucherSpecialCasesUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public string CreateAllSpecialCaseApplicableVouchers(VoucherSpecialCaseFields fields) {
      Assertion.Require(fields, "fields");

      var accountsChart = AccountsChart.Parse(fields.AccountsChartUID);

      FixedList<Ledger> ledgers;

      if (fields.GenerateForAllChildrenLedgers) {
        ledgers = accountsChart.MasterData.Ledgers.FindAll(x => !x.IsMasterLedger);
      } else {
        ledgers = accountsChart.MasterData.Ledgers;
      }

      int count = 0;

      foreach (var ledger in ledgers) {
        var ledgerFields = fields.Copy();

        ledgerFields.LedgerUID = ledger.UID;

        var builder = VoucherBuilder.CreateBuilder(ledgerFields);

        if (builder.TryGenerateVoucher(out _)) {
          count++;
        }
      }

      return $"Se generaron {count} pólizas.";
    }



    public VoucherDto CreateSpecialCaseVoucher(VoucherSpecialCaseFields fields) {
      Assertion.Require(fields, "fields");

      var builder = VoucherBuilder.CreateBuilder(fields);

      Voucher voucher = builder.GenerateVoucher();

      return VoucherMapper.Map(voucher);
    }


    public FixedList<VoucherSpecialCaseTypeDto> GetSpecialCaseTypes() {
      FixedList<VoucherSpecialCaseType> list = VoucherSpecialCaseType.GetList();

      list = base.RestrictUserDataAccessTo(list);

      return VoucherSpecialCaseTypeMapper.Map(list);
    }

    #endregion Use cases

  }  // class VoucherSpecialCasesUseCases

}  // namespace Empiria.FinancialAccounting.Vouchers.UseCases
