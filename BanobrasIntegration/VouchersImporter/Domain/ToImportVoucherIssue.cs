/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BalanceEngine.dll         Pattern   : Information Holder                   *
*  Type     : ToImportVoucherIssue                          License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Información de un registro de la tabla MC_ENCABEZADOS (Banobras) con pólizas por integrar.     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter {

  public enum VoucherIssueType {

    Warning,

    Error,

  }


  public class ToImportVoucherIssue {

    public ToImportVoucherIssue(VoucherIssueType type,
                                string importationSet,
                                string description) {
      this.Type = type;
      this.ImportationSet = importationSet;
      this.Location = string.Empty;
      this.Description = description;
    }

    public ToImportVoucherIssue(VoucherIssueType type,
                                string importationSet,
                                string location,
                                string description) {
      this.Type = type;
      this.ImportationSet = importationSet;
      this.Location = location;
      this.Description = description;
    }


    public VoucherIssueType Type {
      get;
    }


    public string ImportationSet {
      get;
    }


    public string Location {
      get;
    }


    public string Description {
      get;
    }


    public NamedEntityDto ToNamedEntity() {
      if (this.Location.Length != 0) {
        return new NamedEntityDto(this.ImportationSet, $"{this.Description} ({this.Location})");
      } else {
        return new NamedEntityDto(this.ImportationSet, $"{this.Description}");
      }
    }

  }  // class ToImportVoucherIssue

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
