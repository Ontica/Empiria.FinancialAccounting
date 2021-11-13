/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Data Transfer Object                 *
*  Type     : ImportVouchersResult                          License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Data transfer object with voucher importation result.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters {

  /// <summary>Data transfer object with voucher importation result.</summary>
  public class ImportVouchersResult {


    public bool HasErrors {
      get {
        return (Errors.Count != 0);
      }
    }


    public bool IsRunning {
      get; internal set;
    }


    public FixedList<NamedEntityDto> Errors {
      get; internal set;
    } = new FixedList<NamedEntityDto>();


    public FixedList<NamedEntityDto> Warnings {
      get; internal set;
    } = new FixedList<NamedEntityDto>();


    public FixedList<ImportVouchersTotals> VoucherTotals {
      get; internal set;
    } = new FixedList<ImportVouchersTotals>();


  }  // class ImportVouchersResult



  /// <summary>Contains vouchers totals data separated by some unique id.</summary>
  public class ImportVouchersTotals {

    public string UID {
      get; internal set;
    }


    public string Description {
      get; internal set;
    }


    public int VouchersCount {
      get; internal set;
    }


    public int ProcessedCount {
      get; internal set;
    }


    public int ErrorsCount {
      get; internal set;
    }


    public int WarningsCount {
      get; internal set;
    }

  }  // class ImportVouchersTotals

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
