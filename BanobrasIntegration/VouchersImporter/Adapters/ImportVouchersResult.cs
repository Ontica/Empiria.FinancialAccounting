/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                 Component : Vouchers Importer                    *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Data Transfer Object                 *
*  Type     : ImportVouchersResult                          License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Data transfer object with voucher importation result.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Collections.Generic;

namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter.Adapters {

  /// <summary>Data transfer object with voucher importation result.</summary>
  public class ImportVouchersResult {

    static public ImportVouchersResult Default {
      get {
        var list = new List<ImportVouchersTotals>();

        list.Add(new ImportVouchersTotals { UID = "1", Description = "IKOS Cash", VouchersCount = 31 });
        list.Add(new ImportVouchersTotals { UID = "2", Description = "IKOS Tesorería", VouchersCount = 11 });
        list.Add(new ImportVouchersTotals { UID = "3", Description = "Sistema de créditos (SIC)", VouchersCount = 1105 });
        list.Add(new ImportVouchersTotals { UID = "4", Description = "Sistema de fideicomisos (Yatla)", VouchersCount = 879 });
        list.Add(new ImportVouchersTotals { UID = "5", Description = "Sistema de presupuestos (PyC)", VouchersCount = 25 });

        return new ImportVouchersResult {
          VoucherTotals = list.ToFixedList()
        };
      }
    }


    public bool HasErrors {
      get {
        return (Errors.Count != 0);
      }
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


    public int ErrorsCount {
      get; internal set;
    }


    public int WarningsCount {
      get; internal set;
    }

  }  // class ImportVouchersTotals

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.VouchersImporter
