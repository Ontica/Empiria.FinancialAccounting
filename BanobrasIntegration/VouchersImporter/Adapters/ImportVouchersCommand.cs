/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Vouchers Importer                     *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Command payload                       *
*  Type     : ImportVouchersCommand                        License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Command payload used to import vouchers from Excel and text files.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters {

  /// <summary>Command payload used to import vouchers from Excel and text files.</summary>
  public class ImportVouchersCommand {

    public DateTime RecordingDate {
      get; set;
    }

    public string AccountsChartUID {
      get; set;
    }

    public string TransactionTypeUID {
      get; set;
    }

    public bool DistributeVouchers {
      get; set;
    }

    public bool GenerateSubledgerAccount {
      get; set;
    }

    public bool CanEditVoucherEntries {
      get; set;
    }

    public string Type {
      get; set;
    }

    public string Format {
      get; set;
    }

    public string Version {
      get; set;
    }

  } // class ImportVouchersCommand

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters
