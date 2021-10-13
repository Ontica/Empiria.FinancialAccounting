/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BalanceEngine.dll         Pattern   : Information Holder                   *
*  Type     : ToImportVoucherIssue                          License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Información de un registro de la tabla MC_ENCABEZADOS (Banobras) con pólizas por integrar.     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  public enum VoucherIssueType {

    Warning,

    Error,

  }


  public class ToImportVoucherIssue {

    public ToImportVoucherIssue(VoucherIssueType type, string description) {
      this.Type = type;
      this.Description = description;
    }

    public VoucherIssueType Type {
      get;
    }

    public string Description {
      get;
    }

  }  // class ToImportVoucherIssue

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
